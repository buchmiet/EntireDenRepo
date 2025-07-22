using AmazonSPAPIClient;
using DataServicesNET80.Extensions;
using DataServicesNET80.Models;
using DataServicesNET80;
using denModels.MarketplaceServices;
using denEbayNET80;
using DataServicesNET80.DatabaseAccessLayer;


namespace denSharedLibrary;

public interface IMarketActions
{
    Task MarkAmazonOrdersAsShipped(List<DataNededToMarkAmazonOrderAsShipped> orders,
        Action<string> pisz, int locationid);

    AmazonInfo AmaOrdExtractor(List<string> we);
    Task<ebayOrders2.Rootobject> FetchEbayOrders2(Action<string> pisz,  int locationID);


    //         Task<bool> MarkAsDispatchAndLeaveFeedback(Dictionary<string, string> OrdersTrackings, Action<string> notify, int locationId);
    Task MarkEbayOrdersAsDispatched(Dictionary<string, Dictionary<string, int>> dobro,
        Dictionary<string, string> trackings, Action<string> pisz,  int locationID);
    Task<bool> MarkEbayOrdersAsDispatched2 (Dictionary<string, string> OrdersTrackings, Action<string> notify, int locationId);
    Task<Dictionary<string, Dictionary<string, int>>> LeaveFeedbackOnEbay(string[] orderIds, Action<string> napisz,  int locationID);
    Task AssignQuantities2Markets(int id, int locationid, int number2reduceby,Action<string>pisz);
    Task AssignQuantities2markets(int id, int locationid,Action<string> pisz);
    Task<UpdateQuantityResponse> UpdateQuantityOnMarketplace(string itm, int quantity,  int locationid, int marketID,Action<string> pisz);
}

public class MarketActions(IDatabaseAccessLayer databaseAccessLayer, IEbayService ebayService,IAmazonSpApi amazonSpApi) : IMarketActions
{

    public async Task MarkAmazonOrdersAsShipped(List<DataNededToMarkAmazonOrderAsShipped> orders,
        Action<string> pisz, int locationid)
    {
        await  amazonSpApi.MarkAmazonOrdersAsShipped(orders,pisz, locationid);
    }


    public AmazonInfo AmaOrdExtractor(List<string> we)
    {
        return amazonSpApi.AmaOrdExtractor(we);
    }

    public async Task<ebayOrders2.Rootobject> FetchEbayOrders2(Action<string> pisz,  int locationID)
    {
        return await ebayService.FetchEbayOrders2(pisz,  locationID).ConfigureAwait(false);
    }


    public Task<bool> MarkEbayOrdersAsDispatched2(Dictionary<string, string> OrdersTrackings, Action<string> notify, int locationId)
    {
        return ebayService.MarkAsDispatchAndLeaveFeedback(OrdersTrackings,notify, locationId);
    }

    public async Task MarkEbayOrdersAsDispatched(Dictionary<string, Dictionary<string, int>> dobro,
        Dictionary<string, string> trackings, Action<string> pisz, int locationID)
    {
        await ebayService.MarkOrdersAsDispatched(dobro, trackings, pisz, locationID);
        //MarkAsDispatchAndLeaveFeedback(dobro, trackings, pisz, locationID);//  MarkOrdersAsDispatched(dobro, trackings, pisz,  locationID);
    }

    public async Task<Dictionary<string, Dictionary<string, int>>> LeaveFeedbackOnEbay(string[] orderIds, Action<string> napisz, int locationID)
    {
        Dictionary<string, Dictionary<string, int>> g =
            await ebayService.DoFeedback(orderIds, napisz, locationID);
        return g;
    }

    public async Task AssignQuantities2Markets(int id, int locationid, int number2reduceby,Action<string>pisz)
    {

        itembody bo;
        List<itemheader> hedy;
        List<itmmarketassoc> itmm;
        token toko = null;

        Dictionary<int, int> soldwithQuantities = new();
        Dictionary<int, string> AmazonMarketplaces = (await databaseAccessLayer.AmazonMarketplaces()).ToDictionary(p => p.marketID, q => q.code);
        var markets2platformnameTranslations = await databaseAccessLayer.GetMarkets2PlatformTypesDictionary().ConfigureAwait(false);

        int qua = 0;
        using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
        {
            var itemHeaderService = new EntityService<itemheader>(unitOfWork);
            var ItmMarketAssocsService = new EntityService<itmmarketassoc>(unitOfWork);
            hedy = (await itemHeaderService.GetAllAsync(p => p.itembodyID == id && p.locationID == locationid)).ToList();
            itmm = (await ItmMarketAssocsService.GetAllAsync(p => p.itembodyID == id && p.locationID == locationid)).ToList();
            qua = hedy.Sum(p => p.quantity);

            int totalReduction = 0;
            bool allItemsZero = false;

            while (totalReduction < number2reduceby && !allItemsZero)
            {
                allItemsZero = true;
                foreach (var item in hedy)
                {
                    if (item.quantity > 0)
                    {
                        item.quantity--;
                        totalReduction++;
                        allItemsZero = false;

                        if (totalReduction >= number2reduceby)
                        {
                            break;
                        }
                    }
                }
            }

            await itemHeaderService.UpdateRangeAsync(hedy);


            foreach (var ita in itmm)
            {
                if (ita.soldWith != null)
                {
                    var qs = (await itemHeaderService.GetAllAsync(p => p.itembodyID == ita.soldWith && p.locationID == locationid)).Select(p => p.quantity).Sum();
                    soldwithQuantities.Add(ita.marketID, qs);
                }
            }
        }
        Dictionary<string, List<string>> asinSkus = ((await databaseAccessLayer.ASINSKUS()).Where(p => p.locationID == locationid).GroupBy(p => p.asin).ToDictionary(p => p.Key, q => q.Select(p => p.sku).ToList()));

            

          

        foreach (var ita in itmm)
        {
            if (!ita.itemNumber.ToLower().Equals("none"))
            {
                int quantity = (qua - number2reduceby) / ita.quantitySold;
                if (markets2platformnameTranslations[ita.marketID].ToLower().Contains("ebay"))
                {

                    if (soldwithQuantities.ContainsKey(ita.marketID))
                    {
                        int qs = (soldwithQuantities[ita.marketID] - number2reduceby) / ita.quantitySold;
                        quantity = Math.Min(quantity, qs);
                    }
                    var o = await ebayService.SetQuantityTo( ita.itemNumber, quantity,locationid).ConfigureAwait(false);
                    if (!o)
                    {
                        await databaseAccessLayer.AddLogEvent("there was an error updating DB", ita.marketID, ita.itembodyID).ConfigureAwait(false);
                    }
                    else
                    {
                        await databaseAccessLayer.AddLogEvent("(errorCheck) quantity on market " + (await databaseAccessLayer.markety())[ita.marketID].name + " changed from " + (qua / ita.quantitySold) + " to " + quantity, ita.marketID, ita.itembodyID).ConfigureAwait(false);
                    }
                }
                if (AmazonMarketplaces.ContainsKey(ita.marketID))
                {
                    foreach (var sku in asinSkus[ita.itemNumber])
                    {
                        await databaseAccessLayer.AddLogEvent("(errorCheck) quantity on market " + (await databaseAccessLayer.markety())[ita.marketID].name + " changed from " + (qua / ita.quantitySold) + " to " + quantity, ita.marketID, ita.itembodyID).ConfigureAwait(false);
                        await amazonSpApi.UpdateQuantityAndGetResponse(quantity, sku, AmazonMarketplaces[ita.marketID], pisz,locationid).ConfigureAwait(false);
                    }
                }
            }
        }

    }

    public async Task<UpdateQuantityResponse> UpdateQuantityOnMarketplace(string itm, int quantity,  int locationid, int marketID,Action<string> pisz)
    {
        var zwrotka = new UpdateQuantityResponse();
        if (itm.ToLower().Equals("none"))
        {
            zwrotka.Status = UpdateQuantityStatus.NotAttempted;
            zwrotka.ItemNumber = itm;
            zwrotka.Response = new ServerResponse
            {
                Code = denLanguageResourses.Resources.MarketplaceAssociationNotInUse,

            };
            return zwrotka;
        }

        var marketplace2platform = (await databaseAccessLayer.MarketPlatformAssociations()).FirstOrDefault(p => p.marketID == marketID);
        if (marketplace2platform == null)
        {
            zwrotka.Status = UpdateQuantityStatus.Error;
            zwrotka.ItemNumber = itm;
            zwrotka.Response = new ServerResponse
            {
                Code = denLanguageResourses.Resources.UnknownMarketplace,

            };
            return zwrotka;
        }
        if (!(await databaseAccessLayer.Platformy()).ContainsKey(marketplace2platform.platformID))
        {
            zwrotka.Status = UpdateQuantityStatus.Error;
            zwrotka.ItemNumber = itm;
            zwrotka.Response = new ServerResponse
            {
                Code = denLanguageResourses.Resources.UnknownSellingPlatform,

            };
            return zwrotka;
        }
        var platform = (await databaseAccessLayer.Platformy())[marketplace2platform.platformID];

        if (platform.name.ToLower().StartsWith("ebay"))
        {
            return await ebayService.SetQuantityToAndGetResponse( itm, quantity,locationid);
        }
        if (platform.name.ToLower().StartsWith("amazon"))
        {

            return await amazonSpApi.UpdateQuantityAndGetResponse(quantity, itm, (await databaseAccessLayer.AmazonMarketplaces()).First(p => p.marketID == marketID).code, pisz,locationid);
        }
        return zwrotka;
    }


    public async Task AssignQuantities2markets(int id, int locationid,Action<string> pisz)
    {

        itembody bo;
        List<itemheader> hedy;
        List<itmmarketassoc> itmm;  

        Dictionary<int, int> soldwithQuantities = new();

        Dictionary<int, string> AmazonMarketplaces = (await databaseAccessLayer.AmazonMarketplaces()).ToDictionary(p => p.marketID, q => q.code);
        var goodMarkets = (await databaseAccessLayer.markety().ConfigureAwait(false)).Where(p => p.Value.IsInUse).Select(p => p.Key);
        var markets2platformnameTranslations = await databaseAccessLayer.GetMarkets2PlatformTypesDictionary().ConfigureAwait(false);
        hedy = databaseAccessLayer.items[id].ItemHeaders;
        itmm = databaseAccessLayer.items[id].ItmMarketAssocs.Where(p => goodMarkets.Contains(p.marketID)).ToList();
        int qua = hedy.Sum(p => p.quantity);
        foreach (var ita in itmm)
        {
            if (ita.soldWith != null)
            {
                soldwithQuantities.Add(ita.marketID, databaseAccessLayer.items[(int)ita.soldWith].ItemHeaders.Select(p => p.quantity).Sum());

            }
        }
        Dictionary<string, List<string>> asinSkus = ((await databaseAccessLayer.ASINSKUS()).Where(p => p.locationID == locationid).GroupBy(p => p.asin).ToDictionary(p => p.Key, q => q.Select(p => p.sku).ToList()));
          
        var toko = await databaseAccessLayer.GetToken(locationid);

        foreach (var ita in itmm)
        {
            if (!ita.itemNumber.ToLower().Equals("none"))
            {
                if (markets2platformnameTranslations[ita.marketID].ToLower().Contains("ebay"))
                {
                    int quantity = qua / ita.quantitySold;
                    if (soldwithQuantities.ContainsKey(ita.marketID))
                    {
                        int qs = soldwithQuantities[ita.marketID] / ita.quantitySold;
                        quantity = Math.Min(quantity, qs);
                    }
                    var o = await ebayService.SetQuantityTo( ita.itemNumber, quantity,locationid).ConfigureAwait(false);
                    if (!o)
                    {
                        await databaseAccessLayer.AddLogEvent("there was an error updating DB", ita.marketID, ita.itembodyID).ConfigureAwait(false);
                    }
                    else
                    {
                        await databaseAccessLayer.AddLogEvent("quantity on market " + (await databaseAccessLayer.markety())[ita.marketID].name + " updated to " + qua / ita.quantitySold, ita.marketID, ita.itembodyID).ConfigureAwait(false);
                    }
                }
                if (AmazonMarketplaces.ContainsKey(ita.marketID))
                {
                    foreach (var sku in asinSkus[ita.itemNumber])
                    {
                        await databaseAccessLayer.AddLogEvent("quantity on market updated to " + qua / ita.quantitySold, ita.marketID, ita.itembodyID).ConfigureAwait(false);
                        await amazonSpApi.UpdateQuantityAndGetResponse(qua / ita.quantitySold, sku, AmazonMarketplaces[ita.marketID], pisz,locationid).ConfigureAwait(false);
                    }
                }
            }
        }

    }

      
}