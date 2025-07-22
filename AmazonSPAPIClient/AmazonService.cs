using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Models;
using denModels.MarketplaceServices;
using denModels.OauthApi;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OauthApi;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;


namespace AmazonSPAPIClient;

public interface IAmazonSpApi
{
    Task<GetOrdersResponse> GetOrders(List<string> marketplaceIds, List<AmazonSpApi.OrderStatus> orderStatus,
        DateTime? createdAfter = null, DateTime? createdBefore = null, Action<string>? write =
            null, int locationId = 1);
    HttpRequestMessage GetRefreshHeader(OauthTokenObject weToken);
    KeyValuePair<string, string> String2RefreshTokenAndAccessToken(string inputString, Action<string> write);
    Dictionary<string, string> GetRequestHeaders();
    AmazonInfo AmaOrdExtractor(List<string> we);
    Dictionary<Type, KeyValuePair<double, int>> AmazonRates { get; set; }
    Task<GetOrderItemsResponse.Payload> GetOrderItems(string orderId,Action<string> pisz,int locationid);
    KeyValuePair<customer, billaddr> AmazonCustomer2MyCustomer(GetOrdersResponse.Order o);
    Task MarkAmazonOrdersAsShipped(List<DataNededToMarkAmazonOrderAsShipped> orders, Action<string> pisz,int locationid);
    Task<GetOrdersResponse> GetParticularOrders(List<string> marketplaceIds, List<string> OrderIds, Action<string> pisz, int locationid);
    Task<GetOrdersResponse> GetNewOrders(List<string> marketplaceIds, Action<string> pisz, int locationid);
    Task<List<KeyValuePair<string, int>>> GetOrderItemsIds(string orderId, Action<string> pisz, int locationid);
    string UpdateOrAddNextToken(string parameters, string nextToken);
    Task<T> GetJsonResponse<T>( string call, string parameters, Action<string> pisz,int locationid) where T : IHasNextToken, new();
    Task<ConfirmShipmentRequest> MarkOrderAsShipped(string orderId, string trackingNumber, string marketPlaceID, int myorderID, Action<string> pisz,int locationid);
    Task<KeyValuePair<bool, string>> PostJsonResponse( string call, string jsonString,Action<string> pisz, int locationid);
    string ByteArrayToHexString(byte[] bytes);
    string GetSerializedPatchRequest(int quantity);
    Task<HttpResponseMessage> SendPatchRequest(HttpClient client, string signature, OauthTokenObject weToken, string call, string parameters, char delimit, string jsonString);
    Task<string> PatchJsonBodyAndGetResponse<T>(string call,  string jsonString,Action<string> pisz,int locationid);
    Task<OauthTokenObject> Refresh(OauthTokenObject weToken, AmazonSpApi.UpdateAmazonTokenDelegate updateAmazonTokenDelegate);
    Task<OauthTokenObject> UpdateAmazonToken(OauthTokenObject weToken,AmazonRefreshTokenResponse response, AmazonSpApi.UpdateAmazonTokenDelegate updateAmazonTokenDelegate);
    Task<UpdateQuantityResponse> UpdateQuantityAndGetResponse(int quantity, string sku, string AmazonmarketID, Action<string> pisz, int locationid);
    
}

public class AmazonSpApi : IAmazonSpApi
{
    private readonly IOauthService _refreshTokenService;

    private readonly OauthTokenHandler _oauthTokenHandler;

    public static String ServiceName ="amazon";
    private readonly AmazonApiSettings _settings;
    public AmazonSpApi(IHttpClientFactory httpClientFactory, IOauthService refreshTokenService,IDatabaseAccessLayer databaseAccessLayer, IOptions<AmazonApiSettings> amazonSettings)
    {
        _settings = amazonSettings.Value;
        var httpClient = httpClientFactory.CreateClient(ServiceName);
        _refreshTokenService = refreshTokenService;
        _oauthTokenHandler = new OauthTokenHandler
        {
            RequestUrl = _settings.BaseUrl,
            GetRequestHeaders = GetRequestHeaders,
            TokenRetriever = databaseAccessLayer.GetAmazonToken,
            TokenSetter = databaseAccessLayer.UpdateAmazonToken,
            Client = httpClient,
            GetRefreshHeader = GetRefreshHeader,
            ConvertResponseStringToToken = String2RefreshTokenAndAccessToken
        };
        _refreshTokenService.AddOauthService(_oauthTokenHandler, ServiceName);
    }


    public HttpRequestMessage GetRefreshHeader(OauthTokenObject weToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUrl)
        {
            Content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token",weToken.RefreshToken),
                new KeyValuePair<string, string>("client_id",weToken.Client_id),
                new KeyValuePair<string, string>("client_secret",weToken.Client_secret)
            ])
        };
        return request;
    }




    public KeyValuePair<string, string> String2RefreshTokenAndAccessToken(string inputString, Action<string> write)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        try
        {
            AmazonRefreshTokenResponse toko = System.Text.Json.JsonSerializer.Deserialize<AmazonRefreshTokenResponse>(inputString, options);
            return new KeyValuePair<string, string>(toko.refresh_token, toko.access_token);
        }
        catch (Exception ex)
        {
            write("Error while obtaining refresh token :" + ex.InnerException);
            return new KeyValuePair<string, string>("", "");
        }
    }

    public Dictionary<string, string> GetRequestHeaders()
    {
        string amzDate = DateTime.UtcNow.ToString("yyyyMMdd");
        byte[] kSecret = Encoding.UTF8.GetBytes("AWS4" + _settings.SecretAccessKey);
        byte[] kDate = ComputeHmacSha256(kSecret, amzDate);
        byte[] kRegion = ComputeHmacSha256(kDate, _settings.Region);
        byte[] kService = ComputeHmacSha256(kRegion, "execute-api");
        byte[] kSigning = ComputeHmacSha256(kService, "aws4_request");
        var signature = ByteArrayToHexString(kSigning);
        string authorizationHeader = $"AWS4-HMAC-SHA256 Credential={_settings.AccessKeyId}/{amzDate}/{_settings.Region}/execute-api/aws4_request, SignedHeaders=accept;host;x-amz-access-token;x-amz-date, Signature={signature}";
        return new Dictionary<string, string>
        {
            {"x-amz-access-token",_oauthTokenHandler.OauthToken.OauthToken },
            {"X-Amz-Date", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")},
            {"Authorization", authorizationHeader }
        };
    }


    public static Dictionary<string, string> AmazonPairs = new()
    {
        { "Dispatch to:", "Order ID:" }, //UK
        { "Liefern an:", "Bestellnummer:" }, //Germany
        { "Adresse d'expédition :", "Numéro de la commande :" }, //France
        { "Enviar a:", "Nº de pedido:" }, //Spain
        { "Spedire a:", "Numero dell'ordine:" }, //Italy
        { "Wyślij do:", "Identyfikator zamówienia:" }, //Poland
        { "Adresse de livraison :", "Numéro de la commande :" }, //Belgium
        { "Verzenden naar:","Bestelnummer"} //Netherlands
    };

    public AmazonInfo AmaOrdExtractor(List<string> we)
    {
        AmazonInfo input = new AmazonInfo { adres = "" };
          
        var s22 = we.First();
        string s2 = AmazonPairs[s22];
        we.Remove(s22);
        s22 = we.First();
        while (!s22.StartsWith(s2))
        {
            if (!string.IsNullOrEmpty(s22))
            {
                input.adres += s22 + Environment.NewLine;
            }
            we.Remove(s22);
            s22 = we.First();
        }
        input.orderid = s22.Substring(s2.Length).Trim();
        int i = 0;
        do
        {
            s22 = we.First();
            if (AmazonPairs.ContainsKey(s22))
            {
                if (i < 10)
                {
                    we.Remove(s22);
                    i++;
                    s22 = "";
                }
            }
            else
            {
                we.Remove(s22);
                i++;
            }
        } while (!AmazonPairs.ContainsKey(s22) && (we.Count > 0));

        input.reszta = we;
        return input;
    }

    private static Dictionary<Type, KeyValuePair<double, int>> _amazonRates = new()
    {
        {typeof(GetOrdersResponse),new KeyValuePair<double, int> (0.0167,20)},
        {typeof(GetOrderItemsResponse),new KeyValuePair<double, int>(0.5,30) },
        {typeof(GetItemResponse.Rootobject),new KeyValuePair<double,int> (5,10)  } ,
        {typeof(ListingsItemPatchRequest),new KeyValuePair<double, int>(5,10) }
    };


    public delegate Task UpdateAmazonTokenDelegate(string refreshToken, string access_token, int locationId);

    public async Task<GetOrderItemsResponse.Payload> GetOrderItems(string orderId,Action<string> pisz,int locationid)
    {
          
        GetOrderItemsResponse response = await GetJsonResponse<GetOrderItemsResponse>( "orders/v0/orders/" + orderId + "/orderItems", "", pisz,locationid);
        return response.payload;
    }

    public  KeyValuePair<customer, billaddr> AmazonCustomer2MyCustomer(GetOrdersResponse.Order o)
    {
          

        var billAddr = new billaddr
        {
            Line1 = "none",
            Line2 = "none",
            City = o.ShippingAddress.City,
            PostalCode = o.ShippingAddress.PostalCode,
            CountrySubDivisionCode = o.ShippingAddress.StateOrRegion,
            CountryCode = o.ShippingAddress.CountryCode

        };
        if (billAddr.PostalCode == null)
        {
            billAddr.PostalCode = "0000";
        }

        string[] names = new string[2];
        List<string> nazwy;

        nazwy = ["Jeff", "Bezos"];

        names[0] = nazwy[0];
        if (nazwy.Count() == 2)
        {
            names[1] = nazwy[1];
        }
        else names[1] = "";
        var cust = new customer
        {
            CompanyName = "",
            Title = "",
            GivenName = names[0],
            MiddleName = "",
            FamilyName = names[1],
            DisplayName = names[0] + ' ' + names[1] + ' ' + o.BuyerInfo.BuyerEmail,
            Email = o.BuyerInfo.BuyerEmail,
            Phone = "0000000",
            currency = o.OrderTotal.CurrencyCode,
            billaddrID = billAddr.billaddrID
        };

        return new KeyValuePair<customer, billaddr>(cust, billAddr);
    }


    public  async Task MarkAmazonOrdersAsShipped(List<DataNededToMarkAmazonOrderAsShipped> orders, Action<string> pisz,int locationid)
    {
        var orderystatusy = new Dictionary<string, string>();

        int batchSize = 50;
        for (int i = 0; i < orders.Count; i += batchSize)
        {
            var currentBatch = orders.Skip(i).Take(batchSize).ToList();
            var batchMarketPlaceIds = currentBatch.Select(p => p.MarketPlaceId).ToList();
            var batchAmazonOrderIds = currentBatch.Select(p => p.AmazonOrderId).ToList();

            var batchStatusy = await GetParticularOrders(batchMarketPlaceIds, batchAmazonOrderIds,pisz,locationid);
            foreach (var ord in batchStatusy.Payload.Orders)
            {
                orderystatusy.Add(ord.AmazonOrderId, ord.OrderStatus);
            }
        }

        foreach (var ord in orders)
        {
            var ordek = orderystatusy[ord.AmazonOrderId];
            if (ordek.ToLower().Equals("shipped"))
            {
                pisz("Order " + ord.AmazonOrderId + " already marked as shipped, skipping" + Environment.NewLine);
            }
            else
            {
                await MarkOrderAsShipped(ord.AmazonOrderId, ord.Tracking, ord.MarketPlaceId, ord.Orderid, pisz,locationid);
            }
        }
    }


    public  async Task<GetOrdersResponse> GetParticularOrders(List<string> marketplaceIds, List<string> OrderIds, Action<string> pisz, int locationid)
    {
        var sztri = "MarketplaceIds=";
        var thelastone = marketplaceIds.Last();
        foreach (var item in marketplaceIds)
        {
            sztri += item;
            if (!thelastone.Equals(item))
            {
                sztri += ',';
            }
        }
        sztri += "&AmazonOrderIds=";
        thelastone = OrderIds.Last();
        foreach (var item in OrderIds)
        {
            sztri += item;
            if (!thelastone.Equals(item))
            {
                sztri += ',';
            }
        }



        return await GetJsonResponse<GetOrdersResponse>( "orders/v0/orders", sztri, pisz,locationid);
        
    }

    public enum OrderStatus
    {
        PendingAvailability,
        Pending,
        PartiallyShipped,
        Unshipped,
        InvoiceUnconfirmed,
        Canceled,
        Unfulfillable,
        Shipped,
        Error
    }


    public async Task<GetOrdersResponse> GetOrders(List<string> marketplaceIds,List<OrderStatus> orderStatus,DateTime? createdAfter=default, DateTime? createdBefore=default, Action<string>? write=
        null, int locationId=1)
    {
        StringBuilder query = new();
        query.Append("MarketplaceIds=");
        query.Append(string.Join(",", marketplaceIds));
        query.Append("&");
        query.Append("OrderStatuses=");
        query.Append(string.Join(",", orderStatus));
        query.Append("&");
        if (createdAfter != DateTime.MinValue&&createdAfter is { } after)
        {
            query.Append("CreatedAfter=");
            query.Append(after.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            query.Append("&");
        }
        if (createdBefore != DateTime.MinValue && createdAfter is { } before)
        {
            query.Append("CreatedAfter=");
            query.Append(before.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            query.Append("&");
        }

        query.Remove(query.Length - 1, 1);
        return await GetJsonResponse<GetOrdersResponse>("orders/v0/orders", query.ToString(), write, locationId);
    }

    public async Task<GetOrdersResponse> GetNewOrders(List<string> marketplaceIds, Action<string> pisz, int locationid) =>
        await GetOrders(marketplaceIds, [OrderStatus.Unshipped], DateTime.Now.AddDays(-5),DateTime.MinValue, pisz, locationid);
           
        

    public  async Task<List<KeyValuePair<string, int>>> GetOrderItemsIds(string orderId, Action<string> pisz, int locationid)
    {
           
        GetOrderItemsResponse response = await GetJsonResponse<GetOrderItemsResponse>( "orders/v0/orders/" + orderId + "/orderItems", "", pisz,locationid);

        List<KeyValuePair<string, int>> result = response.payload.OrderItems
            .Select(oi => new KeyValuePair<string, int>(oi.OrderItemId, oi.QuantityOrdered))
            .ToList();

        return result;
    }

    public string UpdateOrAddNextToken(string parameters, string nextToken)
    {
        var paramDict = HttpUtility.ParseQueryString(parameters);
        paramDict["NextToken"] = nextToken;

        return string.Join('&', paramDict.AllKeys.Select(key => $"{key}={paramDict[key]}"));
    }

    public async Task<T> GetJsonResponse<T>( string call, string parameters, Action<string> pisz,int locationid) where T : IHasNextToken, new()
    {
        T responseObj = default;
        string previousNextToken = null;
        int retryCount = 0;
        const int maxRetries = 5;
        double rate = AmazonRates[typeof(T)].Key;
        TimeSpan refillInterval = TimeSpan.FromSeconds(1 / rate);
        do
        {
            
            var timeSinceLastRequest = DateTime.Now - LastRequestTime;
            int availableTokens = AmazonRates[typeof(T)].Value;
            availableTokens += (int)(timeSinceLastRequest.TotalSeconds * rate);
            availableTokens = Math.Min(availableTokens, AmazonRates[typeof(T)].Value);
            LastRequestTime = DateTime.Now;

            if (availableTokens <= 0)
            {
                await Task.Delay(refillInterval);
                continue;
            }
            availableTokens--;

            var response =
                await _refreshTokenService.GetResponseFromApiCall(ServiceName, call, parameters, pisz, locationid,true);

            if (response.ResponseHeaders.TryGetValues("x-amzn-RateLimit-Limit", out var rateLimitValues))
            {
                var rateLimit = rateLimitValues.FirstOrDefault();
                var tokensPerSecond = double.Parse(rateLimit.Split(' ')[0]);
                rate = tokensPerSecond;
                refillInterval = TimeSpan.FromSeconds(1 / rate);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                T apiResponse = System.Text.Json.JsonSerializer.Deserialize<T>(response.ResponseString,options);
                   
                if (responseObj == null)
                {
                    responseObj = apiResponse;
                }
                else
                {
                    responseObj.Merge(apiResponse);
                }
                if (!string.IsNullOrEmpty(apiResponse.GetNextToken()))
                {
                    if (previousNextToken == apiResponse.GetNextToken())
                    {
                        retryCount++;
                        if (retryCount >= maxRetries)
                        {
                            break;
                        }
                    }
                    else
                    {
                        retryCount = 0;
                    }

                    previousNextToken = apiResponse.GetNextToken();
                    string encodedNextToken = WebUtility.UrlEncode(apiResponse.GetNextToken());
                    parameters = UpdateOrAddNextToken(parameters, encodedNextToken);
                }
                else  // Jeśli NextToken jest pusty, przerywamy pętlę
                {
                    break;
                }
            }
            else if (response.StatusCode == (HttpStatusCode)429)
            {
                await Task.Delay(_exponentialBackoff);
                _exponentialBackoff = TimeSpan.FromSeconds(_exponentialBackoff.TotalSeconds * 2);
            }
              
            else
            {
                   
                break;
            }

        } while (true);

        return responseObj;
    }


  

    public  async Task<ConfirmShipmentRequest> MarkOrderAsShipped(string orderId, string trackingNumber, string marketPlaceID, int myorderID, Action<string> pisz,int locationid)
    {

        // Pobranie listy orderItemId dla danego zamówienia
        pisz("Getting items for order " + orderId + "... ");
        var orderItemIds = await GetOrderItemsIds(orderId,pisz,locationid);
        pisz("done" + Environment.NewLine);

        // Tworzenie listy przedmiotów w zamówieniu na podstawie pobranych orderItemIds
        List<OrderItem> orderItems = orderItemIds.Select(id => new OrderItem
        {
            orderItemId = id.Key,
            quantity = id.Value
        }).ToList();

        // Tworzenie ciała żądania
        ConfirmShipmentRequest request = new ConfirmShipmentRequest
        {
            packageDetail = new PackageDetail
            {
                packageReferenceId = myorderID.ToString(),
                carrierCode = _settings.DefaultCarrierCode, 
                carrierName = _settings.DefaultCarrierCode,
                shippingMethod = _settings.DefaultShippingMethod,
                trackingNumber = string.IsNullOrWhiteSpace(trackingNumber) ? "NA" : trackingNumber,
                shipDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                orderItems = orderItems
            },
            marketplaceId = marketPlaceID
        };
        pisz("Marking order " + orderId + " as dispatched... ");
        var whathappened = await PostJsonResponse("orders/v0/orders/" + orderId + "/shipmentConfirmation",  JsonConvert.SerializeObject(request), pisz,locationid);
        if (whathappened.Key)
        {
            pisz("done" + Environment.NewLine);
        }
        else
        {
            pisz("error: " + whathappened.Value + Environment.NewLine);
        }
        return request;
    }

    public  async Task<KeyValuePair<bool, string>> PostJsonResponse( string call, string jsonString,Action<string> pisz, int locationid)
    {
        TimeSpan exponentialBackoff = TimeSpan.FromSeconds(1);

        do
        {
            var response =
                await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName, OauthRequestType.POST, call, jsonString, pisz, locationid,true);
              
               
            if (response.ResponseHeaders .TryGetValues("x-amzn-RateLimit-Limit", out var rateLimitValues))
            {
                var rateLimit = rateLimitValues.FirstOrDefault();
                double.Parse(rateLimit.Split(' ')[0]);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new KeyValuePair<bool, string>(true, "");
            }
            else if (response.StatusCode == (HttpStatusCode)429) // Throttling
            {
                await Task.Delay(exponentialBackoff);
                exponentialBackoff = TimeSpan.FromSeconds(exponentialBackoff.TotalSeconds * 2);
            }
            else
            {
                AmazonErrorResponse blad = System.Text.Json.JsonSerializer.Deserialize<AmazonErrorResponse>(response.ResponseString);
                return new KeyValuePair<bool, string>(false, blad.errors[0].message);
            }
        } while (true);
    }

    public  string ByteArrayToHexString(byte[] bytes)
    {
        StringBuilder hex = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }

    public  string GetSerializedPatchRequest(int quantity)
    {
        var patchRequest = new ListingsItemPatchRequest
        {
            productType = "PRODUCT",  
            patches =
            [
                new PatchOperation
                {
                    op = "replace",
                    path = "/attributes/fulfillment_availability",
                    value =
                    [
                        new()
                        {
                            fulfillment_channel_code = "DEFAULT",
                            quantity = quantity
                        }
                    ]
                }
            ]
        };
        return JsonConvert.SerializeObject(patchRequest);
    }

     

    public  DateTime LastRequestTime = DateTime.Now;
    private  TimeSpan _exponentialBackoff = TimeSpan.FromSeconds(1);

    public  Dictionary<Type, KeyValuePair<double, int>> AmazonRates { get => _amazonRates; set => _amazonRates = value; }

    public async Task<HttpResponseMessage> SendPatchRequest(HttpClient client, string signature, OauthTokenObject weToken, string call, string parameters, char delimit, string jsonString)
    {
        var callurl = "https://sellingpartnerapi-eu.amazon.com/" + call;
        if (!string.IsNullOrEmpty(parameters))
        {
            callurl += delimit + parameters;
        }

        string amzDate = DateTime.UtcNow.ToString("yyyyMMdd");
        string authorizationHeader = $"AWS4-HMAC-SHA256 Credential={_settings.AccessKeyId}/{amzDate}/{_settings.Region}/execute-api/aws4_request, SignedHeaders=accept;host;x-amz-access-token;x-amz-date, Signature={signature}";


        var request = new HttpRequestMessage(HttpMethod.Patch, callurl);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("x-amz-access-token", weToken.AccessToken);
        request.Headers.Add("X-Amz-Date", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);

        request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var ga = await client.SendAsync(request).ConfigureAwait(false);
        return ga;
    }

        
    public  async Task<string> PatchJsonBodyAndGetResponse<T>(string call,  string jsonString,Action<string> pisz,int locationid)
    {
        int retryCount = 0;
        double rate = AmazonRates[typeof(T)].Key;          
        TimeSpan refillInterval = TimeSpan.FromSeconds(1 / rate);
        do
        {
             

            // Obsługa token bucket
            var timeSinceLastRequest = DateTime.Now - LastRequestTime;
            int availableTokens = AmazonRates[typeof(T)].Value;
            availableTokens += (int)(timeSinceLastRequest.TotalSeconds * rate);
            availableTokens = Math.Min(availableTokens, AmazonRates[typeof(T)].Value);
            LastRequestTime = DateTime.Now;

            if (availableTokens <= 0)
            {                
                await Task.Delay(refillInterval);
                continue;
            }
            availableTokens--;

            var response =await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName,
                OauthRequestType.PATCH, call, jsonString, pisz, locationid,true);
                
            if (response.ResponseHeaders.TryGetValues("x-amzn-RateLimit-Limit", out var rateLimitValues))
            {
                var rateLimit = rateLimitValues.FirstOrDefault();
                var tokensPerSecond = double.Parse(rateLimit.Split(' ')[0]);
                rate = tokensPerSecond;
                refillInterval = TimeSpan.FromSeconds(1 / rate);
                  
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
              
                return response.ResponseString;

            }

            if (response.StatusCode == (HttpStatusCode)429)
            {                  
                await Task.Delay(_exponentialBackoff);
                _exponentialBackoff = TimeSpan.FromSeconds(_exponentialBackoff.TotalSeconds * 2);
            }
            else
            {
                return response.ResponseString; 
                  
            }
        } while (true);
          
    }


    public  async Task<OauthTokenObject> Refresh(OauthTokenObject weToken, UpdateAmazonTokenDelegate updateAmazonTokenDelegate)
    {



        string jsonString = "";

        using (var client = new HttpClient())
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token",weToken.RefreshToken),
                new KeyValuePair<string, string>("client_id",weToken.Client_id),
                new KeyValuePair<string, string>("client_secret",weToken.Client_secret)
            });
            var result = await client.PostAsync("https://api.amazon.com/auth/o2/token", content);
            jsonString = result.Content.ReadAsStringAsync().Result;
        }
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        AmazonRefreshTokenResponse toko = System.Text.Json.JsonSerializer.Deserialize<AmazonRefreshTokenResponse>(jsonString, options);
        return await UpdateAmazonToken(weToken,toko, updateAmazonTokenDelegate);
    }

    public  async Task<OauthTokenObject> UpdateAmazonToken(OauthTokenObject weToken,AmazonRefreshTokenResponse response, UpdateAmazonTokenDelegate updateAmazonTokenDelegate)
    {
        await updateAmazonTokenDelegate(response.refresh_token, response.access_token, 1);
        return new OauthTokenObject
        {
            Client_secret = weToken.Client_secret,
            Client_id = weToken.Client_id,
            AccessToken = response.access_token,
            RefreshToken = response.refresh_token,
        };
    }


    public  async Task<UpdateQuantityResponse> UpdateQuantityAndGetResponse(int quantity, string sku, string AmazonmarketID, Action<string> pisz, int locationid)
    {
            
           
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        string callUrl = $"listings/2021-08-01/items/{_settings.SellerId}/{sku}?marketplaceIds={AmazonmarketID}";
        var tempValue = await PatchJsonBodyAndGetResponse<ListingsItemPatchRequest>(callUrl,  GetSerializedPatchRequest(quantity), pisz,locationid);
        var returnValue = new UpdateQuantityResponse
        {
            ItemNumber = sku
        };
        if (tempValue.Contains("errors"))
        {
            returnValue.Status = UpdateQuantityStatus.Error;
            var err = System.Text.Json.JsonSerializer.Deserialize<AmazonErrorResponse>(tempValue, options);
            returnValue.Response = new ServerResponse
            {
                Code = err.errors[0].code,
                Message = err.errors[0].message,
            };
            return returnValue;                
        }

        var ok= System.Text.Json.JsonSerializer.Deserialize<AmazonUpdateItemResponse>(tempValue, options);
        returnValue.Status = UpdateQuantityStatus.OK;
        returnValue.Response = new ServerResponse
        {
            Code = ok.status,
            Message = "submission id:" + ok.submissionId
        };
        return returnValue;
    }



    private static byte[] ComputeHmacSha256(byte[] key, string message)
    {
        using var hmacsha256 = new HMACSHA256(key);
        return hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));
    }



}