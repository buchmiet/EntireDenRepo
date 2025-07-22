using denModels.MarketplaceServices;
using denModels.OauthApi;
using Newtonsoft.Json;
using ServiceReference1;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text.Json;
using OauthApi;
using DataServicesNET80.DatabaseAccessLayer;
using Serilog;

namespace denEbayNET80;

public interface IEbayService
{
    HttpRequestMessage GetRefreshHeader(OauthTokenObject weToken);

    KeyValuePair<string, string> String2RefreshTokenAndAccessToken(string inputString, Action<string> pisz);

    Task<Dictionary<string, Dictionary<string, int>>> DoFeedback(string[] orderIds, Action<string> napisz, int locationID);

    Task<ebayOrders2.Rootobject> FetchEbayOrders2(Action<string> pisz, int locationID);

    Task MarkOrdersAsDispatched(Dictionary<string, Dictionary<string, int>> dobro, Dictionary<string, string> trackings, Action<string> pisz, int locationID);

    Task<ebayOrder2.Rootobject> GetEbayOrdersById2(Action<string> pisz, string[] orderId, int locationID);

    Task<EbayClient> GetEbayClient(int locationid);//OauthTokenObject token)

    Task<bool> MarkAsDispatchAndLeaveFeedback(Dictionary<string, string> OrdersTrackings, Action<string> notify, int locationId);

    Task<ItemType> GetItemAsync(string itemId, EbayClient kiebay);

    string GetB64EncodedString(string we);

    Task<InventoryStatusType[]> ReviseItem(InventoryStatusType[] inventory, EbayClient ec);

    Task<string> AddFixedPriceItemCall(ItemType item, EbayClient ec);

    Task<OrderType[]> GetEbayOrders(List<string> orderids, EbayClient ec);

    Task<UpdateQuantityResponse> SetQuantityToAndGetResponse(string itemId, int wartosc, int locationid);

    Task<bool> SetQuantityTo(string itemId, int wartosc, int locationID);

    Task<bool> SetPrice(string itemId, double newPrice, int locationID);

    Task<bool> CompleteSale(string carrierUsed, string shipmentTrackingNumber, string itemID, string transactionID, EbayClient ec);

    Task<bool> LeaveFeedback(string CommentText, CommentTypeCodeType cType, string ItemID, string TransactionID, string TargetUser, EbayClient ec);

    Task<bool> ReviseTitle(string itemId);
}

public class EbayService : IEbayService
{
    private IHttpClientFactory _httpClientFactory;
    private IOauthService _refreshTokenService;
    private HttpClient _httpClient;
    private OauthTokenHandler _oauthTokenHandler;

    public static string ServiceName => "ebay";

    public EbayService(IHttpClientFactory httpClientFactory, IOauthService refreshTokenService, IDatabaseAccessLayer databaseAccessLayer)
    {
        _refreshTokenService = refreshTokenService;
        _oauthTokenHandler = new OauthTokenHandler
        {
            RequestUrl = "https://api.ebay.com/",
            GetRequestHeaders = () => new Dictionary<string, string> { { "X-EBAY-C-MARKETPLACE-ID", "EBAY_GB" } },
            TokenRetriever = databaseAccessLayer.GetEbayToken,
            TokenSetter = databaseAccessLayer.UpdateEbayToken,
            Client = httpClientFactory.CreateClient(ServiceName),
            GetRefreshHeader = GetRefreshHeader,
            ConvertResponseStringToToken = String2RefreshTokenAndAccessToken
        };
        _refreshTokenService.AddOauthService(_oauthTokenHandler, ServiceName);
    }

    public HttpRequestMessage GetRefreshHeader(OauthTokenObject weToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/identity/v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", weToken.RefreshToken),
                new KeyValuePair<string, string>("scope", "https://api.ebay.com/oauth/api_scope/sell.fulfillment")
            ])
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", GetB64EncodedString(weToken.AppID + ':' + weToken.CertID));
        return request;
    }

    public KeyValuePair<string, string> String2RefreshTokenAndAccessToken(string inputString, Action<string> pisz)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        try
        {
            UserAccessToken toko =
                System.Text.Json.JsonSerializer.Deserialize<UserAccessToken>(inputString, options);
            return new KeyValuePair<string, string>(toko.refresh_token, toko.access_token);
        }
        catch (Exception ex)
        {
            pisz("Error while obtaining refresh token :" + ex.InnerException);
            return new KeyValuePair<string, string>("", "");
        }
    }

    public async Task<bool> MarkAsDispatchAndLeaveFeedback(Dictionary<string, string> OrdersTrackings, Action<string> notify, int locationId)
    {
        Log.Debug("Starting the DoFeedback process for {OrderCount} order IDs.", OrdersTrackings.Count);
        notify(string.Format(denLanguageResourses.Resources.LeavingFeedbackForOrders, OrdersTrackings.Count));
        string[] validOrderIds;
        var totalOrders = OrdersTrackings.Count;
        var pages = totalOrders / 50;
        var remainingOrders = totalOrders % 50;
        try
        {
            var ebayClient = await GetEbayClient(locationId);
            Log.Debug("Obtained eBay API client.");

            for (int i = 0; i <= pages; i++)
            {
                if (i < pages)
                {
                    validOrderIds = OrdersTrackings.Keys.Skip(50 * i).Take(50).ToArray();
                }
                else
                {
                    validOrderIds = OrdersTrackings.Keys.Skip(50 * i).Take(remainingOrders).ToArray();
                }

                Log.Debug("Processing page {CurrentPage} of {TotalPages}. Number of IDs on this page: {IdCount}.", i + 1, pages + 1, validOrderIds.Length);

                if (validOrderIds.Length > 0)
                {
                    var ordersDetails = await GetEbayOrdersById2(notify, validOrderIds, locationId).ConfigureAwait(false);
                    Log.Debug("Retrieved information about {OrderCount} eBay orders.", ordersDetails.orders.Length);

                    var legacyOrderIds = ordersDetails.orders.Select(order => order.legacyOrderId).ToList();
                    var orderFeedbacks = await GetEbayOrders(legacyOrderIds, ebayClient);
                    Log.Debug("Obtained data for feedback for {FetchedOrdersCount} orders.", orderFeedbacks.Length);

                    List<PreFeedbackComplete> feedbackDataList = new();
                    notify(denLanguageResourses.Resources.FetchedDataForFeedback);

                    foreach (var order in orderFeedbacks)
                    {
                        foreach (TransactionType transaction in order.TransactionArray)
                        {
                            var feedbackData = new PreFeedbackComplete
                            {
                                buyer = order.BuyerUserID,
                                itemId = transaction.Item.ItemID,
                                transactionId = transaction.TransactionID,
                                orderid = order.OrderID
                            };
                            feedbackDataList.Add(feedbackData);
                        }
                    }

                    var random = new Random();
                    int counter = 1;
                    foreach (var feedback in feedbackDataList)
                    {
                      
                        var result =
                            await CompleteSale("RoyalMail", OrdersTrackings[feedback.orderid], feedback.itemId, feedback.transactionId, ebayClient).ConfigureAwait(false);
                        if (!result)
                        {
                            notify(string.Format(denLanguageResourses.Resources.FeedbackLeftFailed, counter, feedbackDataList.Count, feedback.itemId, feedback.transactionId, feedback.buyer));
                            Log.Warning("Failed to leave feedback for ItemID: {ItemId}, TransactionID: {TransactionId}.", feedback.itemId, feedback.transactionId);
                        }
                        else
                        {
                            result = await LeaveFeedback(feedbacks[random.Next(feedbacks.Length)], CommentTypeCodeType.Positive, feedback.itemId, feedback.transactionId, feedback.buyer, ebayClient).ConfigureAwait(false);
                            if (result)
                            {
                                notify(string.Format(denLanguageResourses.Resources.FeedbackLeftSuccess, feedback.itemId, counter, feedbackDataList.Count));
                                Log.Information("Successfully left feedback for ItemID: {ItemId}, TransactionID: {TransactionId}.", feedback.itemId, feedback.transactionId);
                            }
                            else
                            {
                                notify(string.Format(denLanguageResourses.Resources.FeedbackLeftFailed, counter, feedbackDataList.Count, feedback.itemId, feedback.transactionId, feedback.buyer));
                                Log.Warning("Failed to leave feedback for ItemID: {ItemId}, TransactionID: {TransactionId}.", feedback.itemId, feedback.transactionId);
                            }
                        }
                        counter++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An exception occurred during the DoFeedback process.");
            throw; // Possible rethrow if you want to stop further execution.
        }

        Log.Debug("Completed the DoFeedback process.");
        return true;
    }

    public async Task<Dictionary<string, Dictionary<string, int>>> DoFeedback(string[] orderIds, Action<string> napisz, int locationID)
    {
        Log.Debug("Rozpoczynanie procesu DoFeedback dla {OrderCount} ID zamówień.", orderIds.Length);
        napisz(string.Format(denLanguageResourses.Resources.LeavingFeedbackForOrders, orderIds.Length));
        string[] dobreOrderIds;
        var wszystkie = orderIds.Length;
        var strony = wszystkie / 50;
        var koncowka = wszystkie % 50;
        var zwrotka = new Dictionary<string, Dictionary<string, int>>();
        try
        {
            var ec = await GetEbayClient(locationID);
            Log.Debug("Uzyskano klienta eBay API.");

            for (int x = 0; x <= strony; x++)
            {
                if (x < strony)
                {
                    dobreOrderIds = orderIds.Skip(50 * x).Take(50).ToArray();
                }
                else
                {
                    dobreOrderIds = orderIds.Skip(50 * x).Take(koncowka).ToArray();
                }

                Log.Debug("Przetwarzanie strony {CurrentPage} z {TotalPages}. Liczba ID na tej stronie: {IdCount}.", x + 1, strony + 1, dobreOrderIds.Length);

                if (dobreOrderIds.Length > 0)
                {
                    ebayOrder2.Rootobject go2 = await GetEbayOrdersById2(napisz, dobreOrderIds, locationID).ConfigureAwait(false);
                    Log.Debug("Pobrano informacje o {OrderCount} zamówieniach z eBay.", go2.orders.Length);

                    var ordery = go2.orders.Select(ord => ord.orderId).ToList();
                    foreach (var ord in go2.orders)
                    {
                        var ko = ord.lineItems.ToDictionary(li => li.lineItemId, li => li.quantity);
                        zwrotka.Add(ord.orderId, ko);
                    }
                    var hoho = await GetEbayOrders(ordery, ec);
                    Log.Debug("Uzyskano dane dla opinii dla {FetchedOrdersCount} zamówień.", hoho.Length);

                    var dofi = new List<PreFeedbackComplete>();
                    napisz(denLanguageResourses.Resources.FetchedDataForFeedback);

                    foreach (var ord in hoho)
                    {
                        foreach (TransactionType traka in ord.TransactionArray)
                        {
                            var pre = new PreFeedbackComplete
                            {
                                buyer = ord.BuyerUserID,
                                itemId = traka.Item.ItemID,
                                transactionId = traka.TransactionID
                            };
                            dofi.Add(pre);
                        }
                    }

                    var ran = new Random();
                    int i = 1;
                    foreach (var dof in dofi)
                    {
                        var kuku = await LeaveFeedback(feedbacks[ran.Next(feedbacks.Length)], CommentTypeCodeType.Positive, dof.itemId, dof.transactionId, dof.buyer, ec).ConfigureAwait(false);
                        if (kuku)
                        {
                            napisz(string.Format(denLanguageResourses.Resources.FeedbackLeftSuccess, dof.itemId, i, dofi.Count));
                            Log.Information("Pomyślnie wystawiono opinię dla ItemID: {ItemId}, TransactionID: {TransactionId}.", dof.itemId, dof.transactionId);
                        }
                        else
                        {
                            napisz(string.Format(denLanguageResourses.Resources.FeedbackLeftFailed, i, dofi.Count, dof.itemId, dof.transactionId, dof.buyer));
                            Log.Warning("Nie udało się wystawić opinii dla ItemID: {ItemId}, TransactionID: {TransactionId}.", dof.itemId, dof.transactionId);
                        }
                        i++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Wystąpił wyjątek podczas procesu DoFeedback.");
            throw; // Możliwe ponowne zgłoszenie wyjątku, jeśli chcesz przerwać dalsze wykonanie.
        }

        Log.Debug("Zakończono proces DoFeedback.");
        return zwrotka;
    }

    public async Task<ebayOrders2.Rootobject> FetchEbayOrders2(Action<string> pisz, int locationID)
    {
        var jsonstring = (await _refreshTokenService.GetResponseFromApiCall(ServiceName, "sell/fulfillment/v1/order", "limit=200&filter:orderfulfillmentstatus:%7BNOT_STARTED%7CIN_PROGRESS%7D", pisz, locationID, false).ConfigureAwait(false)).ResponseString;
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        return JsonConvert.DeserializeObject<ebayOrders2.Rootobject>(jsonstring);
    }

    public async Task MarkOrdersAsDispatched(Dictionary<string, Dictionary<string, int>> dobro, Dictionary<string, string> trackings, Action<string> pisz, int locationID)
    {
        foreach (var dobre in dobro)
        {
            var callname = "sell/fulfillment/v1/order/" + dobre.Key + "/shipping_fulfillment";
            var ship = new ShippingFullFillmentDetails.Rootobject
            {
                shippedDate = ""
            };

            if (trackings.ContainsKey(dobre.Key))
            {
                ship.trackingNumber = trackings[dobre.Key];
                ship.shippingCarrierCode = "RoyalMail";
            }
            else
            {
                ship.trackingNumber = "";
            }
            ship.lineItems = new ShippingFullFillmentDetails.Lineitem[dobre.Value.Count];
            int i = 0;
            foreach (var itema in dobre.Value)
            {
                ship.shippedDate = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'MM':'ss.fff") + "Z";
                ship.lineItems[i] = new ShippingFullFillmentDetails.Lineitem
                {
                    lineItemId = itema.Key,
                    quantity = itema.Value
                };
                i++;
            }
            var json = System.Text.Json.JsonSerializer.Serialize(ship);
            var ku = await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName, OauthRequestType.POST, callname, json, pisz, locationID, false);
        }
    }

    public async Task<ebayOrder2.Rootobject> GetEbayOrdersById2(Action<string> pisz, string[] orderId, int locationID)
    {
        string str = "orderIds=";
        for (int i = 0; i < orderId.Length; i++)
        {
            str += orderId[i];
            if (i < orderId.Length - 1) { str += ','; }
        }

        Log.Debug("Pobieranie zamówień eBay dla ID: {OrderIds}", str);
        string orderki = null;
        try
        {
            orderki = (await _refreshTokenService.GetResponseFromApiCall(ServiceName, "sell/fulfillment/v1/order", str, pisz, locationID, false).ConfigureAwait(false)).ResponseString;

            if (string.IsNullOrEmpty(orderki))
            {
                Log.Error("Odpowiedź API jest pusta dla zapytania: {Query}", str);
                return null;
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IncludeFields = true,
            };

            var result = System.Text.Json.JsonSerializer.Deserialize<ebayOrder2.Rootobject>(orderki, options);
            Log.Information("Deserializacja zamówień eBay zakończona sukcesem dla zapytania: {Query}", str);

            return result;
        }
        catch (System.Text.Json.JsonException jsonEx)
        {
            Log.Error(jsonEx, "Błąd podczas deserializacji danych zamówień eBay: {OrderData}", orderki);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Wystąpił błąd podczas pobierania danych zamówień z eBay dla zapytania: {Query}", str);
            throw;
        }
    }

    private const string Version = "1131";

    private const string BaseApiUrl = "https://api.ebay.com/wsapi";

    // This is the actual client from the service we just imported. It's being injected
    // here via the built-in DI in ASP.NET Core.
    private static eBayAPIInterfaceClient _ebay;

    // All of the functions in this class need these credentials passed, so declare it in
    // the constructor to make things easier.
    private static CustomSecurityHeaderType _creds;

    public async Task<EbayClient> GetEbayClient(int locationid)//OauthTokenObject token)
    {
        var weToken = await _oauthTokenHandler.TokenRetriever(locationid, false);
        var zwro = new EbayClient
        {
            Client = new eBayAPIInterfaceClient(eBayAPIInterfaceClient.EndpointConfiguration.eBayAPI),
            Credentials = new CustomSecurityHeaderType
            {
                Credentials = new UserIdPasswordType
                {
                    AppId = weToken.AppID,
                    DevId = weToken.DevID,
                    AuthCert = weToken.CertID
                },
                eBayAuthToken = weToken.AccessToken
            }
        };
        return zwro;
    }

    public async Task<ItemType> GetItemAsync(string itemId, EbayClient kiebay)
    {
        // All of the API requests and responses use their own type of object.
        // This one, naturally, uses GetItemRequest and GetItemResponse.
        var reqType = new GetItemRequest
        {
            GetItemRequest1 = new GetItemRequestType
            {
                ItemID = itemId,
                Version = Version,
                DetailLevel = [DetailLevelCodeType.ReturnAll]
            },
            RequesterCredentials = kiebay.Credentials
        };
        // The service isn't smart enough to know the endpoint URLs itself, so
        // we have to set it explicitly.
        var addr = new EndpointAddress($"{BaseApiUrl}?callname=GetItem&siteid=3");
        // This creates a channel from the built-in client's ChannelFactory.
        // See the WCF docs for explanation of this step.
        //if (_ebay == null)
        //{
        //StartEbayClient();
        // }
        var ch = kiebay.Client.ChannelFactory.CreateChannel(addr);
        // Actually call the API now
        ServiceReference1.GetItemResponse itemResp = new();
        try
        {
            itemResp = await ch.GetItemAsync(reqType).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
        }
        // Check that the call was a success
        if (itemResp.GetItemResponse1.Ack == AckCodeType.Success)
        {
            return itemResp.GetItemResponse1.Item;
        }
        // Handle an API error however you want. Throw an
        // exception or return null, whatever works for you.
        return null;
    }

    public string GetB64EncodedString(string we)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(we);
        return Convert.ToBase64String(plainTextBytes, 0, plainTextBytes.Length,
            Base64FormattingOptions.None);
    }

    public async Task<InventoryStatusType[]> ReviseItem(InventoryStatusType[] inventory, EbayClient ec)
    {
        var reqType = new ReviseInventoryStatusRequest
        {
            ReviseInventoryStatusRequest1 = new ReviseInventoryStatusRequestType
            {
                InventoryStatus = inventory,
                Version = Version
            },
            RequesterCredentials = ec.Credentials
        };
        var addr = new EndpointAddress($"{BaseApiUrl}?callname=ReviseInventoryStatus&siteid=3");
        var ch = ec.Client.ChannelFactory.CreateChannel(addr);
        var invRes = await ch.ReviseInventoryStatusAsync(reqType).ConfigureAwait(false);
        if (invRes.ReviseInventoryStatusResponse1.Ack == AckCodeType.Success)
        {
            return invRes.ReviseInventoryStatusResponse1.InventoryStatus;
        }
        return null;
    }

    public async Task<string> AddFixedPriceItemCall(ItemType item, EbayClient ec)
    {
        var reqType = new AddFixedPriceItemRequest
        {
            AddFixedPriceItemRequest1 = new AddFixedPriceItemRequestType
            {
                Item = item,
                Version = Version
            },
            RequesterCredentials = ec.Credentials
        };
        var addr = new EndpointAddress($"{BaseApiUrl}?callname=AddFixedPriceItem&siteid=3");
        var ch = ec.Client.ChannelFactory.CreateChannel(addr);
        var addIt = await ch.AddFixedPriceItemAsync(reqType).ConfigureAwait(false);
        if (addIt.AddFixedPriceItemResponse1.Ack != AckCodeType.Failure)
        {
            return addIt.AddFixedPriceItemResponse1.ItemID;
        }
        return null;
    }

    public async Task<OrderType[]> GetEbayOrders(List<string> orderids, EbayClient ec)
    {
        // All of the API requests and responses use their own type of object.
        // This one, naturally, uses GetItemRequest and GetItemResponse.

        var reqType = new GetOrdersRequest
        {
            GetOrdersRequest1 = new GetOrdersRequestType
            {
                OrderIDArray = orderids.ToArray(),
                DetailLevel = new DetailLevelCodeType[1] { DetailLevelCodeType.ReturnSummary },
                Version = Version
            },
            RequesterCredentials = ec.Credentials
        };
        // The service isn't smart enough to know the endpoint URLs itself, so
        // we have to set it explicitly.
        var addr = new EndpointAddress($"{BaseApiUrl}?callname=GetOrders&siteid=3");
        // This creates a channel from the built-in client's ChannelFactory.
        // See the WCF docs for explanation of this step.
        var ch = ec.Client.ChannelFactory.CreateChannel(addr);
        // Actually call the API now
        //   var ordersResp = await ch.GetOrdersAsync(reqType);
        var ordersResp = await Task.Run(() => ch.GetOrdersAsync(reqType)).ConfigureAwait(false);
        // Check that the call was a success
        if (ordersResp.GetOrdersResponse1.Ack == AckCodeType.Success)
        {
            return ordersResp.GetOrdersResponse1.OrderArray;
        }
        // Handle an API error however you want. Throw an
        // exception or return null, whatever works for you.
        return null;
    }

    public async Task<UpdateQuantityResponse> SetQuantityToAndGetResponse(string itemId, int wartosc, int locationid)
    {
        var zwrotka = new UpdateQuantityResponse
        {
            ItemNumber = itemId
        };
        var ec = await GetEbayClient(locationid);
        ItemType u = await GetItemAsync(itemId, ec).ConfigureAwait(false);
        if (u == null)
        {
            zwrotka.Status = UpdateQuantityStatus.Error;
            zwrotka.Response = new ServerResponse
            {
                Code = denLanguageResourses.Resources.CannotConnectEbay
            };
            return zwrotka;
        }
        if (u.Variations != null)
        {
            try
            {
                var qua = u.Variations.Variation[0].Quantity - u.Variations.Variation[0].SellingStatus.QuantitySold +
                    u.Variations.Variation[1].Quantity - u.Variations.Variation[1].SellingStatus.QuantitySold;

                if (qua == wartosc)
                {
                    zwrotka.Status = UpdateQuantityStatus.NotAttempted;
                    zwrotka.Response = new ServerResponse
                    {
                        Code = denLanguageResourses.Resources.QuantitySameRevisionSkipped
                    };
                    return zwrotka;
                }
                qua = wartosc;
                int not = qua / 2;
                not += qua % 2;
                int wt = qua - not;
                var xx = new InventoryStatusType[2];
                xx[0] = new InventoryStatusType
                {
                    ItemID = itemId,
                    SKU = "noTool",
                    Quantity = not,
                    QuantitySpecified = true
                };
                xx[1] = new InventoryStatusType
                {
                    ItemID = itemId,
                    SKU = "withTool",
                    Quantity = wt,
                    QuantitySpecified = true
                };
                var rx = await ReviseItem(xx, ec).ConfigureAwait(false);
                zwrotka.Status = UpdateQuantityStatus.OK;
                zwrotka.Response = new ServerResponse
                {
                    Code = denLanguageResourses.Resources.EbayConfirmationResponse
                };
                return zwrotka;
            }
            catch (Exception ex)
            {
                zwrotka.Status = UpdateQuantityStatus.Error;
                zwrotka.Response = new ServerResponse
                {
                    Code = denLanguageResourses.Resources.ErrorTitle,
                    Message = ex.Message
                };
                return zwrotka;
            }
        }
        else
        {
            try
            {
                var qua = u.Quantity - u.SellingStatus.QuantitySold;
                if (qua == wartosc)
                {
                    zwrotka.Status = UpdateQuantityStatus.NotAttempted;
                    zwrotka.Response = new ServerResponse
                    {
                        Code = denLanguageResourses.Resources.QuantitySameRevisionSkipped
                    };
                    return zwrotka;
                }
                var xx = new InventoryStatusType[1];
                xx[0] = new InventoryStatusType
                {
                    ItemID = itemId,
                    Quantity = wartosc,
                    QuantitySpecified = true
                };
                var rx = await ReviseItem(xx, ec).ConfigureAwait(false);
                zwrotka.Status = UpdateQuantityStatus.OK;
                zwrotka.Response = new ServerResponse
                {
                    Code = denLanguageResourses.Resources.EbayConfirmationResponse
                };
                return zwrotka;
            }
            catch (Exception ex)
            {
                zwrotka.Status = UpdateQuantityStatus.Error;
                zwrotka.Response = new ServerResponse
                {
                    Code = denLanguageResourses.Resources.ErrorTitle,
                    Message = ex.Message
                };
                return zwrotka;
            }
        }
    }

    public async Task<bool> SetQuantityTo(string itemId, int wartosc, int locationID)
    {
        var ec = await GetEbayClient(locationID);
        ItemType u = await GetItemAsync(itemId, ec).ConfigureAwait(false);
        if (u == null) { return false; }

        if (u.Variations != null)
        {
            try
            {
                var qua = u.Variations.Variation[0].Quantity - u.Variations.Variation[0].SellingStatus.QuantitySold +
                    u.Variations.Variation[1].Quantity - u.Variations.Variation[1].SellingStatus.QuantitySold;

                if (qua == wartosc)
                {
                    return true;
                }
                qua = wartosc;
                int not = qua / 2;
                not += qua % 2;
                int wt = qua - not;
                var xx = new InventoryStatusType[2];
                xx[0] = new InventoryStatusType
                {
                    ItemID = itemId,
                    SKU = "noTool",
                    Quantity = not,
                    QuantitySpecified = true
                };
                xx[1] = new InventoryStatusType
                {
                    ItemID = itemId,
                    SKU = "withTool",
                    Quantity = wt,
                    QuantitySpecified = true
                };
                var rx = await ReviseItem(xx, ec).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
        }
        else
        {
            try
            {
                var qua = u.Quantity - u.SellingStatus.QuantitySold;
                if (qua == wartosc)
                {
                    return true;
                }
                var xx = new InventoryStatusType[1];
                xx[0] = new InventoryStatusType
                {
                    ItemID = itemId,
                    Quantity = wartosc,
                    QuantitySpecified = true
                };
                var rx = await ReviseItem(xx, ec).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        return true;
    }

    public async Task<bool> ReviseTitle(string itemId)
    {
        var ec = await GetEbayClient(1);
        ItemType u = await GetItemAsync(itemId, ec).ConfigureAwait(false);
        if (u == null)
        {
            return false;
        }

        u.Title += "changed";
        try
        {
            var reqType = new ReviseItemRequest()
            {
                ReviseItemRequest1 = new ReviseItemRequestType
                {
                    Item = u,
                    Version = Version
                },
                RequesterCredentials = ec.Credentials
            };
            var addr = new EndpointAddress($"{BaseApiUrl}?callname=ReviseItem&siteid=3");
            var ch = ec.Client.ChannelFactory.CreateChannel(addr);
            var invRes = await ch.ReviseItemAsync(reqType).ConfigureAwait(false);
            if (invRes.ReviseItemResponse1.Ack == AckCodeType.Success)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> SetPrice(string itemId, double newPrice, int locationID)
    {
        var ec = await GetEbayClient(locationID);
        ItemType item = await GetItemAsync(itemId, ec).ConfigureAwait(false);
        item.QuantitySpecified = false;
        //       int currentQuantity = item.Quantity;
        if (item != null)
        {
            if (item.Variations != null)
            {
                // dla każdej wariacji, aktualizujemy cenę
                foreach (var variation in item.Variations.Variation)
                {
                    //  variation.Quantity = variation.Quantity;
                    if (variation.SKU == "noTool")
                    {
                        variation.StartPrice.Value = newPrice;
                    }
                    else if (variation.SKU == "withTool")
                    {
                        variation.StartPrice.Value = newPrice + 1;
                    }
                }
            }
            else
            {
                //  item.Quantity = currentQuantity;
                // jeśli nie ma wariacji, aktualizujemy cenę na nową cenę dla "noTool"
                item.StartPrice.Value = newPrice;
            }

            var reqType = new ReviseFixedPriceItemRequest
            {
                ReviseFixedPriceItemRequest1 = new ReviseFixedPriceItemRequestType
                {
                    Item = item,
                    Version = Version
                },
                RequesterCredentials = ec.Credentials
            };
            var addr = new EndpointAddress($"{BaseApiUrl}?callname=ReviseFixedPriceItem&siteid=3");
            var ch = ec.Client.ChannelFactory.CreateChannel(addr);
            var invRes = await ch.ReviseFixedPriceItemAsync(reqType).ConfigureAwait(false);
            if (invRes.ReviseFixedPriceItemResponse1.Ack == AckCodeType.Success)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public class PreFeedbackComplete
    {
        public string itemId;
        public string transactionId;
        public string buyer;
        public string orderid;
    }

    public static string[] feedbacks =  { "Good buyer, prompt payment, valued customer, highly recommended",
        "Thank you for an easy, pleasant transaction. Excellent buyer. A++++++.",
        "Quick response and fast payment. Perfect! THANKS!!",
        "Hope to deal with you again. Thank you."
    };

    
    public async Task<bool> CompleteSale(string carrierUsed, string shipmentTrackingNumber, string itemID, string transactionID, EbayClient ec)
    {
        var reqType = new CompleteSaleRequest
        {
            CompleteSaleRequest1 = new CompleteSaleRequestType
            {
                ItemID = itemID,
                TransactionID = transactionID,
                Shipment = new ShipmentType
                {
                    ShipmentTrackingDetails =
                    [
                        new ShipmentTrackingDetailsType
                        {
                            ShipmentTrackingNumber = shipmentTrackingNumber,
                            ShippingCarrierUsed = carrierUsed,
                        }
                    ]
                },
                Shipped = true,
                Version = Version
            },
            RequesterCredentials = ec.Credentials,
        };
        var addr = new EndpointAddress($"{BaseApiUrl}?callname=CompleteSale&siteid=3");
        // This creates a channel from the built-in client's ChannelFactory.
        // See the WCF docs for explanation of this step.
        var ch = ec.Client.ChannelFactory.CreateChannel(addr);
        // Actually call the API now
        var completeSaleResponse = await ch.CompleteSaleAsync(reqType).ConfigureAwait(false);
        // Check that the call was a success
        if (completeSaleResponse.CompleteSaleResponse1.Ack != AckCodeType.Failure)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> LeaveFeedback(string CommentText, CommentTypeCodeType cType, string ItemID, string TransactionID, string TargetUser, EbayClient ec)
    {
        // All of the API requests and responses use their own type of object.
        // This one, naturally, uses GetItemRequest and GetItemResponse.

        var reqType = new LeaveFeedbackRequest
        {
            LeaveFeedbackRequest1 = new LeaveFeedbackRequestType
            {
                CommentText = CommentText,
                CommentType = CommentTypeCodeType.Positive,
                CommentTypeSpecified = true,
                ItemID = ItemID,
                TransactionID = TransactionID,
                TargetUser = TargetUser,
                Version = Version,
            },
            RequesterCredentials = ec.Credentials,
        };
        // The service isn't smart enough to know the endpoint URLs itself, so
        // we have to set it explicitly.
        var addr = new EndpointAddress($"{BaseApiUrl}?callname=LeaveFeedback&siteid=3");
        // This creates a channel from the built-in client's ChannelFactory.
        // See the WCF docs for explanation of this step.
        var ch = ec.Client.ChannelFactory.CreateChannel(addr);
        // Actually call the API now
        LeaveFeedbackResponse FeedBackResp = await ch.LeaveFeedbackAsync(reqType).ConfigureAwait(false);
        // Check that the call was a success
        if (FeedBackResp.LeaveFeedbackResponse1.Ack == AckCodeType.Success)
        {
            return true;
        }
        // Handle an API error however you want. Throw an
        // exception or return null, whatever works for you.
        return false;
    }
}