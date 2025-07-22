using DataServicesNET80.Extensions;
using DataServicesNET80.Models;
using System.Linq.Expressions;
using Answers;
using denModels.OauthApi;
using EntityEvents;
using Microsoft.EntityFrameworkCore;

namespace DataServicesNET80.DatabaseAccessLayer;

public partial class DatabaseAccessLayer : IDatabaseAccessLayer
{
    public AsyncLazy<T> CreateAsyncLazy<T>(Func<Task<T>> taskFactory)
    {
        return new AsyncLazy<T>(taskFactory);
    }

    public event EventHandler<ItemBodiesChangedEventArgs> ItemBodiesChanged;

    public virtual void
        
        
        
        
        OnItemBodiesChanged(List<int> changedBodies)
    {
        ItemBodiesChanged?.Invoke(this, new ItemBodiesChangedEventArgs { ChangedBodies = changedBodies });
    }


    public async Task<Answer> GetItemBodyId(int itembodyid)
    {
        var response = Answer.Prepare($"Getting item body with itembodyid {itembodyid}");
        itembody? zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<itembody>(unitOfWork);
            zwrotka = await service.GetOneAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return response.Error("Error when accessing database");
        }

        return response.WithValue(zwrotka);
    }

    public async Task<Answer> GetItemBodyId(string mpn)
    {
        var response = Answer.Prepare($"Getting item body with mpn '{mpn}'");
        itembody? zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<itembody>(unitOfWork);
            zwrotka = await service.GetOneAsync(p => p.mpn == mpn).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return response.Error("Error when accessing database");
        }

        return response.WithValue(zwrotka.itembodyID);
    }

    public async Task<shopitem> GetShopItemByItembodyId(int itembodyid)
    {
        shopitem zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<shopitem>(unitOfWork);
            zwrotka = await service.GetOneAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return zwrotka;
    }

    public async Task UpdateShopItem(shopitem shopitem)
    {
        shopitem zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<shopitem>(unitOfWork);
            var sklepitem = (await service.GetByIdAsync(shopitem.shopitemId).ConfigureAwait(false)).GetValue<shopitem>();
            sklepitem.price = sklepitem.price;
            await service.UpdateAsync(sklepitem).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));
    }

    public async Task<itembody[]> GetAlsoFitting(int watchID)
    {
        using (var db = new Time4PartsContext())
        {
            try
            {
                var query = from ib in db.itembodies
                    join bg in db.bodiesgroupeds on ib.itembodyID equals bg.itemBodyID
                    join g4b in db.group4bodies on bg.group4bodiesID equals g4b.group4bodiesID
                    join maf in db.mayalsofits on g4b.group4bodiesID equals maf.group4bodiesID
                    join g4w in db.group4watches on maf.group4watchesID equals g4w.group4watchesID
                    join wg in db.watchesgroupeds on g4w.group4watchesID equals wg.group4watchesID
                    join w in db.watches on wg.watchID equals w.watchID
                    where w.watchID == watchID
                    select ib;

                var result = await query.ToArrayAsync();
                return result;
            }
            catch (Exception ex)
            {
                // Obsługa wyjątku
                return null;
            }
        }
    }

    public async Task<int> GetNumberOfordersWithCondition(Expression<Func<order, bool>> predicate)
    {
        int zwrotka = -2;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new orderService(unitOfWork);
            zwrotka = await service.CountAsync(predicate).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return -2;
        }

        return zwrotka;
    }

    public async Task<OauthTokenObject> GetTokenForServiceAsync(int locationId, string serviceName)
    {
        //using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
        //var tokenRepository = new EntityService<OauthToken>(unitOfWork);
        //var serviceRepository = new EntityService<OauthService>(unitOfWork);

        //var service = await serviceRepository.GetOneAsync(s => s.ServiceName == serviceName);

        //if (service == null)
        //    throw new Exception($"Service {serviceName} not found");

        //var token = await tokenRepository.GetOneAsync(t =>
        //    t.LocationId == locationId && t.ServiceId == service.OauthServiceId);

        //if (token == null)
        //    return null;

        //  return new OauthTokenObject(token);
        return null;
    }

    public async Task<OauthTokenObject> UpdateTokenAsync(int locationId, string serviceName,
        string accessToken = null, string refreshToken = null,
        bool logEachEvent = false)
    {
        //await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        //{
        //    using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        //    var tokenRepository = new EntityService<OauthToken>(unitOfWork);
        //    var serviceRepository = new EntityService<OauthService>(unitOfWork);

        //    var service = await serviceRepository.GetOneAsync(s => s.ServiceName == serviceName);

        //    if (service == null)
        //        throw new Exception($"Service {serviceName} not found");

        //    var token = await tokenRepository.GetOneAsync(t =>
        //        t.LocationId == locationId && t.ServiceId == service.OauthServiceId);

        //    if (token == null)
        //        throw new Exception($"Token for location {locationId} and service {serviceName} not found");

        //    if (!string.IsNullOrEmpty(refreshToken))
        //        token.RefreshToken = refreshToken;

        //    if (!string.IsNullOrEmpty(accessToken))
        //        token.AccessToken = accessToken;

        //    await tokenRepository.UpdateAsync(token);


        //}, TimeSpan.FromSeconds(10));

        return await GetTokenForServiceAsync(locationId, serviceName);
    }



    //public async Task<Answer> UpdateTokens(string refresh_token, string oauth_token, int locationId,
    //    string serviceName)
    //{
    //    var response = Answer.Prepare($"Updating tokens for location {locationId} and service {serviceName}");
    //    DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
    //    {
    //        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
    //        var oldTokenService = new EntityService<token>(unitOfWork);
    //        var tokenService= new EntityService<OauthToken>(unitOfWork);
    //      //  var fullTokenViewService=new EntityService<OauthTokenWithService>(unitOfWork);

    //        var tokenToUpdate= await tokenService.GetOneAsync(t => t.LocationId == locationId && t.Service.ServiceName == serviceName);
    //        if (tokenToUpdate == null)
    //        {
    //            response=response.Error($"Token for location {locationId} and service {serviceName} not found");
    //        }

    //        var nowyTok = await oldTokenService.GetOneAsync(p => p.locationID == locationID).ConfigureAwait(false);
    //        if (!string.IsNullOrEmpty(Refreshtoken))
    //        {
    //            nowyTok.quickBooksRefresh = Refreshtoken;
    //        }
    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            nowyTok.quickBooksToken = token;
    //        }
    //        await oldTokenService.UpdateAsync(nowyTok).ConfigureAwait(false);
    //        NewToken.RefreshToken = Refreshtoken;
    //        NewToken.AccessToken = token;
    //    }, TimeSpan.FromSeconds(10));

    //    return response;
    //}

    public async Task<QuickBooksTokenObject> UpdateQuickbooksTokenNew(string refreshToken, string token, int locationId, bool logEachEvent)
    {
        await UpdateTokenAsync(locationId, "quickbooks", token, refreshToken, logEachEvent);

        return new QuickBooksTokenObject
        {
            RefreshToken = refreshToken,
            AccessToken = token
        };
    }
    public async Task<QuickBooksTokenObject> UpdateQuickbooksToken(string Refreshtoken, string token, int locationID, bool logEachEvent)
    {
        var NewToken = new QuickBooksTokenObject();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<token>(unitOfWork);
            var nowyTok = await service.GetOneAsync(p => p.locationID == locationID).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(Refreshtoken))
            {
                nowyTok.quickBooksRefresh = Refreshtoken;
            }
            if (!string.IsNullOrEmpty(token))
            {
                nowyTok.quickBooksToken = token;
            }
            await service.UpdateAsync(nowyTok).ConfigureAwait(false);
            NewToken.RefreshToken = Refreshtoken;
            NewToken.AccessToken = token;
        }, TimeSpan.FromSeconds(10));
        //      await UpdateTokenAsync(locationID, "quickbooks", access_token, Refreshtoken, true);
        return NewToken;
    }

    public async Task UpdateEbayToken(string refresh_token, string oauth_token, int locationID, bool logEachEvent)
    {
        int zwrotka = -2;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<token>(unitOfWork);
            var weToken = await service.GetOneAsync(p => p.locationID == locationID).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(oauth_token))
            {
                weToken.ebayOauthToken = oauth_token;
            }

            if (!string.IsNullOrEmpty(refresh_token))
            {
                weToken.ebayRefreshToken = refresh_token;
            }

            await service.UpdateAsync(weToken).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));
        await UpdateTokenAsync(locationID, "amazon", oauth_token, refresh_token, true);
    }

    public async Task UpdateAmazonToken(string refresh_token, string access_token, int locatioid, bool LogEachEvent)
    {
        int zwrotka = -2;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<token>(unitOfWork);
            var logentryService = new EntityService<logentry>(unitOfWork);
            var stringa = string.Format($"saving Amazon's AmazonSPAPIRefreshToken {refresh_token}, AmazonSPAPIToken {access_token}");
            await logentryService.AddAsync(new logentry { eventdate = DateTime.Now, _event = stringa }).ConfigureAwait(false);
            var weToken = await service.GetOneAsync(p => p.locationID == locatioid).ConfigureAwait(false);
            weToken.AmazonSPAPIRefreshToken = refresh_token;
            weToken.AmazonSPAPIToken = access_token;
            await service.UpdateAsync(weToken).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));
        await UpdateTokenAsync(locatioid, "amazon",access_token,refresh_token,true);
    }

    public async Task<OauthTokenObject> GetAmazonToken(int locationId, bool logeachevent)
    {
        OauthTokenObject zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<token>(unitOfWork);
            var weToken = await service.GetOneAsync(p => p.locationID == locationId).ConfigureAwait(false);

            var logentryService = new EntityService<logentry>(unitOfWork);
            var stringa = string.Format($"reading Amazon's AmazonSPAPIRefreshToken {weToken.AmazonSPAPIRefreshToken}, AmazonSPAPIToken {weToken.AmazonSPAPIToken}, AmazonSPAPIClientID {weToken.AmazonSPAPIClientID},AmazonSPAPIClientSecret {weToken.AmazonSPAPIClientSecret}");
            await logentryService.AddAsync(new logentry { eventdate = DateTime.Now, _event = stringa }).ConfigureAwait(false);

            zwrotka = new OauthTokenObject
            {
                RefreshToken = weToken.AmazonSPAPIRefreshToken,
                OauthToken = weToken.AmazonSPAPIToken,
                //       AccessToken = weToken.AmazonSPAPIToken,
                Client_id = weToken.AmazonSPAPIClientID,
                Client_secret = weToken.AmazonSPAPIClientSecret
            };
        }, TimeSpan.FromSeconds(10));
        //OauthTokenObject comparableToken = (await GetToken(locationId, "amazon")).GetValue<OauthTokenObject>();
        //if (zwrotka.RefreshToken != comparableToken.RefreshToken || zwrotka.OauthToken != comparableToken.OauthToken)
        //{

        //}
        return zwrotka;
    }

    public async Task<OauthTokenObject> GetQuickBooksToken(int locationid, bool logEachEvent)
    {
        OauthTokenObject zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<token>(unitOfWork);
            var weToken = await service.GetOneAsync(p => p.locationID == locationid).ConfigureAwait(false);
            zwrotka = new OauthTokenObject
            {
                RefreshToken = weToken.quickBooksRefresh,
                OauthToken = weToken.quickBooksToken,
            };
        }, TimeSpan.FromSeconds(10));
        //var comparableToken2 = await GetToken(locationid, "quickbooks");
        // var comparableToken= comparableToken2.GetValue<OauthTokenObject>();
        //if (zwrotka.RefreshToken != comparableToken.RefreshToken || zwrotka.OauthToken != comparableToken.OauthToken)
        //{
        //}
        return zwrotka;
    }

    public async Task<OauthTokenObject> GetEbayToken(int locationid, bool logEachEvent)
    {
        OauthTokenObject zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<token>(unitOfWork);
            var weToken = await service.GetOneAsync(p => p.locationID == locationid).ConfigureAwait(false);
            zwrotka = new OauthTokenObject
            {
                RefreshToken = weToken.ebayRefreshToken,
                AccessToken = weToken.ebayToken,
                OauthToken = weToken.ebayOauthToken,
                AppID = weToken.AppID,
                CertID = weToken.CertID,
                DevID = weToken.DevID
            };
        }, TimeSpan.FromSeconds(10));
        //OauthTokenObject comparableToken = (await GetToken(locationid, "ebay")).GetValue<OauthTokenObject>();
        //if (zwrotka.RefreshToken != comparableToken.RefreshToken || zwrotka.OauthToken != comparableToken.OauthToken)
        //{

        //}
        return zwrotka;
    }



    public async Task<Answer> GetToken(int locationId, string serviceName) //OauthTokenObject
    {
        var response = Answer.Prepare($"Getting token for location {locationId} and service {serviceName}");
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var ouathService = new EntityService<OauthToken>(unitOfWork);
            var serviceEntityService = new EntityService<OauthService>(unitOfWork);
            var service = await serviceEntityService.GetOneAsync(s => s.ServiceName == serviceName).ConfigureAwait(false);
            var servvices =(await ouathService.GetAllAsync().ConfigureAwait(false)).ToList();
            var serviceToken = await ouathService.GetOneAsync(p => p.LocationId == locationId&&p.ServiceId==service.OauthServiceId).ConfigureAwait(false);
            if (serviceToken == null)
            {
                response = response.Error("Failed to receive token");
            }
            else
            {
                response = response.WithValue(serviceToken);
            }

        }, TimeSpan.FromSeconds(10));
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return response.Error("Failed to receive token");
        }
        return response;
    }




    public async Task UpdateInvoiceTXN(invoicetxn invoice)
    {
        int zwrotka = -2;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var invoiceTXNservice = new EntityService<invoicetxn>(unitOfWork);
            var oldInvoice = await invoiceTXNservice.GetOneAsync(p => p.invoiceTXNID == invoice.invoiceTXNID).ConfigureAwait(false);
            oldInvoice.qbInvoiceId = invoice.qbInvoiceId;
            await invoiceTXNservice.UpdateAsync(oldInvoice).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));
    }

    public async Task Updateorder(order order)
    {
        int zwrotka = -2;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var orderService = new orderService(unitOfWork);
            var oldorder = await orderService.GetOneAsync(p => p.orderID == order.orderID).ConfigureAwait(false);
            oldorder.quickbooked = order.quickbooked;
            await orderService.UpdateAsync(oldorder).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task<List<Complete>> GetKompletyWithGivenStatusesAndLocation(List<string> statuses, int locationId)
    {
        var komplety = new List<Complete>();
        int i = 0;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<completeview>(unitOfWork);
            var kompy = await service.GetAllAsync(p => statuses.Contains(p.status) && locationId == locationId).ConfigureAwait(false);
            var orderyid = kompy.Select(p => p.orderID).ToList();
            Console.WriteLine($"komplety {kompy.Count()}");

            foreach (var komp in await service.GetAllAsync(p => statuses.Contains(p.status) && locationId == locationId).ConfigureAwait(false))
            {
                Complete komplet;
                Console.WriteLine($"analizuje order {komp.orderID}");
                if (!komplety.Any(p => p.Order.orderID == komp.orderID))
                {
                    Console.WriteLine("to nowy order");
                    komplet = new Complete
                    {
                        Order = new(),
                        Customer = new(),
                        BillAddr = new(),
                        OrderItems = new()
                    };
                    komplet.Order.orderID = komp.orderID;
                    komplet.Order.VAT = komp.VAT;
                    komplet.Order.xchgrate = komp.xchgrate;
                    komplet.Order.saletotal = komp.saletotal;
                    komplet.Order.status = komp.status;
                    komplet.Order.salecurrency = komp.salecurrency;
                    komplet.Order.acquiredcurrency = komp.acquiredcurrency;
                    komplet.Order.customerID = komp.customerID;
                    komplet.Order.dispatchedOn = komp.dispatchedOn;
                    komplet.Order.locationID = komp.locationID;
                    komplet.Order.paidOn = komp.paidOn;
                    komplet.Order.VATRateID = komp.VATRateID;
                    komplet.Order.quickbooked = komp.quickbooked;
                    komplet.Order.tracking = komp.tracking;
                    komplet.Order.order_notes = komp.order_notes;
                    komplet.Order.postagePrice = komp.postagePrice;
                    komplet.Order.postageType = komp.postageType;
                    komplet.Order.market = komp.market;
                    komplet.Customer.customerID = komp.customerID;
                    komplet.Customer.Title = komp.Title;
                    komplet.Customer.GivenName = komp.GivenName;
                    komplet.Customer.MiddleName = komp.MiddleName;
                    komplet.Customer.FamilyName = komp.FamilyName;
                    komplet.Customer.CompanyName = komp.CompanyName;
                    komplet.Customer.Email = komp.Email;
                    komplet.Customer.Phone = komp.Phone;
                    komplet.Customer.DisplayName = komp.DisplayName;
                    komplet.Customer.currency = komp.customer_currency;
                    komplet.Customer.customer_notes = komp.customer_notes;
                    komplet.Customer.billaddrID = komp.billaddrID;
                    komplet.BillAddr.Line1 = komp.Line1;
                    komplet.BillAddr.Line2 = komp.Line2;
                    komplet.BillAddr.City = komp.City;
                    komplet.BillAddr.CountryCode = komp.CountryCode;
                    komplet.BillAddr.CountrySubDivisionCode = komp.CountrySubDivisionCode;
                    komplet.BillAddr.PostalCode = komp.PostalCode;
                    komplet.BillAddr.AddressAsAString = komp.AddressAsAString;
                    komplet.BillAddr.billaddrID = (int)komp.billaddrID;
                    komplety.Add(komplet);

                    i++;
                    Console.WriteLine($"order nr {i} dodany do kolekcji");
                }
                else
                {
                    Console.WriteLine("order o tym id zostal juz dodany");
                    komplet = komplety.First(p => p.Order.orderID == komp.orderID);
                }

                if (komp.OrderItemId != null)
                {
                    var oi = new orderitem
                    {
                        OrderItemId = (int)komp.OrderItemId,
                        itemName = komp.itemName,
                        quantity = (int)komp.quantity,
                        itembodyID = (int)komp.itembodyID,
                        OrderItemTypeId = (int)komp.OrderItemTypeId,
                        price = (decimal)komp.price,
                        orderID = komp.orderID,
                        itmMarketAssID = komp.itmMarketAssID,
                        ItemWeight = komp.ItemWeight
                    };
                    komplet.OrderItems.Add(oi);
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        Console.WriteLine($"jest w sumie {i} orderow");
        return komplety;
    }

    public async IAsyncEnumerable<Complete> GetKompletyAsyncStream(List<int> ids)
    {
        using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
        var service = new EntityService<completeview>(unitOfWork);

        var orderItemsBuffer = new Dictionary<int, List<orderitem>>();
        int? lastOrderId = null;
        Complete currentComplete = null;

        await foreach (var viewRecord in service.GetAllAsyncStream(x => ids.Contains(x.orderID)))
        {
            if (lastOrderId.HasValue && lastOrderId.Value != viewRecord.orderID)
            {
                // Zwróć obiekt Complete dla poprzedniego orderID
                // Zwróć obiekt Complete dla poprzedniego orderIDGetCurrentAndPreviousDayorders
                yield return currentComplete;

                // Resetuj bufor dla nowego orderID
                orderItemsBuffer.Clear();
                currentComplete = viewrecord2COmplete(viewRecord);
            }
            else if (!lastOrderId.HasValue)
            {
                currentComplete = viewrecord2COmplete(viewRecord);
            }

            if (viewRecord.OrderItemId != null)
            {
                var orderItem = new orderitem
                {
                    OrderItemId = (int)viewRecord.OrderItemId,
                    itemName = viewRecord.itemName,
                    quantity = (int)viewRecord.quantity,
                    itembodyID = (int)viewRecord.itembodyID,
                    OrderItemTypeId = (int)viewRecord.OrderItemTypeId,
                    price = (decimal)viewRecord.price,
                    orderID = viewRecord.orderID,
                    itmMarketAssID = viewRecord.itmMarketAssID,
                    ItemWeight = viewRecord.ItemWeight
                };
                currentComplete.OrderItems.Add(orderItem);
            }

            lastOrderId = viewRecord.orderID;
        }

        // Zwróć ostatni obiekt Complete, jeśli istnieje
        if (currentComplete != null)
        {
            yield return currentComplete;
        }

        ;

        Complete viewrecord2COmplete(completeview viewRecord)
        {
            return new Complete
            {
                Order = new order
                {
                    orderID = viewRecord.orderID,
                    VAT = viewRecord.VAT,
                    saletotal = viewRecord.saletotal,
                    status = viewRecord.status,
                    acquiredcurrency = viewRecord.acquiredcurrency,
                    salecurrency = viewRecord.salecurrency,
                    xchgrate = viewRecord.xchgrate,
                    customerID = viewRecord.customerID,
                    dispatchedOn = viewRecord.dispatchedOn,
                    locationID = viewRecord.locationID,
                    paidOn = viewRecord.paidOn,
                    VATRateID = viewRecord.VATRateID,
                    quickbooked = viewRecord.quickbooked,
                    tracking = viewRecord.tracking,
                    order_notes = viewRecord.order_notes,
                    postagePrice = viewRecord.postagePrice,
                    postageType = viewRecord.postageType,
                    market = viewRecord.market
                },
                Customer = new customer
                {
                    customerID = viewRecord.customerID,
                    Title = viewRecord.Title,
                    GivenName = viewRecord.GivenName,
                    MiddleName = viewRecord.MiddleName,
                    FamilyName = viewRecord.FamilyName,
                    CompanyName = viewRecord.CompanyName,
                    Email = viewRecord.Email,
                    Phone = viewRecord.Phone,
                    DisplayName = viewRecord.DisplayName,
                    currency = viewRecord.customer_currency,
                    customer_notes = viewRecord.customer_notes,
                    billaddrID = viewRecord.billaddrID
                },
                BillAddr = new billaddr
                {
                    billaddrID = (int)viewRecord.billaddrID,
                    Line1 = viewRecord.Line1,
                    Line2 = viewRecord.Line2,
                    City = viewRecord.City,
                    CountryCode = viewRecord.CountryCode,
                    CountrySubDivisionCode = viewRecord.CountrySubDivisionCode,
                    PostalCode = viewRecord.PostalCode,
                    AddressAsAString = viewRecord.AddressAsAString
                },
                OrderItems = new()
            };
        }
    }

    public async Task<List<Complete>> GetKomplety(List<int> ids)
    {
        var komplety = new List<Complete>();

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<completeview>(unitOfWork);

            foreach (var komp in await service.GetAllAsync(p => ids.Contains(p.orderID)).ConfigureAwait(false))
            {
                Complete komplet;
                if (!komplety.Any(p => p.Order.orderID == komp.orderID))
                {
                    komplet = new Complete
                    {
                        Order = new(),
                        Customer = new(),
                        BillAddr = new(),
                        OrderItems = new()
                    };
                    komplet.Order.orderID = komp.orderID;
                    komplet.Order.VAT = komp.VAT;
                    komplet.Order.saletotal = komp.saletotal;
                    komplet.Order.status = komp.status;
                    komplet.Order.xchgrate = komp.xchgrate;
                    komplet.Order.acquiredcurrency = komp.acquiredcurrency;
                    komplet.Order.salecurrency = komp.salecurrency;
                    komplet.Order.customerID = komp.customerID;
                    komplet.Order.dispatchedOn = komp.dispatchedOn;
                    komplet.Order.locationID = komp.locationID;
                    komplet.Order.paidOn = komp.paidOn;
                    komplet.Order.VATRateID = komp.VATRateID;
                    komplet.Order.quickbooked = komp.quickbooked;
                    komplet.Order.tracking = komp.tracking;
                    komplet.Order.order_notes = komp.order_notes;
                    komplet.Order.postagePrice = komp.postagePrice;
                    komplet.Order.postageType = komp.postageType;
                    komplet.Order.market = komp.market;
                    komplet.Customer.customerID = komp.customerID;
                    komplet.Customer.Title = komp.Title;
                    komplet.Customer.GivenName = komp.GivenName;
                    komplet.Customer.MiddleName = komp.MiddleName;
                    komplet.Customer.FamilyName = komp.FamilyName;
                    komplet.Customer.CompanyName = komp.CompanyName;
                    komplet.Customer.Email = komp.Email;
                    komplet.Customer.Phone = komp.Phone;
                    komplet.Customer.DisplayName = komp.DisplayName;
                    komplet.Customer.currency = komp.customer_currency;
                    komplet.Customer.customer_notes = komp.customer_notes;
                    komplet.Customer.billaddrID = komp.billaddrID;
                    komplet.BillAddr.billaddrID = (int)komp.billaddrID;
                    komplet.BillAddr.Line1 = komp.Line1;
                    komplet.BillAddr.Line2 = komp.Line2;
                    komplet.BillAddr.City = komp.City;
                    komplet.BillAddr.CountryCode = komp.CountryCode;
                    komplet.BillAddr.CountrySubDivisionCode = komp.CountrySubDivisionCode;
                    komplet.BillAddr.PostalCode = komp.PostalCode;
                    komplet.BillAddr.AddressAsAString = komp.AddressAsAString;
                    komplety.Add(komplet);
                }
                else
                {
                    komplet = komplety.First(p => p.Order.orderID == komp.orderID);
                }

                if (komp.OrderItemId != null)
                {
                    var oi = new orderitem
                    {
                        OrderItemId = (int)komp.OrderItemId,
                        itemName = komp.itemName,
                        quantity = (int)komp.quantity,
                        itembodyID = (int)komp.itembodyID,
                        OrderItemTypeId = (int)komp.OrderItemTypeId,
                        price = (decimal)komp.price,
                        orderID = komp.orderID,
                        itmMarketAssID = komp.itmMarketAssID,
                        ItemWeight = komp.ItemWeight
                    };
                    komplet.OrderItems.Add(oi);
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return komplety;
    }

    public async Task<Answer> GetListOfItemParameters(int itembodyid)
    {
        var answer = Answer.Prepare($"Getting list of parameters for item {itembodyid}");
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var itemitsparametersandvaluesService = new EntityService<itemitsparametersandvalue>(unitOfWork);
            answer.AddValue((await itemitsparametersandvaluesService.GetAllAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false)).ToDictionary(p => p.ParameterName, q => q.ParameterValueName));
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return answer.Error("Error while reading DB");
        }

        return answer;
    }

    public async Task<Complete> GetKomplet(int orderid)
    {
        var komplet = new Complete
        {
            Order = new(),
            Customer = new(),
            BillAddr = new(),
            OrderItems = new()
        };
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<completeview>(unitOfWork);
            var komplety = await service.GetAllAsync(p => p.orderID == orderid).ConfigureAwait(false);
            komplet.Order.orderID = komplety.First().orderID;
            komplet.Order.VAT = komplety.First().VAT;
            komplet.Order.xchgrate = komplety.First().xchgrate;
            komplet.Order.saletotal = komplety.First().saletotal;
            komplet.Order.status = komplety.First().status;
            komplet.Order.acquiredcurrency = komplety.First().acquiredcurrency;
            komplet.Order.salecurrency = komplety.First().salecurrency;
            komplet.Order.customerID = komplety.First().customerID;
            komplet.Order.dispatchedOn = komplety.First().dispatchedOn;
            komplet.Order.locationID = komplety.First().locationID;
            komplet.Order.paidOn = komplety.First().paidOn;
            komplet.Order.VATRateID = komplety.First().VATRateID;
            komplet.Order.quickbooked = komplety.First().quickbooked;
            komplet.Order.tracking = komplety.First().tracking;
            komplet.Order.order_notes = komplety.First().order_notes;
            komplet.Order.postagePrice = komplety.First().postagePrice;
            komplet.Order.postageType = komplety.First().postageType;
            komplet.Order.market = komplety.First().market;
            komplet.Customer.customerID = komplety.First().customerID;
            komplet.Customer.Title = komplety.First().Title;
            komplet.Customer.GivenName = komplety.First().GivenName;
            komplet.Customer.MiddleName = komplety.First().MiddleName;
            komplet.Customer.FamilyName = komplety.First().FamilyName;
            komplet.Customer.CompanyName = komplety.First().CompanyName;
            komplet.Customer.Email = komplety.First().Email;
            komplet.Customer.Phone = komplety.First().Phone;
            komplet.Customer.DisplayName = komplety.First().DisplayName;
            komplet.Customer.currency = komplety.First().customer_currency;
            komplet.Customer.customer_notes = komplety.First().customer_notes;
            komplet.Customer.billaddrID = komplety.First().billaddrID;
            komplet.BillAddr.Line1 = komplety.First().Line1;
            komplet.BillAddr.Line2 = komplety.First().Line2;
            komplet.BillAddr.City = komplety.First().City;
            komplet.BillAddr.CountryCode = komplety.First().CountryCode;
            komplet.BillAddr.CountrySubDivisionCode = komplety.First().CountrySubDivisionCode;
            komplet.BillAddr.PostalCode = komplety.First().PostalCode;
            komplet.BillAddr.AddressAsAString = komplety.First().AddressAsAString;
            foreach (var item in komplety)
            {
                var oi = new orderitem
                {
                    OrderItemId = (int)item.OrderItemId,
                    itemName = item.itemName,
                    quantity = (int)item.quantity,
                    itembodyID = (int)item.itembodyID,
                    OrderItemTypeId = (int)item.OrderItemTypeId,
                    price = (decimal)item.price,
                    orderID = item.orderID,
                    itmMarketAssID = item.itmMarketAssID,
                    ItemWeight = item.ItemWeight
                };
                komplet.OrderItems.Add(oi);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return komplet;
    }

    public async Task<List<xrate>> GetXratesRange(DateTime min, DateTime max, List<string> currencies)
    {
        var zwrotka = new List<xrate>();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());

            var service = new EntityService<xrate>(unitOfWork);
            foreach (var currency in currencies)
            {
                for (var date = min; date <= max; date = date.AddDays(1))
                {
                    var xrate = (await service.GetAllAsync(x => x.code == currency && x.date <= date).ConfigureAwait(false)).OrderByDescending(x => x.date).FirstOrDefault();
                    if (xrate != null)
                    {
                        // Utwórz nowy obiekt xrate z oryginalnymi danymi, ale zaktualizowaną datą.
                        var newXrate = new xrate
                        {
                            XrateId = xrate.XrateId,
                            date = date,
                            rate = xrate.rate,
                            code = xrate.code
                        };
                        zwrotka.Add(newXrate);
                    }
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return zwrotka;
    }

    public async Task RemoveComplete(int orderid)
    {
        var zwrotka = new List<order>();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var orderService = new orderService(unitOfWork);
            var billAddrService = new EntityService<billaddr>(unitOfWork);
            var CustomerService = new EntityService<customer>(unitOfWork);
            var orderitemervice = new EntityService<orderitem>(unitOfWork);

            var oi = await orderitemervice.GetAllAsync(p => p.orderID == orderid).ConfigureAwait(false);
            await orderitemervice.DeleteRangeAsync(oi).ConfigureAwait(false);
            var ord = await orderService.GetOneAsync(p => p.orderID == orderid);
            int customerid = ord.customerID;
            await orderService.DeleteAsync(ord).ConfigureAwait(false);
            var cust = await CustomerService.GetOneAsync(p => p.customerID == customerid).ConfigureAwait(false);
            var billaddrid = cust.billaddrID;
            await CustomerService.DeleteAsync(cust).ConfigureAwait(false);
            var billaddr = await billAddrService.GetOneAsync(p => p.billaddrID == billaddrid).ConfigureAwait(false);
            await billAddrService.DeleteAsync(billaddr).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));
    }

    public async Task<Complete> AddComplete(order order, customer customer, billaddr billaddr, List<orderitem> orderitem)
    {
        Complete komplet = new();
        var zwrotka = new List<order>();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var orderService = new orderService(unitOfWork);
            var billAddrService = new EntityService<billaddr>(unitOfWork);
            var CustomerService = new EntityService<customer>(unitOfWork);
            var orderitemervice = new EntityService<orderitem>(unitOfWork);

            //            var existingBillAddr = await billAddrService.GetOneAsync(p =>
            //p.Line1 == billaddr.Line1 &&
            //p.Line2 == billaddr.Line2 &&
            //p.PostalCode == billaddr.PostalCode &&
            //p.City == billaddr.City &&
            //p.CountryCode == billaddr.CountryCode)
            //.ConfigureAwait(false);
            //            if (existingBillAddr != null)
            //            {
            //                billaddr.billaddrID = existingBillAddr.billaddrID;
            //            }
            //            else
            //            {
            await billAddrService.AddAsync(billaddr).ConfigureAwait(false);
            //}

            customer.billaddrID = billaddr.billaddrID;

            //             var existingCustomer = await CustomerService.GetOneAsync(c =>
            // c.GivenName == customer.GivenName &&
            // c.FamilyName == customer.FamilyName &&
            //// c.billaddrID == customer.billaddrID &&
            // c.Email == customer.Email)
            // .ConfigureAwait(false);

            //             if (existingCustomer != null)
            //             {
            //                 customer.customerID = existingCustomer.customerID;
            //             }
            //             else
            //             {
            await CustomerService.AddAsync(customer).ConfigureAwait(false);
            //          }

            //    customer.billaddrID = billaddr.billaddrID;
            komplet.BillAddr = billaddr;
            komplet.Customer = customer;
            order.customerID = customer.customerID;

            await orderService.AddAsync(order).ConfigureAwait(false);
            komplet.Order = order;
            foreach (var item in orderitem)
            {
                item.orderID = order.orderID;
            }
            await orderitemervice.AddRangeAsync(orderitem).ConfigureAwait(false);
            komplet.OrderItems = [.. orderitem];
        }, TimeSpan.FromSeconds(10));

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return komplet;
    }

    public async Task<List<Complete>> GetCurrentAndPreviousDaysKomplety()
    {
        var komplety = new List<Complete>();

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<completeview>(unitOfWork);
            foreach (var komp in await service.GetAllAsync(o =>
                         (o.paidOn.Year == DateTime.Now.Year && o.paidOn.Month == DateTime.Now.Month || o.paidOn.Date == DateTime.Now.AddDays(-1).Date)
                         && (o.status.Equals("_NEW") || o.status.Equals("PROC") || o.status.Equals("SHIP") || o.status.Equals("_PIC"))
                     ).ConfigureAwait(false))
            {
                Complete komplet;
                if (!komplety.Any(p => p.Order.orderID == komp.orderID))
                {
                    komplet = new Complete
                    {
                        Order = new(),
                        Customer = new(),
                        BillAddr = new(),
                        OrderItems = new()
                    };
                    komplet.Order.orderID = komp.orderID;
                    komplet.Order.VAT = komp.VAT;
                    komplet.Order.saletotal = komp.saletotal;
                    komplet.Order.status = komp.status;
                    komplet.Order.xchgrate = komp.xchgrate;
                    komplet.Order.acquiredcurrency = komp.acquiredcurrency;
                    komplet.Order.salecurrency = komp.salecurrency;
                    komplet.Order.customerID = komp.customerID;
                    komplet.Order.dispatchedOn = komp.dispatchedOn;
                    komplet.Order.locationID = komp.locationID;
                    komplet.Order.paidOn = komp.paidOn;
                    komplet.Order.VATRateID = komp.VATRateID;
                    komplet.Order.quickbooked = komp.quickbooked;
                    komplet.Order.tracking = komp.tracking;
                    komplet.Order.order_notes = komp.order_notes;
                    komplet.Order.postagePrice = komp.postagePrice;
                    komplet.Order.postageType = komp.postageType;
                    komplet.Order.market = komp.market;
                    komplet.Customer.customerID = komp.customerID;
                    komplet.Customer.Title = komp.Title;
                    komplet.Customer.GivenName = komp.GivenName;
                    komplet.Customer.MiddleName = komp.MiddleName;
                    komplet.Customer.FamilyName = komp.FamilyName;
                    komplet.Customer.CompanyName = komp.CompanyName;
                    komplet.Customer.Email = komp.Email;
                    komplet.Customer.Phone = komp.Phone;
                    komplet.Customer.DisplayName = komp.DisplayName;
                    komplet.Customer.currency = komp.customer_currency;
                    komplet.Customer.customer_notes = komp.customer_notes;
                    komplet.Customer.billaddrID = komp.billaddrID;
                    komplet.BillAddr.Line1 = komp.Line1;
                    komplet.BillAddr.Line2 = komp.Line2;
                    komplet.BillAddr.City = komp.City;
                    komplet.BillAddr.CountryCode = komp.CountryCode;
                    komplet.BillAddr.CountrySubDivisionCode = komp.CountrySubDivisionCode;
                    komplet.BillAddr.PostalCode = komp.PostalCode;
                    komplet.BillAddr.AddressAsAString = komp.AddressAsAString;
                    komplety.Add(komplet);
                }
                else
                {
                    komplet = komplety.First(p => p.Order.orderID == komp.orderID);
                }

                if (komp.OrderItemId != null)
                {
                    var oi = new orderitem
                    {
                        OrderItemId = (int)komp.OrderItemId,
                        itemName = komp.itemName,
                        quantity = (int)komp.quantity,
                        itembodyID = (int)komp.itembodyID,
                        OrderItemTypeId = (int)komp.OrderItemTypeId,
                        price = (decimal)komp.price,
                        orderID = komp.orderID,
                        itmMarketAssID = komp.itmMarketAssID,
                        ItemWeight = komp.ItemWeight
                    };
                    komplet.OrderItems.Add(oi);
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return komplety;
    }

    public async Task<Answer> FlipQBInvoiceIdinInvoiceTX(int invoiceTXNID, string? quickbooksinvoiceid = null)
    {
        var answer = Answer.Prepare($"Flipping QB invoice id in invoiceTXN {invoiceTXNID}");
        invoicetxn returnValue = new();

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<invoicetxn>(unitOfWork);
            var invtxnResponse = await service.GetByIdAsync(invoiceTXNID).ConfigureAwait(false);
            if (!invtxnResponse.IsSuccess)
            {
                return;
            }
            var invtxn = invtxnResponse.GetValue<invoicetxn>();
            invtxn.qbInvoiceId = quickbooksinvoiceid;
            await service.UpdateAsync(invtxn).ConfigureAwait(false);
            returnValue = invtxn;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return answer.WithValue(returnValue);
    }

    public async Task<Dictionary<string, invoicetxn>> PlatformTXN2InvoiceTXN(List<string> platformTXNs)
    {
        Dictionary<string, invoicetxn> zwrotka = new Dictionary<string, invoicetxn>();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var service = new EntityService<invoicetxn>(unitOfWork);
            zwrotka = (await service.GetAllAsync(p => platformTXNs.Contains(p.platformTXN)).ConfigureAwait(false)).ToDictionary(p => p.platformTXN, q => q);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return zwrotka;
    }

    public async Task<List<order>> UpdateorderStatusesAndTrackings(Dictionary<int, (string Status, string Tracking)> orderUpdates)
    {
        var zwrotka = new List<order>();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var orderService = new orderService(unitOfWork);
            var orders = (await orderService.GetAllAsync(o => orderUpdates.Keys.Contains(o.orderID)).ConfigureAwait(false)).ToList();
            foreach (var ord in orders)
            {
                if (orderUpdates.TryGetValue(ord.orderID, out var updateInfo))
                {
                    ord.tracking = updateInfo.Tracking;
                    ord.status = updateInfo.Status;
                    ord.dispatchedOn = DateTime.Now;
                }
            }
            await orderService.UpdateRangeAsync(orders).ConfigureAwait(false);
            zwrotka = orders.ToList();
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return zwrotka;
    }

    public async Task AddBodies2Group(List<int> itembodies, int group4bodiesID)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<bodiesgrouped>(unitOfWork);
            List<bodiesgrouped> bodiesGroupedList = new List<bodiesgrouped>();
            foreach (var itemBodyID in itembodies)
            {
                bodiesGroupedList.Add(new bodiesgrouped
                {
                    group4bodiesID = group4bodiesID,
                    itemBodyID = itemBodyID
                });
            }
            await service.AddRangeAsync(bodiesGroupedList).ConfigureAwait(false);
            if (_lazyBodiesGrouped.IsValueCreated)
            {
                var existing = await _lazyBodiesGrouped.Value.ConfigureAwait(false);
                existing.AddRange(bodiesGroupedList);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task<bodyinthebox> AddBodyInTheBox(bodyinthebox bodyinthebox)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<bodyinthebox>(unitOfWork);
            await service.AddAsync(bodyinthebox).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        items[bodyinthebox.itembodyID].bodyinthebox = bodyinthebox;
        BodyInTheBoxes.Add(bodyinthebox);
        return bodyinthebox;
    }

    public async Task AddCasioInvoices(List<casioinvoice> lista)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<casioinvoice>(unitOfWork);
            await service.AddRangeAsync(lista).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task<casioukcurrentorder> AddCasioUKCurOrd(casioukcurrentorder cuo)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<casioukcurrentorder>(unitOfWork);
            await service.AddAsync(cuo).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return cuo;
    }

    public async Task<parameter> AddCecha(string name)
    {
        parameter zwrotka = new();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            var cecha = new parameter { name = name };
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<parameter>(unitOfWork);
            await service.AddAsync(cecha).ConfigureAwait(false);
            zwrotka = cecha;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result == DatabaseOperationExecutor.DatabaseOperationResult.Error) return zwrotka;
        if (_lazyCechy.IsValueCreated)
        {
            var existingCechies = await _lazyCechy.Value.ConfigureAwait(false);
            existingCechies.Add(zwrotka.parameterID, zwrotka);
            var cvs = await cechyValues().ConfigureAwait(false);
            cvs.Add(zwrotka.parameterID, new List<parametervalue>());
        }
        if (_lazyCechyValues.IsValueCreated)
        {
            Dictionary<int, List<parametervalue>> excv = await _lazyCechyValues.Value.ConfigureAwait(false);
            excv[zwrotka.parameterID] = new List<parametervalue>();
        }

        return zwrotka;
    }

    public async Task<parametervalue> AddCechaValue(int parameterID, string name)
    {
        parametervalue zwrotka = new parametervalue();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            int maxPos = 0;

            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<parametervalue>(unitOfWork);

            var maxPosCV = await service.GetOneAsync(c => c.parameterID == parameterID, c => c.pos);
            if (maxPosCV != null)
            {
                maxPos = maxPosCV.pos + 1;
            }
            var cechaV = new parametervalue
            {
                parameterID = parameterID,
                name = name,
                pos = maxPos
            };
            await service.AddAsync(cechaV).ConfigureAwait(false);
            zwrotka = cechaV;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result == DatabaseOperationExecutor.DatabaseOperationResult.Error)
        {
            if (_lazyCechyValues.IsValueCreated)
            {
                _lazyCechyValues = CreateAsyncLazy(LoadCechyValuesAsync);
            }
        }
        return zwrotka;
    }

    public async Task<ItemBodyDBOperationStatus> AddFreshBody(itembody cialko, itemheader header)
    {
        ItemBodyDBOperationStatus zwrotka = ItemBodyDBOperationStatus.OperationSuccessful;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var ItemBodiesService = new EntityService<itembody>(unitOfWork);
            var ItemHeaderService = new EntityService<itemheader>(unitOfWork);
            if (ItemBodiesService.Any(p => p.name.Equals(cialko.name)))
            {
                zwrotka = ItemBodyDBOperationStatus.ObjectWithNameExists;
            }
            if (ItemBodiesService.Any(p => p.name.Equals(cialko.myname)))
            {
                zwrotka = ItemBodyDBOperationStatus.ObjectWithShortNameExists;
            }
            if (ItemBodiesService.Any(p => p.mpn.Equals(cialko.mpn)))
            {
                zwrotka = ItemBodyDBOperationStatus.ObjectWithMpnExists;
            }
            if (zwrotka == ItemBodyDBOperationStatus.OperationSuccessful)
            {
                cialko.readyTotrack = true;
                await ItemBodiesService.AddAsync(cialko).ConfigureAwait(false);
                header.itembodyID = cialko.itembodyID;
                await ItemHeaderService.AddAsync(header).ConfigureAwait(false);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (DALState == DatabaseAccessLayerState.Loaded)
        {
            items.Add(cialko.itembodyID, new AssociatedData { itembody = cialko });
            items[cialko.itembodyID].ItemHeaders.Add(header);
            OnItemBodiesChanged(new List<int> { header.itembodyID });
        }
        return zwrotka;
    }

    public async Task AddSupplier(string name)
    {
        group4body zwrotka = new group4body();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<supplier>(unitOfWork);
            var sup = new supplier { name = name };
            await service.AddAsync(sup).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task<type> AddType(string name)
    {
        type typek = new();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<type>(unitOfWork);
            typek = new type { name = name };
            await service.AddAsync(typek).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        if (_lazyTypes.IsValueCreated)
        {
            var existingTypes = await _lazyTypes.Value.ConfigureAwait(false);
            existingTypes.Add(typek.typeID, name);
        }

        await _entityEventsService.PublishAsync<type>(new EntityEventArgs([typek.typeID], EntityActionType.Add));

        return typek;
    }

    public async Task<group4body> AddGroup4Body(string name)
    {
        var zwrotka = new group4body();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<group4body>(unitOfWork);
            var g4b = new group4body() { name = name };
            await service.AddAsync(g4b).ConfigureAwait(false);
            zwrotka = g4b;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        if (_lazyGroup4Bodies.IsValueCreated)
        {
            var existingG4W = await _lazyGroup4Bodies.Value.ConfigureAwait(false);
            existingG4W.Add(zwrotka);
        }
        return zwrotka;
    }

    public async Task<group4watch> AddGroup4watch(string name)
    {
        var zwrotka = new group4watch();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<group4watch>(unitOfWork);
            var g4w = new group4watch { name = name };
            await service.AddAsync(g4w).ConfigureAwait(false);
            zwrotka = g4w;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        if (_lazyGroup4Watches.IsValueCreated)
        {
            var existingG4W = await _lazyGroup4Watches.Value.ConfigureAwait(false);
            existingG4W.Add(zwrotka);
        }
        return zwrotka;
    }

    public async Task AddLogEvent(string loggedText, int marketID, int itembodyId, int? itemheaderId = null)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<logevent>(unitOfWork);
            var le = new logevent
            {
                happenedOn = DateTime.Now,
                _event = loggedText,
                marketID = marketID,
                itemBodyID = itembodyId
            };
            if (itembodyId != null)
            {
                le.itemHeaderID = itemheaderId;
            }
            await service.AddAsync(le).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task<mayalsofit> AddMayAlsoFit(int group4bodiesID, int group4watchesID)
    {
        mayalsofit zwrotka = new();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<mayalsofit>(unitOfWork);
            var maf = new mayalsofit { group4bodiesID = group4bodiesID, group4watchesID = group4watchesID };
            await service.AddAsync(maf).ConfigureAwait(false);
            zwrotka = maf;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (_lazyMayalsofits.IsValueCreated)
        {
            var existingMAF = await _lazyMayalsofits.Value.ConfigureAwait(false);
            existingMAF.Add(zwrotka);
        }
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return zwrotka;
    }

    public async Task<multidrawer> AddMultiDrawer(multidrawer multidrawer)
    {
        multidrawer zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<multidrawer>(unitOfWork);
            await service.AddAsync(multidrawer).ConfigureAwait(false);
            zwrotka = multidrawer;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        if (_lazyMultiDrawer.IsValueCreated)
        {
            var existingMultiDrawer = await _lazyMultiDrawer.Value.ConfigureAwait(false);
            existingMultiDrawer.Add(multidrawer);
        }
        return zwrotka;
    }

    public async Task AddWatch2Group(int watchid, int group4watchesID)
    {
        var wa = new watchesgrouped();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var service = new EntityService<watchesgrouped>(unitOfWork);
            wa.watchID = watchid;
            wa.group4watchesID = group4watchesID;
            await service.AddAsync(wa).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (_lazyWatchesGrouped.IsValueCreated)
        {
            var existing = await _lazyWatchesGrouped.Value.ConfigureAwait(false);
            existing.Add(wa);
        }
    }

    public async Task<List<orderData>> GetOrderDataAsync(string currency, Tuple<DateTime, DateTime> period, HashSet<string> countries, int location)
    {
        List<orderData> orderDataList = new();

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var cvService = new CompleteViewService(unitOfWork);
            var vatrateService = new EntityService<vatrate>(unitOfWork);
            var vaty = (await vatrateService.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.VATRateID,
                q => 1 + q.Rate / 100);
            var orderserv = new orderService(unitOfWork);
            var ordeki = (await orderserv.GetAllAsync(p => p.paidOn >= period.Item1 && p.paidOn <= period.Item2 && p.locationID == 1).ConfigureAwait(false)).ToList();
            var ordki = await cvService.GetDistinctCompletesWithinTimeRange(period.Item1, period.Item2, countries, location);
            foreach (var cv in ordki)
            {
                var orderData = new orderData();
                orderData.orderId = cv.orderID;
                orderData.Total = cv.saletotal * cv.xchgrate;
                orderData.IsVat = cv.VAT;
                if (!cv.VAT)
                {
                    orderData.NetTotal = cv.saletotal / vaty[cv.VATRateID];
                }
                else
                {
                    orderData.NetTotal = cv.saletotal;
                }
                orderData.MarketId = cv.market;
                orderData.PaidOn = cv.paidOn;
                orderData.CountryCode = cv.CountryCode;
                orderDataList.Add(orderData);  // Dodanie wyników do listy
            }
        }, TimeSpan.FromSeconds(20)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return orderDataList;
    }

    public async Task<Dictionary<itembody, itemheader>> AddNewBodz(List<Body2Add> bodiez, int locationID)
    {
        var zwrotka = new Dictionary<itembody, itemheader>();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var ItemBodiesService = new EntityService<itembody>(unitOfWork);
            var ItemHeadersService = new EntityService<itemheader>(unitOfWork);
            List<itembody> bodiesToAdd = new List<itembody>();
            List<itemheader> headersToAdd = new List<itemheader>();
            foreach (var item in bodiez)
            {
                var ib = item.body;
                bodiesToAdd.Add(ib);
            }
            await ItemBodiesService.AddRangeAsync(bodiesToAdd).ConfigureAwait(false);
            foreach (var item in bodiesToAdd)
            {
                var bodz = bodiez.First(p => p.body.mpn.Equals(item.mpn));
                var s = new itemheader
                {
                    itembodyID = item.itembodyID,
                    locationID = locationID,
                    pricePaid = bodz.price,
                    supplierID = 1,
                    purchasedOn = bodz.when,
                    quantity = bodz.quantity,
                    xchgrate = bodz.xchgrate,
                    VATRateID = bodz.vatrate,
                    acquiredcurrency = bodz.acquiredcurrency,
                    purchasecurrency = bodz.purchasecurrency
                };
                headersToAdd.Add(s);
            }
            await ItemHeadersService.AddRangeAsync(headersToAdd).ConfigureAwait(false);
            for (int i = 0; i < bodiesToAdd.Count; i++)
            {
                zwrotka.Add(bodiesToAdd[i], headersToAdd[i]);
            }
        }, TimeSpan.FromSeconds(10));

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        await RefreshBodies(zwrotka.Keys.Select(p => p.itembodyID).ToList()).ConfigureAwait(false);
        return zwrotka;
    }

    public async Task<Dictionary<int, string>> GetMarkets2PlatformTypesDictionary()
    {
        Dictionary<int, string> zwrotka = new();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var platformService = new EntityService<platform>(unitOfWork);
            var marketService = new EntityService<market>(unitOfWork);
            var marketPlatformAssociationService = new EntityService<marketplatformassociation>(unitOfWork);
            var markets = (await marketService.GetAllAsync(p => p.IsInUse).ConfigureAwait(false)).ToList();
            var platforms = (await platformService.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.platformID, q => q.name);
            var m2P = (await marketPlatformAssociationService.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.marketID, q => q.platformID);
            foreach (var market in markets)
            {
                zwrotka.Add(market.marketID, platforms[m2P[market.marketID]]);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return zwrotka;
    }

    public async Task<itemheader> AddNewItemHeader(int supplierId, decimal pricePaid, int itemBodyId, int quantity, int locationID, int xchgrate, int vatrateid, string acquiredcurrency, string purchasecurrency)
    {
        itemheader itm = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var ItemLogEventService = new EntityService<logevent>(unitOfWork);
            var ItemHeadersService = new EntityService<itemheader>(unitOfWork);
            itm = new itemheader
            {
                itembodyID = itemBodyId,
                locationID = locationID,
                pricePaid = pricePaid,
                purchasedOn = DateTime.Now,
                quantity = quantity,
                supplierID = supplierId,
                xchgrate = xchgrate,
                VATRateID = vatrateid,
                acquiredcurrency = acquiredcurrency,
                purchasecurrency = purchasecurrency
            };
            await ItemHeadersService.AddAsync(itm).ConfigureAwait(false);
            var le = new logevent
            {
                happenedOn = DateTime.Now,
                itemBodyID = itm.itembodyID,
                _event = "created itemheader with quantity " + quantity,
                itemHeaderID = itm.itemheaderID
            };
            await ItemLogEventService.AddAsync(le).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        if (DALState == DatabaseAccessLayerState.Loaded)
        {
            items[itm.itembodyID].ItemHeaders.Add(itm);
        }
        OnItemBodiesChanged(new List<int> { itm.itembodyID });
        return itm;
    }

    public async Task AddNewMarket(FullItmMarketAss itema)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var itmMarketAssocService = new EntityService<itmmarketassoc>(unitOfWork);
            var PlatformService = new EntityService<platform>(unitOfWork);
            var AsinSkuService = new EntityService<asinsku>(unitOfWork);
            var MarketPlatformAssociationService = new EntityService<marketplatformassociation>(unitOfWork);
            itmmarketassoc gu = new()
            {
                SEName = itema.itmmarketassoc.SEName,
                quantitySold = itema.itmmarketassoc.quantitySold,
                itemNumber = itema.itmmarketassoc.itemNumber,
                locationID = 1,
                marketID = itema.itmmarketassoc.marketID,
                itembodyID = itema.itmmarketassoc.itembodyID
            };
            await itmMarketAssocService.AddAsync(gu).ConfigureAwait(false);
            items[itema.itmmarketassoc.itembodyID].ItmMarketAssocs.Add(gu);
            int platform = (await MarketPlatformAssociationService.GetOneAsync(p => p.marketID == itema.itmmarketassoc.marketID).ConfigureAwait(false)).platformID;
            string platformName = (await PlatformService.GetByIdAsync(platform).ConfigureAwait(false)).GetValue<platform>().name;
            if (platformName.ToLower().Contains("amazon"))
            {
                var gus = new asinsku
                {
                    asin = itema.itmmarketassoc.itemNumber,
                    sku = itema.SKU.sku,
                    locationID = itema.SKU.locationID
                };
                await AsinSkuService.AddAsync(gus).ConfigureAwait(false);

                if (_lazyASINSKUS.IsValueCreated)
                {
                    var existingAsinSkus = await _lazyASINSKUS.Value.ConfigureAwait(false);
                    existingAsinSkus.Add(gus);
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return;
        }
    }

    public async Task<token> GetToken(int locationId)
    {
        var zwrotka = new token();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var TokenService = new EntityService<token>(unitOfWork);
            zwrotka = await TokenService.GetOneAsync(p => p.locationID == locationId).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        return zwrotka;
    }

    public async Task<asinsku> AddAsinSku(asinsku asssk)
    {
        asinsku zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var AsinSkuService = new EntityService<asinsku>(unitOfWork);
            var asku = new asinsku
            {
                asin = asssk.asin = asssk.asin,
                sku = asssk.sku,
                locationID = asssk.locationID
            };
            await AsinSkuService.AddAsync(asku).ConfigureAwait(false);
            zwrotka = asssk;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        if (_lazyASINSKUS.IsValueCreated)
        {
            var existingas = await _lazyASINSKUS.Value.ConfigureAwait(false);
            existingas.Add(zwrotka);
        }
        return zwrotka;
    }

    public async Task<colourtranslation> AddOrUpdateColourTranslation(colourtranslation ct)
    {
        colourtranslation zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var ColourTranslationService = new EntityService<colourtranslation>(unitOfWork);

            if (ct.Id == 0)
            {
                await ColourTranslationService.AddAsync(ct).ConfigureAwait(false);
                if (_lazyColourTranslations.IsValueCreated)
                {
                    var existingolourTranslations = await _lazyColourTranslations.Value.ConfigureAwait(false);
                    existingolourTranslations[ct.kodKoloru] = ct;
                }

                zwrotka = ct;
            }
            else
            {
                var cct = await ColourTranslationService.GetOneAsync(p => p.Id == ct.Id).ConfigureAwait(false);
                cct.schemat = ct.schemat;
                cct.kodKoloru = ct.kodKoloru;
                cct.col1 = ct.col1;
                cct.col2 = ct.col2;
                cct.col3 = ct.col3;
                cct.col4 = ct.col4;
                await ColourTranslationService.UpdateAsync(cct).ConfigureAwait(false);
                await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                if (_lazyColourTranslations.IsValueCreated)
                {
                    var existingolourTranslations = await _lazyColourTranslations.Value.ConfigureAwait(false);
                    existingolourTranslations[ct.kodKoloru] = cct;
                }
                zwrotka = cct;
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return zwrotka;
    }

    public async Task<typeparassociation> AddTypeParameterRelation(int typeid, int parameterID)
    {
        var zwrotka = new typeparassociation();

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            int maxPos = 0;

            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var typeParAssociationService = new EntityService<typeparassociation>(unitOfWork);
            var typep2 = await typeParAssociationService.GetAllAsync(p => p.typeID == typeid).ConfigureAwait(false);

            maxPos = typep2.Count();// (await typeParAssociationService.MaxAsync(c => c.typeID == typeid, c => c.pos).ConfigureAwait(false));

            var tp = new typeparassociation
            {
                typeID = typeid,
                parameterID = parameterID,
                pos = maxPos
            };
            await typeParAssociationService.AddAsync(tp).ConfigureAwait(false);
            zwrotka = tp;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (result == DatabaseOperationExecutor.DatabaseOperationResult.Error)
        {
            if (_lazyTypePars.IsValueCreated)
            {
                var existingTypePars = await _lazyTypePars.Value.ConfigureAwait(false);
                if (existingTypePars.ContainsKey(typeid))
                {
                    existingTypePars[typeid].Add(zwrotka);
                }
                else
                {
                    existingTypePars.Add(typeid, new List<typeparassociation> { zwrotka });
                }
            }
            await _entityEventsService.PublishAsync<type>(new EntityEventArgs([typeid], EntityActionType.Update));
        }

        return zwrotka;
    }

    public async Task<ItemBodyUpdateOperationResult> CheckIfSuchBodyCanBeAdded(string name, string myname, string mpn)
    {
        ItemBodyUpdateOperationResult status = new ItemBodyUpdateOperationResult
        {
            Status = ItemBodyDBOperationStatus.OperationSuccessful,
            itembody = null
        };

        await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var itemBodyService = new EntityService<itembody>(unitOfWork);

            var res = await itemBodyService.GetOneAsync(p => p.name.Equals(name)).ConfigureAwait(false);
            if (res != null)
            {
                status.Status = ItemBodyDBOperationStatus.ObjectWithNameExists;
                status.itembody = res;
            }

            res = await itemBodyService.GetOneAsync(p => p.name.Equals(myname)).ConfigureAwait(false);
            if (res != null)
            {
                status.Status = ItemBodyDBOperationStatus.ObjectWithShortNameExists;
                status.itembody = res;
            }

            res = await itemBodyService.GetOneAsync(p => p.name.Equals(mpn)).ConfigureAwait(false);
            if (res != null)
            {
                status.Status = ItemBodyDBOperationStatus.ObjectWithMpnExists;
                status.itembody = res;
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (status.Status == (ItemBodyDBOperationStatus)(-2)) // Assuming -2 is a valid enum value or you can use a specific enum value for this case
        {
            status.Status = ItemBodyDBOperationStatus.UnknownError;
        }
        return status;
    }

    public async Task<ItemBodyUpdateOperationResult> CheckIfSuchBodyCanBeSaved(int itembodyid, string name, string myname, string mpn)
    {
        ItemBodyUpdateOperationResult status = new ItemBodyUpdateOperationResult
        {
            Status = ItemBodyDBOperationStatus.OperationSuccessful,
            itembody = null
        };

        await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var itemBodyService = new EntityService<itembody>(unitOfWork);

            var cialko = await itemBodyService.GetOneAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false);
            var res = await itemBodyService.GetOneAsync(p => p.name.Equals(name) && p.itembodyID != itembodyid).ConfigureAwait(false);
            if (res != null)
            {
                status.Status = ItemBodyDBOperationStatus.ObjectWithNameExists;
                status.itembody = res;
            }

            res = await itemBodyService.GetOneAsync(p => p.myname.Equals(myname) && p.itembodyID != itembodyid).ConfigureAwait(false);
            if (res != null)
            {
                status.Status = ItemBodyDBOperationStatus.ObjectWithShortNameExists;
                status.itembody = res;
            }

            await itemBodyService.GetOneAsync(p => p.mpn.Equals(mpn) && p.itembodyID != itembodyid).ConfigureAwait(false);
            if (res != null)
            {
                status.Status = ItemBodyDBOperationStatus.ObjectWithMpnExists;
                status.itembody = res;
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (status.Status == (ItemBodyDBOperationStatus)(-2)) // Assuming -2 is a valid enum value or you can use a specific enum value for this case
        {
            status.Status = ItemBodyDBOperationStatus.UnknownError;
        }
        return status;
    }

    public async Task CurOrds2BackOrds(List<casioukbackorder> cuoList)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var casioUKbackordersService = new EntityService<casioukbackorder>(unitOfWork);
            await casioUKbackordersService.AddRangeAsync(cuoList).ConfigureAwait(false);
            await unitOfWork.DeleteAllFromTable("casioukcurrentorder");
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task<List<casioinvoice>> FindAmongCasioUKInvoices(string mpn)
    {
        List<casioinvoice> zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var CasioInvoiceService = new EntityService<casioinvoice>(unitOfWork);
            zwrotka = (await CasioInvoiceService.GetAllAsync(p => p.mpn.Equals(mpn)).ConfigureAwait(false)).ToList();
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return zwrotka;
    }

    public async Task<bool> FlipReadyToTrack(int itembodyid)
    {
        itembody cialko = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var itemBodyService = new EntityService<itembody>(unitOfWork);
            cialko = await itemBodyService.GetOneAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false);
            cialko.readyTotrack = !cialko.readyTotrack;
            await itemBodyService.UpdateAsync(cialko).ConfigureAwait(false);
            if (DALState == DatabaseAccessLayerState.Loaded)
            {
                items[itembodyid].itembody = cialko;
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        OnItemBodiesChanged(new List<int> { itembodyid });
        return cialko.readyTotrack;
    }

    public async Task<List<casioukbackorder>> GetCasioUKBackorders()
    {
        List<casioukbackorder> cbo = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            var context = DbContextFactory.GetContext();
            cbo = (await context.casioukbackorders.ToListAsync().ConfigureAwait(false)).ToList();
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return cbo;
    }

    public async Task UpdateQuantityInCasioBackorder(int id, int quantity)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            var context = DbContextFactory.GetWriteableContext();
            var bo = await context.casioukbackorders
                .FirstOrDefaultAsync(p => p.casioUKbackorderId == id)
                .ConfigureAwait(false);

            if (bo != null)
            {
                bo.quantity = quantity;
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }
    public async Task<Answer> GetInvoiceTxns(List<int> orderIds)
    {
        var response = Answer.Prepare($"Getting invoicetxn with orderids : {string.Join(',', orderIds)}");
        List<invoicetxn> zwrotka = null;

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            var context = DbContextFactory.GetContext();
            zwrotka = await context.invoicetxns
                .Where(p => orderIds.Contains(p.orderID))
                .ToListAsync()
                .ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        return response.WithValue(zwrotka);
    }

    public async Task<List<casioukcurrentorder>> GetCasioUKCurrentorders()
    {
        List<casioukcurrentorder> cbo = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            var context = DbContextFactory.GetContext();
            cbo = await context.casioukcurrentorders
                .ToListAsync()
                .ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }

        return cbo;
    }


    public async Task GetPackage(int locationId)
    {
        if (DALState == DatabaseAccessLayerState.Loaded) { return; }
        DALState = DatabaseAccessLayerState.Loading;
        if (!_lazyItemCechy.IsValueCreated)
        {
            var _ = await Itmcechies(); // To spowoduje inicjalizację _lazyItemCechy
        }
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var itemBodiesService = new EntityService<itembody>(unitOfWork);
            var itemBodies = (await itemBodiesService.GetAllAsync().ConfigureAwait(false)).ToList();
            items = new Dictionary<int, AssociatedData>();
            var itemHeadersService = new EntityService<itemheader>(unitOfWork);
            var itemHeaders = (await itemHeadersService.GetAllAsync().ConfigureAwait(false)).ToList();
            var BodyInTheBoxService = new EntityService<bodyinthebox>(unitOfWork);
            BodyInTheBoxes = (await BodyInTheBoxService.GetAllAsync().ConfigureAwait(false)).ToList();
            var ItmMarketAssocsService = new EntityService<itmmarketassoc>(unitOfWork);
            var ItmMarketAssocs = (await ItmMarketAssocsService.GetAllAsync().ConfigureAwait(false)).ToList();
            var photosService = new EntityService<photo>(unitOfWork);
            var photos = (await photosService.GetAllAsync().ConfigureAwait(false)).GroupBy(p => p.itembodyID).ToDictionary(a => a.Key, b => b.ToList());
            await using (var context = new Time4PartsContext())
            {
                foreach (var itembody in itemBodies)
                {
                    var hedy = itemHeaders.Where(x => x.itembodyID == itembody.itembodyID && x.locationID == 1).ToList();

                    var bodyinthebox = BodyInTheBoxes.FirstOrDefault(x => x.itembodyID == itembody.itembodyID);

                    var itmCechies = (await Itmcechies().ConfigureAwait(false)).Where(p => p.itembodyID == itembody.itembodyID).ToList(); ;
                    var itmMarketAssocs = ItmMarketAssocs.Where(x => x.itembodyID == itembody.itembodyID).ToList();
                    var associatedData = new AssociatedData
                    {
                        itembody = itembody,
                        ItemHeaders = hedy,
                        bodyinthebox = bodyinthebox,
                        ItmCechies = itmCechies,
                        ItmMarketAssocs = itmMarketAssocs,
                    };
                    if (photos.TryGetValue(itembody.itembodyID, out var photoList))
                    {
                        associatedData.Photos = photoList.OrderBy(p => p.pos).ToList();
                    }
                    else
                    {
                        associatedData.Photos = new List<photo>();
                    }
                    items.Add(itembody.itembodyID, associatedData);
                }

                colourProperty = 12;
            }
            DALState = DatabaseAccessLayerState.Loaded;
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return;
        }
    }

    public async Task<int> IncreaseQuantityInHeader(int itemheaderId, int quantity, string whatBroughtYouHere)
    {
        itemheader itm = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var itemHeadersService = new EntityService<itemheader>(unitOfWork);
            itm = (await itemHeadersService.GetByIdAsync(itemheaderId).ConfigureAwait(false)).GetValue<itemheader>();
            var oldq = itm.quantity;
            itm.quantity += quantity;
            await itemHeadersService.UpdateAsync(itm).ConfigureAwait(false);
            var ItemLogEventService = new EntityService<logevent>(unitOfWork);
            var le = new logevent
            {
                happenedOn = DateTime.Now,
                _event = "changed quantity " + oldq + " by " + quantity + " to " + itm.quantity + " " + whatBroughtYouHere,
                itemHeaderID = itm.itemheaderID,
                itemBodyID = itm.itembodyID
            };
            await ItemLogEventService.AddAsync(le).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return -2;
        }
        if (DALState == DatabaseAccessLayerState.Loaded)
        {
            items[itm.itembodyID].ItemHeaders.First(p => p.itemheaderID == itemheaderId).quantity = itm.quantity;
        }
        OnItemBodiesChanged(new List<int> { itm.itembodyID });
        return itm.quantity;
    }

    public async Task<int> SetQuantityInHeader(int itembodyId, int quantity, string whatBroughtYouHere)
    {
        List<itemheader> itm = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var itemHeadersService = new EntityService<itemheader>(unitOfWork);
            itm = (await itemHeadersService.GetAllAsync(p => p.itembodyID == itembodyId).ConfigureAwait(false))
                .ToList();


            int baseAmount = quantity / itm.Count;
            int remainder = quantity % itm.Count;

            for (int i = 0; i < itm.Count; i++)
            {
                itm[i].quantity = baseAmount + (i < remainder ? 1 : 0);
            }

            await itemHeadersService.UpdateRangeAsync(itm).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return -2;
        }
        if (DALState == DatabaseAccessLayerState.Loaded)
        {
            items[itembodyId].ItemHeaders = itm;
            OnItemBodiesChanged(new List<int> { itembodyId });
        }

        return 0;
    }





    public async Task MoveDownCechaValue(int parameterValueID)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var cechyValuesService = new EntityService<parametervalue>(unitOfWork);

            var toMoveDown = (await cechyValuesService.GetByIdAsync(parameterValueID).ConfigureAwait(false)).GetValue<parametervalue>();
            if (toMoveDown != null)
            {
                var toMoveUp = await cechyValuesService.GetOneAsync(t => t.parameterID == toMoveDown.parameterID && t.pos == toMoveDown.pos + 1).ConfigureAwait(false);
                if (toMoveUp != null)
                {
                    toMoveUp.pos--;
                    toMoveDown.pos++;
                    await cechyValuesService.UpdateAsync(toMoveUp).ConfigureAwait(false);
                    await cechyValuesService.UpdateAsync(toMoveDown).ConfigureAwait(false);
                }

                if (_lazyCechyValues.IsValueCreated)
                {
                    Dictionary<int, List<parametervalue>> excv = await _lazyCechyValues.Value.ConfigureAwait(false);
                    int index = excv[toMoveDown.parameterID].FindIndex(p => p.parameterValueID == toMoveUp.parameterValueID);
                    if (index != -1)
                    {
                        excv[toMoveDown.parameterID][index] = toMoveUp;
                    }
                    int indexDown = excv[toMoveDown.parameterID].FindIndex(p => p.parameterValueID == toMoveDown.parameterValueID);
                    if (indexDown != -1)
                    {
                        excv[toMoveDown.parameterID][indexDown] = toMoveDown;
                    }
                }
                if (_lazyParameters4AllBodies.IsValueCreated)
                {
                    var cechyService = new EntityService<parameter>(unitOfWork);
                    var itmParametersService = new EntityService<itmparameter>(unitOfWork);
                    var cecha = (await cechyService.GetByIdAsync(toMoveUp.parameterID).ConfigureAwait(false)).GetValue<parameter>();

                    if (cecha != null)
                    {
                        List<itmparameter> doiter = (await itmParametersService.GetAllAsync(
                                p => p.parameterValueID == toMoveUp.parameterValueID || p.parameterValueID == toMoveDown.parameterValueID).ConfigureAwait(false))
                            .ToList();

                        Dictionary<int, Dictionary<parameter, parametervalue>> exc4a = await _lazyParameters4AllBodies.Value.ConfigureAwait(false);

                        foreach (var itema in doiter)
                        {
                            if (exc4a.ContainsKey(itema.itembodyID))
                            {
                                var innerDict = exc4a[itema.itembodyID];
                                if (innerDict.ContainsKey(cecha))
                                {
                                    if (innerDict[cecha].parameterValueID == toMoveUp.parameterValueID)
                                    {
                                        innerDict[cecha] = toMoveUp;
                                    }
                                    else if (innerDict[cecha].parameterValueID == toMoveDown.parameterValueID)
                                    {
                                        innerDict[cecha] = toMoveDown;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task MoveUpCechaValue(int parameterValueID)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var cechyValuesService = new EntityService<parametervalue>(unitOfWork);

            var toMoveUp = (await cechyValuesService.GetByIdAsync(parameterValueID).ConfigureAwait(false)).GetValue<parametervalue>();
            if (toMoveUp != null)
            {
                var toMoveDown = await cechyValuesService.GetOneAsync(t => t.parameterID == toMoveUp.parameterID && t.pos == toMoveUp.pos - 1).ConfigureAwait(false);
                if (toMoveDown != null)
                {
                    toMoveUp.pos--;
                    toMoveDown.pos++;
                    await cechyValuesService.UpdateAsync(toMoveUp).ConfigureAwait(false);
                    await cechyValuesService.UpdateAsync(toMoveDown).ConfigureAwait(false);
                }

                if (_lazyCechyValues.IsValueCreated)
                {
                    Dictionary<int, List<parametervalue>> excv = await _lazyCechyValues.Value.ConfigureAwait(false);
                    int index = excv[toMoveDown.parameterID].FindIndex(p => p.parameterValueID == toMoveUp.parameterValueID);
                    if (index != -1)
                    {
                        excv[toMoveDown.parameterID][index] = toMoveUp;
                    }
                    int indexDown = excv[toMoveDown.parameterID].FindIndex(p => p.parameterValueID == toMoveDown.parameterValueID);
                    if (indexDown != -1)
                    {
                        excv[toMoveDown.parameterID][indexDown] = toMoveDown;
                    }
                }
                if (_lazyParameters4AllBodies.IsValueCreated)
                {
                    var cechyService = new EntityService<parameter>(unitOfWork);
                    var itmParametersService = new EntityService<itmparameter>(unitOfWork);
                    var cecha = (await cechyService.GetByIdAsync(toMoveUp.parameterID).ConfigureAwait(false)).GetValue<parameter>();

                    if (cecha != null)
                    {
                        List<itmparameter> doiter = (await itmParametersService.GetAllAsync(
                                p => p.parameterValueID == toMoveUp.parameterValueID || p.parameterValueID == toMoveDown.parameterValueID).ConfigureAwait(false))
                            .ToList();

                        Dictionary<int, Dictionary<parameter, parametervalue>> exc4a = await _lazyParameters4AllBodies.Value.ConfigureAwait(false);

                        foreach (var itema in doiter)
                        {
                            if (exc4a.ContainsKey(itema.itembodyID))
                            {
                                var innerDict = exc4a[itema.itembodyID];
                                if (innerDict.ContainsKey(cecha))
                                {
                                    if (innerDict[cecha].parameterValueID == toMoveUp.parameterValueID)
                                    {
                                        innerDict[cecha] = toMoveUp;
                                    }
                                    else if (innerDict[cecha].parameterValueID == toMoveDown.parameterValueID)
                                    {
                                        innerDict[cecha] = toMoveDown;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task MoveDownTypePar(int parameterID, int typeid)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var typeParAssociationsService = new EntityService<typeparassociation>(unitOfWork);
            var toMoveDown = await typeParAssociationsService.GetOneAsync(t => t.typeID == typeid && t.parameterID == parameterID).ConfigureAwait(false);
            if (toMoveDown != null)
            {
                var toMoveUp = await typeParAssociationsService.GetOneAsync(t => t.typeID == typeid && t.pos == toMoveDown.pos + 1).ConfigureAwait(false);
                if (toMoveUp != null)
                {
                    toMoveUp.pos--;
                    toMoveDown.pos++;
                    await typeParAssociationsService.UpdateAsync(toMoveUp).ConfigureAwait(false);
                    await typeParAssociationsService.UpdateAsync(toMoveDown).ConfigureAwait(false);
                }
            }

            if (_lazyTypePars.IsValueCreated)
            {
                var existingTypePars = await _lazyTypePars.Value.ConfigureAwait(false);
                if (existingTypePars.ContainsKey(typeid))
                {
                    existingTypePars[typeid] = (await typeParAssociationsService.GetAllAsync(p => p.typeID == typeid).ConfigureAwait(false)).OrderBy(p => p.pos).ToList();
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task MoveUpTypePar(int parameterID, int typeid)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var typeParAssociationsService = new EntityService<typeparassociation>(unitOfWork);
            var toMoveUp = await typeParAssociationsService.GetOneAsync(t => t.typeID == typeid && t.parameterID == parameterID).ConfigureAwait(false);
            if (toMoveUp != null)
            {
                var toMoveDown = await typeParAssociationsService.GetOneAsync(t => t.typeID == typeid && t.pos == toMoveUp.pos - 1).ConfigureAwait(false);
                if (toMoveUp != null)
                {
                    toMoveUp.pos--;
                    toMoveDown.pos++;
                    await typeParAssociationsService.UpdateAsync(toMoveUp).ConfigureAwait(false);
                    await typeParAssociationsService.UpdateAsync(toMoveDown).ConfigureAwait(false);
                }
            }

            if (_lazyTypePars.IsValueCreated)
            {
                var existingTypePars = await _lazyTypePars.Value.ConfigureAwait(false);
                if (existingTypePars.ContainsKey(typeid))
                {
                    existingTypePars[typeid] = (await typeParAssociationsService.GetAllAsync(p => p.typeID == typeid).ConfigureAwait(false)).OrderBy(p => p.pos).ToList();
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task MovRegPhotoUp(int photoid, int itembodyid)
    {
        List<photo> photki = new List<photo>();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var photosService = new EntityService<photo>(unitOfWork);

            var fotki = await photosService.GetAllAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false);
            var fotkadol = fotki.First(p => p.photoID == photoid);
            var fotkagora = fotki.First(p => p.pos == fotkadol.pos - 1);
            fotkadol.pos--;
            fotkagora.pos++;
            await photosService.UpdateAsync(fotkadol).ConfigureAwait(false);
            await photosService.UpdateAsync(fotkagora).ConfigureAwait(false);

            photki = fotki.ToList();
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return;
        }
        items[itembodyid].Photos = photki;
    }

    public async Task RefreshBodies(List<int> ids)
    {
        Dictionary<int, itembody> itemBodies = null;
        Dictionary<int, List<itemheader>> heds = null;
        Dictionary<int, bodyinthebox> bodiesInBoxes = null;
        Dictionary<int, List<itmparameter>> ceszki = null;
        Dictionary<int, List<itmmarketassoc>> ima = null;
        Dictionary<int, List<photo>> photos = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var photosService = new EntityService<photo>(unitOfWork);
            var ItemBodiesService = new EntityService<itembody>(unitOfWork);
            var ItemHeaderService = new EntityService<itemheader>(unitOfWork);
            var bodiesInTheboxesService = new EntityService<bodyinthebox>(unitOfWork);
            var itmMarketAssocService = new EntityService<itmmarketassoc>(unitOfWork);
            var itmParametersService = new EntityService<itmparameter>(unitOfWork);
            itemBodies = (await ItemBodiesService.GetAllAsync(p => ids.Contains(p.itembodyID)).ConfigureAwait(false)).ToDictionary(p => p.itembodyID, q => q);
            heds = (await ItemHeaderService.GetAllAsync(p => ids.Contains(p.itembodyID) && p.locationID == 1).ConfigureAwait(false)).GroupBy(p => p.itembodyID).ToDictionary(p => p.Key, q => q.ToList());
            bodiesInBoxes = (await bodiesInTheboxesService.GetAllAsync(p => ids.Contains(p.itembodyID)).ConfigureAwait(false)).ToDictionary(p => p.itembodyID, q => q);
            ceszki = (await itmParametersService.GetAllAsync(p => ids.Contains(p.itembodyID)).ConfigureAwait(false)).GroupBy(p => p.itembodyID).ToDictionary(p => p.Key, q => q.ToList());
            ima = (await itmMarketAssocService.GetAllAsync(p => ids.Contains(p.itembodyID)).ConfigureAwait(false)).GroupBy(p => p.itembodyID).ToDictionary(p => p.Key, q => q.ToList());
            photos = (await photosService.GetAllAsync(p => ids.Contains(p.itembodyID)).ConfigureAwait(false)).GroupBy(p => p.itembodyID).ToDictionary(p => p.Key, q => q.ToList());
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return;
        }
        foreach (var cialko in itemBodies)
        {
            var associatedData = new AssociatedData();

            associatedData.itembody = cialko.Value;

            if (heds.ContainsKey(cialko.Key))
            {
                associatedData.ItemHeaders = heds[cialko.Key];
            }

            associatedData.bodyinthebox = bodiesInBoxes.ContainsKey(cialko.Key) ? bodiesInBoxes[cialko.Key] : null;

            if (ceszki.ContainsKey(cialko.Key))
            {
                associatedData.ItmCechies = ceszki[cialko.Key];
            }

            if (ima.ContainsKey(cialko.Key))
            {
                associatedData.ItmMarketAssocs = ima[cialko.Key];
            }

            if (photos.ContainsKey(cialko.Key))
            {
                associatedData.Photos = photos[cialko.Key].OrderBy(p => p.pos).ToList();
            }
            else
            {
                associatedData.Photos = new List<photo>();
            }

            items[cialko.Key] = associatedData;
        }
        OnItemBodiesChanged(ids);
    }

    public async Task RefreshBody(int itembodyid, bool refreshNeeded)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var ItemBodiesService = new EntityService<itembody>(unitOfWork);
            var photosService = new EntityService<photo>(unitOfWork);
            var cialko = await ItemBodiesService.GetOneAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false);
            items[itembodyid].itembody = cialko;
            items[itembodyid].Photos = (await photosService.GetAllAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false)).OrderBy(p => p.pos).ToList();
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (refreshNeeded)
        {
            OnItemBodiesChanged(new List<int> { itembodyid });
        }
    }

    public async Task RefreshMarket(FullItmMarketAss itema)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var itmMarketAssocService = new EntityService<itmmarketassoc>(unitOfWork);
            var PlatformService = new EntityService<platform>(unitOfWork);
            var AsinSkuService = new EntityService<asinsku>(unitOfWork);
            var MarketPlatformAssociationService = new EntityService<marketplatformassociation>(unitOfWork);
            var gu = await itmMarketAssocService.GetOneAsync(p => p.itmmarketassID == itema.itmmarketassoc.itmmarketassID).ConfigureAwait(false);
            gu.SEName = itema.itmmarketassoc.SEName;
            gu.quantitySold = itema.itmmarketassoc.quantitySold;
            gu.itemNumber = itema.itmmarketassoc.itemNumber;
            gu.soldWith = itema.itmmarketassoc.soldWith;
            await itmMarketAssocService.UpdateAsync(gu).ConfigureAwait(false);

            asinsku gus = null;
            int platform = (await MarketPlatformAssociationService.GetOneAsync(p => p.marketID == itema.itmmarketassoc.marketID).ConfigureAwait(false)).platformID;
            string platformName = (await PlatformService.GetByIdAsync(platform).ConfigureAwait(false)).GetValue<platform>().name;

            if (platformName.ToLower().Contains("amazon"))
            {
                gus = await AsinSkuService.GetOneAsync(p => p.asinskuID == itema.SKU.asinskuID).ConfigureAwait(false);
                gus.asin = itema.itmmarketassoc.itemNumber;
                gus.sku = itema.SKU.sku;
                await AsinSkuService.UpdateAsync(gus).ConfigureAwait(false);
            }

            int index = items[gu.itembodyID].ItmMarketAssocs.FindIndex(x => x.itmmarketassID == gu.itmmarketassID);
            if (index != -1)
            {
                items[gu.itembodyID].ItmMarketAssocs[index] = gu;
            }

            if (gus != null)
            {
                index = (await ASINSKUS().ConfigureAwait(false)).FindIndex(x => x.asinskuID == gus.asinskuID);
                if (index != -1)
                {
                    if (_lazyASINSKUS.IsValueCreated)
                    {
                        var existingAsinSkus = await _lazyASINSKUS.Value.ConfigureAwait(false);
                        existingAsinSkus[index] = gus;
                    }
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return;
        }
    }

    public async Task RefreshQuantitiesInHeaders(Dictionary<KeyValuePair<int, string>, int> quantities, int itemBodyId, int locationID, bool? flipReady = null)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var ItemHeaderService = new EntityService<itemheader>(unitOfWork);
            var ItemBodyService = new EntityService<itembody>(unitOfWork);
            var SupplierService = new EntityService<supplier>(unitOfWork);
            var logeventService = new EntityService<logevent>(unitOfWork);
            var hedy = await ItemHeaderService.GetAllAsync(p => p.itembodyID == itemBodyId && p.locationID == locationID).ConfigureAwait(false);
            var suppliers = (await SupplierService.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.supplierID, q => q.name);

            var logi = new List<logevent>();
            foreach (var itema in hedy)
            {
                var newQuantity = quantities.First(p => p.Key.Key == itema.supplierID).Value;
                logi.Add(new logevent
                {
                    happenedOn = DateTime.Now,
                    _event = $"quantity(from provider {suppliers[itema.supplierID]}) changed from {itema.quantity} to {newQuantity} while turning the tracking on",
                    itemHeaderID = itema.itemheaderID,
                    itemBodyID = itemBodyId
                });
                itema.quantity = quantities.First(p => p.Key.Key == itema.supplierID).Value;
            }
            await logeventService.AddRangeAsync(logi).ConfigureAwait(false);

            if (DALState == DatabaseAccessLayerState.Loaded)
            {
                items[itemBodyId].itembody.readyTotrack = true;
                foreach (var itema in hedy)
                {
                    var oldhed = items[itemBodyId].ItemHeaders.FirstOrDefault(p => p.itemheaderID == itema.itemheaderID);
                    if (oldhed != null)
                    {
                        var index = items[itemBodyId].ItemHeaders.IndexOf(oldhed);
                        items[itemBodyId].ItemHeaders[index] = itema;
                    }
                }
            }
            await ItemHeaderService.UpdateRangeAsync(hedy);

            if (flipReady.HasValue && flipReady.Value)
            {
                var cialko = (await ItemBodyService.GetByIdAsync(itemBodyId).ConfigureAwait(false)).GetValue<itembody>();
                cialko.readyTotrack = true;
                await ItemBodyService.UpdateAsync(cialko).ConfigureAwait(false);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return;
        }
        OnItemBodiesChanged([itemBodyId]);
    }

    public async Task<casioukcurrentorder> RefreshUKCurOrd(casioukcurrentorder cuo)
    {
        casioukcurrentorder zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var casioUKcurrentordersService = new EntityService<casioukcurrentorder>(unitOfWork);
            var zwrotka = (await casioUKcurrentordersService.GetByIdAsync(cuo.casioUKcurrentOrderId).ConfigureAwait(false)).GetValue<casioukcurrentorder>();
            zwrotka.quantity = cuo.quantity;
            await casioUKcurrentordersService.UpdateAsync(zwrotka);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return zwrotka;
    }

    public async Task RemoveBodiesGrouped(int id)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using (var context = new Time4PartsContext())
            {
                using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
                var bodiesGroupedsService = new EntityService<bodiesgrouped>(unitOfWork);
                var group4bodiesService = new EntityService<group4body>(unitOfWork);
                var bodG = await bodiesGroupedsService.GetAllAsync(p => p.group4bodiesID == id).ConfigureAwait(false);
                var bodGIds = bodG.Select(b => b.group4bodiesID);
                await bodiesGroupedsService.DeleteRangeAsync(bodG).ConfigureAwait(false);
                var g4b = await group4bodiesService.GetOneAsync(p => p.group4bodiesID == id).ConfigureAwait(false);
                await group4bodiesService.DeleteAsync(g4b).ConfigureAwait(false);
                if (_lazyBodiesGrouped.IsValueCreated)
                {
                    var existingBodiesGrouped = await _lazyBodiesGrouped.Value.ConfigureAwait(false);
                    existingBodiesGrouped.RemoveAll(b => bodGIds.Contains(b.group4bodiesID));
                }
                if (_lazyGroup4Bodies.IsValueCreated)
                {
                    var existingGroup4Bodies = await _lazyGroup4Bodies.Value.ConfigureAwait(false);
                    existingGroup4Bodies.Remove(g4b);
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task RemoveBodyFromGroup(int id, int itembodyid)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var bodiesGroupedsService = new EntityService<bodiesgrouped>(unitOfWork);
            var body = await bodiesGroupedsService.GetOneAsync(p => p.group4bodiesID == id && p.itemBodyID == itembodyid).ConfigureAwait(false);
            if (body != null)
            {
                await bodiesGroupedsService.DeleteAsync(body);
            }
            if (_lazyBodiesGrouped.IsValueCreated)
            {
                var existingBodiesGrouped = await _lazyBodiesGrouped.Value.ConfigureAwait(false);
                existingBodiesGrouped.Remove(body);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task RemoveBodyInTheBox(bodyinthebox bodyinthebox)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var bodyInTheBoxService = new EntityService<bodyinthebox>(unitOfWork);
            var cialko = await bodyInTheBoxService.GetOneAsync(p => p.BodyInTheBoxID == bodyinthebox.BodyInTheBoxID).ConfigureAwait(false);
            if (cialko != null)
            {
                await bodyInTheBoxService.DeleteAsync(cialko).ConfigureAwait(false);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return;
        }
        items[bodyinthebox.itembodyID].bodyinthebox = null;
        return;
    }

    public async Task RemoveCasioBackorder(int id)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var casioUKbackorderService = new EntityService<casioukbackorder>(unitOfWork);
            var bo = await casioUKbackorderService.GetOneAsync(p => p.casioUKbackorderId == id).ConfigureAwait(false);
            if (bo != null)
            {
                await casioUKbackorderService.DeleteAsync(bo).ConfigureAwait(false);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task RemoveCecha(int parameterID)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var cechyService = new EntityService<parameter>(unitOfWork);
            var cecha = await cechyService.GetOneAsync(p => p.parameterID == parameterID).ConfigureAwait(false);
            if (cecha != null)
            {
                await cechyService.DeleteAsync(cecha).ConfigureAwait(false);

                if (_lazyCechyValues.IsValueCreated)
                {
                    Dictionary<int, List<parametervalue>> excv = await _lazyCechyValues.Value.ConfigureAwait(false);
                    if (excv.ContainsKey(parameterID))
                    {
                        excv.Remove(parameterID);
                    }
                }
                if (_lazyCechy.IsValueCreated)
                {
                    var exC = await _lazyCechy.Value.ConfigureAwait(false);
                    if (exC.ContainsKey(parameterID))
                    {
                        exC.Remove(parameterID);
                    }
                }
                List<itmparameter> sparowane = null;
                if (_lazyItemCechy.IsValueCreated)
                {
                    var lic = await _lazyItemCechy.Value.ConfigureAwait(false);
                    sparowane = lic.Where(p => p.parameterID == parameterID).ToList();
                }
                var doiter = sparowane.ToDictionary(p => p.itembodyID, q => q);
                foreach (var item in doiter)
                {
                    var itema = items[item.Key];
                    itema.ItmCechies.RemoveAll(p => p.parameterID == parameterID);
                    itema.PrzyporzadkowaniaCech.Remove(cecha);
                }
                if (_lazyParameters4AllBodies.IsValueCreated)
                {
                    var exlc4a = await _lazyParameters4AllBodies.Value.ConfigureAwait(false);
                    foreach (var item in doiter)
                    {
                        var inceptionLevelOne = exlc4a[item.Key];
                        inceptionLevelOne.Remove(cecha);
                    }
                }

                if (_lazyParameters4AllBodies.IsValueCreated)
                {
                    var existingc4ab1 = await _lazyParameters4AllBodies.Value.ConfigureAwait(false);
                    foreach (var item in doiter)
                    {
                        var inceptionLevelOne = existingc4ab1[item.Key];
                        inceptionLevelOne.Remove(cecha);
                    }
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task RemoveParameterValue(int parameterValueID)
    {
        List<int> typesids = new();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var cechyValueService = new EntityService<parametervalue>(unitOfWork);
            var itmCechyService = new EntityService<itmparameter>(unitOfWork);
            var cechyService = new EntityService<parameter>(unitOfWork);
            var typeParAssociationService = new EntityService<typeparassociation>(unitOfWork);
            var toRemove = (await cechyValueService.GetByIdAsync(parameterValueID).ConfigureAwait(false)).GetValue<parametervalue>();
            if (toRemove != null)
            {
                var itemcechies = await itmCechyService.GetAllAsync(p => p.parameterValueID == parameterValueID).ConfigureAwait(false);
                var itemsToUpdate = await cechyValueService.GetAllAsync(t => t.parameterID == toRemove.parameterID && t.pos > toRemove.pos).ConfigureAwait(false);
                foreach (var item in itemsToUpdate)
                {
                    item.pos--;
                }
                await cechyValueService.UpdateRangeAsync(itemsToUpdate).ConfigureAwait(false);
                await itmCechyService.DeleteRangeAsync(itemcechies).ConfigureAwait(false);
                await cechyValueService.DeleteAsync(toRemove).ConfigureAwait(false);
                var cecha = (await cechyService.GetByIdAsync(toRemove.parameterID).ConfigureAwait(false)).GetValue<parameter>();
                typesids = (await typeParAssociationService.GetAllAsync(p => p.parameterID == cecha.parameterID).ConfigureAwait(false)).Select(p => p.typeID).ToList();
                if (_lazyCechyValues.IsValueCreated)
                {
                    Dictionary<int, List<parametervalue>> excv = await _lazyCechyValues.Value.ConfigureAwait(false);
                    excv[toRemove.parameterID] = (await cechyValueService.GetAllAsync(p => p.parameterID == toRemove.parameterID).ConfigureAwait(false)).ToList();
                }
                if (_lazyParameters4AllBodies.IsValueCreated)
                {
                    var lc4a = await _lazyParameters4AllBodies.Value.ConfigureAwait(false);

                    foreach (var itema in itemcechies)
                    {
                        lc4a[itema.itembodyID].Remove(cecha);
                    }
                }
                if (_lazyParameters4AllBodies.IsValueCreated)
                {
                    var lc4ab = await _lazyParameters4AllBodies.Value.ConfigureAwait(false);
                    foreach (var itema in itemcechies)
                    {
                        if (lc4ab[itema.itembodyID].ContainsKey(cecha))
                        {
                            lc4ab[itema.itembodyID].Remove(cecha);
                        }
                    }
                }
                foreach (var itema in itemcechies)
                {
                    items[itema.itembodyID].ItmCechies.Remove(itema);
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        await _entityEventsService.PublishAsync<type>(new EntityEventArgs(typesids, EntityActionType.Update));

    }

    public async Task RemoveDrawer(int multidrawerid)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var MultiDrawerService = new EntityService<multidrawer>(unitOfWork);
            var zw = (await MultiDrawerService.GetByIdAsync(multidrawerid).ConfigureAwait(false)).GetValue<multidrawer>();
            await MultiDrawerService.DeleteAsync(zw).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task RemoveMarket(FullItmMarketAss itema)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var itmMarketAssocService = new EntityService<itmmarketassoc>(unitOfWork);
            var MarketPlatformAssociationService = new EntityService<marketplatformassociation>(unitOfWork);
            var orderItemService = new EntityService<orderitem>(unitOfWork);

            var AsinSkuService = new EntityService<asinsku>(unitOfWork);
            var PlatformService = new EntityService<platform>(unitOfWork);

            var oi = (await orderItemService.GetAllAsync(p =>
                p.itmMarketAssID == itema.itmmarketassoc.itmmarketassID).ConfigureAwait(false)).Count();

            if (oi == 0)
            {
                int platforma = (await MarketPlatformAssociationService
                        .GetOneAsync(p => p.marketID == itema.itmmarketassoc.marketID).ConfigureAwait(false))
                    .platformID;
                string platformName = (await PlatformService.GetByIdAsync(platforma).ConfigureAwait(false)).GetValue<parameter>().name;
                var gu = await itmMarketAssocService.GetOneAsync(p => p.itmmarketassID == itema.itmmarketassoc.itmmarketassID)
                    .ConfigureAwait(false);

                if (platformName.ToLower().Contains("amazon"))
                {
                    asinsku gus = null;
                    gus = (await AsinSkuService.GetByIdAsync(itema.SKU.asinskuID).ConfigureAwait(false)).GetValue<asinsku>();
                    if (gus != null)
                    {
                        await AsinSkuService.DeleteAsync(gus);
                        if (_lazyASINSKUS.IsValueCreated)
                        {
                            var existingAsinSkus = await _lazyASINSKUS.Value.ConfigureAwait(false);
                            var xo = existingAsinSkus.First(p => p.asinskuID == itema.SKU.asinskuID);
                            existingAsinSkus.Remove(xo);
                        }
                    }
                }

                await itmMarketAssocService.DeleteAsync(gu).ConfigureAwait(false);
                var xe = items[gu.itembodyID].ItmMarketAssocs.First(p => p.itmmarketassID == itema.itmmarketassoc.itmmarketassID);
                items[gu.itembodyID].ItmMarketAssocs.Remove(xe);
            }
            else
            {
                var gu = await itmMarketAssocService.GetOneAsync(p => p.itmmarketassID == itema.itmmarketassoc.itmmarketassID)
                    .ConfigureAwait(false);
                var index = items[gu.itembodyID].ItmMarketAssocs
                    .FindIndex(p => p.itmmarketassID == gu.itmmarketassID);

                gu.itemNumber = "none";
                await itmMarketAssocService.UpdateAsync(gu).ConfigureAwait(false);
                items[gu.itembodyID].ItmMarketAssocs[index] = gu;
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task RemoveMayAlsoFit(int group4bodiesID, int group4watchesID)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var mayalsofitService = new EntityService<mayalsofit>(unitOfWork);
            var maf = await mayalsofitService.GetOneAsync(p => p.group4bodiesID == group4bodiesID && p.group4watchesID == group4watchesID).ConfigureAwait(false);
            if (maf != null)
            {
                await mayalsofitService.DeleteAsync(maf).ConfigureAwait(false);
                if (_lazyMayalsofits.IsValueCreated)
                {
                    var existingMAF = await _lazyMayalsofits.Value.ConfigureAwait(false);
                    existingMAF.Remove(maf);
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task<Answer> RemoveType(int typeId)
    {
        var response = Answer.Prepare($"Removing entity type with typeid {typeId}");
        List<itembody> bodies = new();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using UnitOfWork unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            EntityService<type> typeService = new EntityService<type>(unitOfWork);
            EntityService<itembody> itembodyService = new EntityService<itembody>(unitOfWork);
            var typeResponse = await typeService.GetByIdAsync(typeId).ConfigureAwait(false);
            if (!typeResponse.IsSuccess)
            {
                response = response.Attach(typeResponse);
                return; // zaznaczona linia 
            }
            type? typek = (await typeService.GetByIdAsync(typeId).ConfigureAwait(false)).GetValue<type>();

            if (typek != null)
            {
                bodies = (await itembodyService.GetAllAsync(p => p.typeId == typek.typeID).ConfigureAwait(false)).ToList();
                if (bodies != null)
                {
                    type? unassignedType = await typeService.GetOneAsync(p => p.name.ToLower().Equals("unassigned"))
                        .ConfigureAwait(false);
                    foreach (itembody body in bodies)
                    {
                        body.typeId = unassignedType.typeID;
                    }

                    await itembodyService.UpdateRangeAsync(bodies).ConfigureAwait(false);
                    foreach (var itembody in bodies)
                    {
                        items[itembody.itembodyID].itembody = itembody;
                        items[itembody.itembodyID].PrzyporzadkowaniaCech = new();
                        items[itembody.itembodyID].ItmCechies = new();
                    }
                }

                await typeService.DeleteAsync(typek).ConfigureAwait(false);
                if (_lazyTypes.IsValueCreated)
                {
                    Dictionary<int, string> existingTypes = await _lazyTypes.Value.ConfigureAwait(false);
                    existingTypes.Remove(typeId);
                }

                if (_lazyTypePars.IsValueCreated)
                {
                    Dictionary<int, List<typeparassociation>> existingTypePars = await _lazyTypePars.Value.ConfigureAwait(false);
                    existingTypePars.Remove(typeId);
                }

                if (_lazyParameters4AllBodies.IsValueCreated)
                {
                    _lazyParameters4AllBodies = CreateAsyncLazy(LoadParametersAndValuesForAllBodiesAsync);
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result == DatabaseOperationExecutor.DatabaseOperationResult.Success && bodies.Any())
        {
            OnItemBodiesChanged(bodies.Select(p => p.itembodyID).ToList());
        }

        return response;
    }

    public async Task RemoveTypeParameterRelation(int parameterID, int typeid)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var typeParAssociationService = new EntityService<typeparassociation>(unitOfWork);
            var itmCechyService = new EntityService<itmparameter>(unitOfWork);
            var toRemove = await typeParAssociationService.GetOneAsync(t => t.typeID == typeid && t.parameterID == parameterID).ConfigureAwait(false);
            if (toRemove != null)
            {
                int removedPos = toRemove.pos;
                await typeParAssociationService.DeleteAsync(toRemove).ConfigureAwait(false);
                var toUpdate = await typeParAssociationService.GetAllAsync(t => t.typeID == typeid && t.pos > removedPos).ConfigureAwait(false);
                foreach (var item in toUpdate)
                {
                    item.pos -= 1;
                }
                await typeParAssociationService.UpdateRangeAsync(toUpdate).ConfigureAwait(false);
                var cialkowe = await itmCechyService.GetAllAsync(p => p.parameterID == parameterID).ConfigureAwait(false);
                await itmCechyService.DeleteRangeAsync(cialkowe).ConfigureAwait(false);

                var cialka = cialkowe.ToDictionary(p => p.itembodyID, q => q);
                foreach (var itema in cialka)
                {
                    items[itema.Key].ItmCechies.RemoveAll(p => p.parameterID == parameterID);
                }
                if (_lazyParameters4AllBodies.IsValueCreated)
                {
                    var lc4a = await _lazyParameters4AllBodies.Value.ConfigureAwait(false);
                    foreach (var itema in cialka)
                    {
                        var inceptionLevelOne = lc4a[itema.Key];
                        var ke = inceptionLevelOne.First(p => p.Key.parameterID == parameterID).Key;
                        inceptionLevelOne.Remove(ke);
                    }
                }
                if (_lazyTypePars.IsValueCreated)
                {
                    var existingTypePars = await _lazyTypePars.Value.ConfigureAwait(false);
                    if (existingTypePars.ContainsKey(typeid))
                    {
                        if (existingTypePars[typeid].Count == 1)
                        {
                            existingTypePars[typeid] = new List<typeparassociation>();
                        }
                        else
                        {
                            existingTypePars[typeid] = (await typeParAssociationService.GetAllAsync(p => p.typeID == typeid).ConfigureAwait(false)).ToList();
                        }
                    }
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result == DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            await _entityEventsService.PublishAsync<type>(new EntityEventArgs([typeid], EntityActionType.Update));
        }
    }

    public async Task RemoveWatchesGrouped(int id)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var watchesGroupedService = new EntityService<watchesgrouped>(unitOfWork);
            var group4watchService = new EntityService<group4watch>(unitOfWork);
            var watchG = await watchesGroupedService.GetAllAsync(p => p.group4watchesID == id).ConfigureAwait(false);
            var watchGIds = watchG.Select(b => b.group4watchesID).ToList();
            await watchesGroupedService.DeleteRangeAsync(watchG).ConfigureAwait(false);
            var g4w = await group4watchService.GetOneAsync(p => p.group4watchesID == id).ConfigureAwait(false);
            await group4watchService.DeleteAsync(g4w).ConfigureAwait(false);
            if (_lazyWatchesGrouped.IsValueCreated)
            {
                var existingWatchesGrouped = await _lazyWatchesGrouped.Value.ConfigureAwait(false);
                existingWatchesGrouped.RemoveAll(b => watchGIds.Contains(b.group4watchesID));
            }
            if (_lazyGroup4Watches.IsValueCreated)
            {
                var existingGroup4Watches = await _lazyGroup4Watches.Value.ConfigureAwait(false);
                existingGroup4Watches.Remove(g4w);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task RemoveWatchFromGroup(int id, int watchid)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var watchesGroupedService = new EntityService<watchesgrouped>(unitOfWork);
            var watch = await watchesGroupedService.GetOneAsync(p => p.group4watchesID == id && p.watchID == watchid).ConfigureAwait(false);
            await watchesGroupedService.DeleteAsync(watch).ConfigureAwait(false);
            if (_lazyWatchesGrouped.IsValueCreated)
            {
                var existingWatchesGrouped = await _lazyWatchesGrouped.Value.ConfigureAwait(false);
                var we = existingWatchesGrouped.FirstOrDefault(p => p.watchesGroupedID == watch.watchesGroupedID);
                existingWatchesGrouped.Remove(we);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task<multidrawer> RenameDrawer(int multidrawerid, string name)
    {
        multidrawer zwrotka = null;
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var MultiDrawerService = new EntityService<multidrawer>(unitOfWork);
            zwrotka = await MultiDrawerService.GetOneAsync(p => p.MultiDrawerID == multidrawerid).ConfigureAwait(false);
            if (zwrotka != null)
            {
                zwrotka.name = name;
                await MultiDrawerService.UpdateAsync(zwrotka).ConfigureAwait(false);
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return null;
        }
        return zwrotka;
    }

    public async Task RenameGroup4Bodies(int id, string name)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var group4bodyService = new EntityService<group4body>(unitOfWork);
            var g4b = await group4bodyService.GetOneAsync(p => p.group4bodiesID == id).ConfigureAwait(false);
            if (g4b != null)
            {
                g4b.name = name;
                await group4bodyService.UpdateAsync(g4b).ConfigureAwait(false);
                if (_lazyGroup4Bodies.IsValueCreated)
                {
                    var existingGroup4Bodies = await _lazyGroup4Bodies.Value.ConfigureAwait(false);
                    var itemToUpdate = existingGroup4Bodies.FirstOrDefault(g => g.group4bodiesID == id);
                    if (itemToUpdate != null)
                    {
                        itemToUpdate.name = name;
                    }
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task RenameGroup4Watches(int id, string name)
    {
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var group4watchService = new EntityService<group4watch>(unitOfWork);
            var g4w = await group4watchService.GetOneAsync(p => p.group4watchesID == id).ConfigureAwait(false);
            if (g4w != null)
            {
                g4w.name = name;
                await group4watchService.UpdateAsync(g4w).ConfigureAwait(false);
                if (_lazyGroup4Watches.IsValueCreated)
                {
                    var existingGroup4watches = await _lazyGroup4Watches.Value.ConfigureAwait(false);
                    var itemToUpdate = existingGroup4watches.FirstOrDefault(g => g.group4watchesID == id);
                    if (itemToUpdate != null)
                    {
                        itemToUpdate.name = name;
                    }
                }
            }
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async Task SaveBodyHeaderCechy(itembody body, itemheader header, List<parametervalue> values)
    {
        itembody cialko = new();
        itemheader hed = new();
        List<itmparameter> itmparameter = new();

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var ItemBodiesService = new EntityService<itembody>(unitOfWork);
            var ItemHeaderService = new EntityService<itemheader>(unitOfWork);
            var ItmParametersService = new EntityService<itmparameter>(unitOfWork);

            await unitOfWork.BeginTransactionAsync();

            cialko = (await ItemBodiesService.GetByIdAsync(body.itembodyID).ConfigureAwait(false)).GetValue<itembody>();
            int oldType = cialko.typeId;
            cialko.typeId = body.typeId;
            cialko.brandID = body.brandID;
            cialko.name = body.name;
            cialko.myname = body.myname;
            cialko.mpn = body.mpn;
            cialko.weight = body.weight;
            await ItemBodiesService.UpdateAsync(cialko, false).ConfigureAwait(false);
            hed = (await ItemHeaderService.GetByIdAsync(header.itemheaderID).ConfigureAwait(false)).GetValue<itemheader>();
            hed.quantity = header.quantity;
            hed.pricePaid = header.pricePaid;
            await ItemHeaderService.UpdateAsync(hed, false).ConfigureAwait(false);

            if (oldType == cialko.typeId)
            {
                itmparameter = (await ItmParametersService.GetAllAsync(p => p.itembodyID == cialko.itembodyID)
                    .ConfigureAwait(false)).ToList();
                foreach (var value in values)
                {
                    var itmcecha = itmparameter.FirstOrDefault(p => p.parameterID == value.parameterID);

                    if (itmcecha == null)
                    {
                        itmcecha = new itmparameter
                        {
                            parameterID = value.parameterID,
                            parameterValueID = value.parameterValueID,
                            itembodyID = body.itembodyID
                        };
                        await ItmParametersService.AddAsync(itmcecha, false).ConfigureAwait(false);
                    }
                    else
                    {
                        itmcecha.parameterValueID = value.parameterValueID;
                    }
                }
            }
            else
            {
                var itmParametersToRemove = await ItmParametersService.GetAllAsync(p => p.itembodyID == cialko.itembodyID)
                    .ConfigureAwait(false);
                await ItmParametersService.DeleteRangeAsync(itmParametersToRemove, false).ConfigureAwait(false);
            }

            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            await unitOfWork.CommitAsync().ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return;
        }
        if (DALState == DatabaseAccessLayerState.Loaded)
        {
            items[body.itembodyID].itembody = cialko;
            items[body.itembodyID].ItemHeaders.RemoveAll(ih => ih.itemheaderID == hed.itemheaderID);
            items[body.itembodyID].ItemHeaders.Add(hed);
        }
        var t = await GetCechy4OneBodyAsync(cialko.itembodyID).ConfigureAwait(false);

        if (_lazyParameters4AllBodies.IsValueCreated)
        {
            var c4ab = await _lazyParameters4AllBodies.Value.ConfigureAwait(false);
            c4ab[cialko.itembodyID] = t;
        }

        items[cialko.itembodyID].ItmCechies = itmparameter;
        OnItemBodiesChanged(new List<int> { body.itembodyID });

     
    }

    public async Task<Dictionary<parameter, parametervalue>> GetCechy4OneBodyAsync(int itembodyid)
    {
        return await ExecuteWithUnitOfWorkAsync<itemitsparametersandvalue, Dictionary<parameter, parametervalue>>(
            async service =>
            {
                return (await service.GetAllAsync(p => p.itembodyID == itembodyid).ConfigureAwait(false)).ToDictionary(p => new parameter
                    {
                        parameterID = p.parameterID,
                        name = p.ParameterName
                    },
                    q => new parametervalue
                    {
                        parameterValueID = (int)q.parameterValueID,
                        parameterID = q.parameterID,
                        name = q.ParameterValueName,
                        pos = (int)q.pos
                    });
            },
            new Dictionary<parameter, parametervalue>()
        ).ConfigureAwait(false);
    }

    public async Task<int> UpdateAddressesAsStrings(Dictionary<int, int> orderIdsBillAddrs, Dictionary<int, string> orderIdsStrings)
    {
        //     List<int> Refreshedorders = new List<int>();

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitofwork = new UnitOfWork(DbContextFactory.GetWriteableContext());
            var billaddrService = new EntityService<billaddr>(unitofwork);
            var adry = await billaddrService.GetAllAsync(p => orderIdsBillAddrs.Values.Contains(p.billaddrID))
                .ConfigureAwait(false);
            foreach (var li in orderIdsBillAddrs)
            {
                var ba = adry.First(p => p.billaddrID == li.Value);
                ba.AddressAsAString = orderIdsStrings[li.Key];
                //     Refreshedorders.Add(li.Key);
            }
            await billaddrService.UpdateRangeAsync(adry).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(4)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return 2;
        }
        return 0;
    }

    public async Task<TResult> ExecuteWithUnitOfWorkAsync<T, TResult>(Func<EntityService<T>, Task<TResult>> operation,
        TResult defaultValue) where T : class, new() where TResult : class
    {
        TResult resultData = defaultValue;

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());

            var service = new EntityService<T>(unitOfWork);
            resultData = await operation(service).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10));

        if (result == DatabaseOperationExecutor.DatabaseOperationResult.Success && resultData != null)
        {
            // Tutaj możesz obsłużyć błąd w sposób specyficzny dla Twojej aplikacji
            // na przykład logowanie błędu, zwrócenie wartości domyślnej lub rzucenie wyjątku.
            return resultData;
        }

        return defaultValue;
    }

    public async Task<Dictionary<int, int>> CountordersForCustomers(List<int> kusty)
    {
        var zwrotka = new Dictionary<int, int>();
        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var orderService = new orderService(unitOfWork);
            zwrotka = await orderService.CountordersForCustomersAsync(kusty).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return new Dictionary<int, int>();
        }
        return zwrotka;
    }

    public async Task<(DateTime oldestorderDate, DateTime newestorderDate)> GetorderDateRangeAsync()
    {
        DateTime oldestDate = new();
        DateTime newestDate = new();

        DatabaseOperationExecutor.DatabaseOperationResult result = await DatabaseOperationExecutor.Instance.ExecuteWithRetryAsync(async () =>
        {
            using var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
            var orderService = new orderService(unitOfWork);
            oldestDate = await orderService.MinAsync(o => true, o => o.paidOn).ConfigureAwait(false);
            newestDate = await orderService.MaxAsync(o => true, o => o.paidOn).ConfigureAwait(false);
        }, TimeSpan.FromSeconds(4)).ConfigureAwait(false);

        if (result != DatabaseOperationExecutor.DatabaseOperationResult.Success)
        {
            return default;
        }
        return (oldestDate, newestDate);
    }

   
}