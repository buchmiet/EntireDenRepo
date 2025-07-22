using System.Linq.Expressions;
using Answers;
using DataServicesNET80.Extensions;
using DataServicesNET80.Models;
using denModels.OauthApi;

namespace DataServicesNET80.DatabaseAccessLayer;

public interface IDatabaseAccessLayer


{


    event EventHandler<ItemBodiesChangedEventArgs> ItemBodiesChanged;
    List<bodyinthebox> BodyInTheBoxes { get; set; }
    DatabaseAccessLayerState DALState { get; }
    int colourProperty { get; set; }
    Dictionary<int, AssociatedData> items { get; set; }
    void OnItemBodiesChanged(List<int> changedBodies);
    //   event EventHandler<DatabaseAccessLayer.TypeChangedEventArgs> TypeChanged;
    // void OnTypeChanged(int type);
    // event DatabaseAccessLayer.TypesChangedEventHandler TypesChanged;
    Task<OauthTokenObject> GetEbayToken(int locationid, bool logEachEvent);
    Task<Dictionary<int, decimal>> VatRates();
    Task<Dictionary<string, currency>> Currencies();
    Task<Dictionary<int, string>> Brands();
    Task<List<marketplatformassociation>> MarketPlatformAssociations();
    Task<List<amazonmarketplace>> AmazonMarketplaces();
    Task<List<asinsku>> ASINSKUS();
    Task<List<bodiesgrouped>> bodiesgrouped();
    Task<Dictionary<int, parameter>> parameter();
    Task<Dictionary<string, string>> PostageTypes();
    Task<Dictionary<int, Dictionary<parameter, parametervalue>>> ParametersAndValuesForAllBodies();
    Task<Dictionary<int, List<parametervalue>>> cechyValues();
    Task<Dictionary<int, colourtranslation>> ColourTranslations();
    Task<List<group4body>> Group4Bodies();
    Task<List<group4watch>> Group4Watches();
    Task<List<itmparameter>> Itmcechies();
    Task<Dictionary<string, string>> kantry();
    Task<Dictionary<string, int>> krajStrefa();
    Task<Dictionary<int, market>> markety();
    Task<List<mayalsofit>> mayalsofits();
    Task<List<multidrawer>> multidrawer();
    Task<Dictionary<int, orderitemtype>> orderItemTypes();
    Task<Dictionary<int, platform>> Platformy();
    Task<Dictionary<string, string>> statuses();
    Task<Dictionary<int, string>> strefyRM();
    Task<Dictionary<int, supplier>> Suppliers();
    Task<Dictionary<int, List<typeparassociation>>> TypePars();
    Task<Dictionary<int, string>> types();
    Task<Dictionary<string, countryvatrrate>> CountryVatRates();
    Task<List<watch>> Watches();
    Task<List<watchesgrouped>> watchesgrouped();
    Task<Answer> GetItemBodyId(int itembodyid);
    Task<Answer> GetItemBodyId(string mpn);
    Task UpdateShopItem(shopitem shopitem);
    Task<shopitem> GetShopItemByItembodyId(int itembodyid);
    Task<itembody[]> GetAlsoFitting(int watchID);
    Task<int> GetNumberOfordersWithCondition(Expression<Func<order, bool>> predicate);

    Task<OauthTokenObject> GetQuickBooksToken(int locationid, bool logEachEvent);
    Task<QuickBooksTokenObject> UpdateQuickbooksToken(string Refreshtoken, string token, int locationID, bool logEachEvent);
    Task UpdateEbayToken(string refresh_token, string oauth_token, int locationID, bool logEachEvent);
    Task UpdateInvoiceTXN(invoicetxn invoice);
    Task Updateorder(order order);

    Task<List<Complete>> GetKompletyWithGivenStatusesAndLocation(List<string> statuses, int locationId);
    IAsyncEnumerable<Complete> GetKompletyAsyncStream(List<int> ids);
    Task<List<Complete>> GetKomplety(List<int> ids);
    Task<Answer> GetListOfItemParameters(int itembodyid);
    Task<Complete> GetKomplet(int orderid);
    Task UpdateAmazonToken(string refresh_token, string access_token, int locatioid, bool LogEachEvent);
    Task<OauthTokenObject> GetAmazonToken(int locationId, bool logeachevent);
    Task<List<xrate>> GetXratesRange(DateTime min, DateTime max, List<string> currencies);
    Task RemoveComplete(int orderid);
    Task<Complete> AddComplete(order order, customer customer, billaddr billaddr, List<orderitem> orderitem);
    Task<List<Complete>> GetCurrentAndPreviousDaysKomplety();
    //  Task<List<order>> GetCurrentAndPreviousDayorders();
    Task<Answer> FlipQBInvoiceIdinInvoiceTX(int invoiceTXNID, string? quickbooksinvoiceid = null);
    Task<Dictionary<string, invoicetxn>> PlatformTXN2InvoiceTXN(List<string> platformTXNs);
    Task<List<order>> UpdateorderStatusesAndTrackings(Dictionary<int, (string Status, string Tracking)> orderUpdates);
    Task AddBodies2Group(List<int> itembodies, int group4bodiesID);
    Task<bodyinthebox> AddBodyInTheBox(bodyinthebox bodyinthebox);
    Task AddCasioInvoices(List<casioinvoice> lista);
    Task<casioukcurrentorder> AddCasioUKCurOrd(casioukcurrentorder cuo);
    Task<parameter> AddCecha(string name);
    Task<parametervalue> AddCechaValue(int parameterID, string name);
    Task<ItemBodyDBOperationStatus> AddFreshBody(itembody cialko, itemheader header);
    Task AddSupplier(string name);
    Task<type> AddType(string name);
    Task<group4watch> AddGroup4watch(string name);
    Task AddLogEvent(string loggedText, int marketID, int itembodyId, int? itemheaderId = null);
    Task<mayalsofit> AddMayAlsoFit(int group4bodiesID, int group4watchesID);
    Task<multidrawer> AddMultiDrawer(multidrawer multidrawer);
    Task AddWatch2Group(int watchid, int group4watchesID);
    Task<List<orderData>> GetOrderDataAsync(string currency, Tuple<DateTime, DateTime> period, HashSet<string> countries, int location);
    Task<Dictionary<itembody, itemheader>> AddNewBodz(List<Body2Add> bodiez, int locationID);
    Task<Dictionary<int, string>> GetMarkets2PlatformTypesDictionary();

    Task<itemheader> AddNewItemHeader(int supplierId, decimal pricePaid, int itemBodyId, int quantity, int locationID,
        int xchgrate, int vatrateid, string acquiredcurrency, string purchasecurrency);
    Task AddNewMarket(FullItmMarketAss itema);
    Task<token> GetToken(int locationId);
    Task<asinsku> AddAsinSku(asinsku asssk);
    Task<colourtranslation> AddOrUpdateColourTranslation(colourtranslation ct);
    Task<typeparassociation> AddTypeParameterRelation(int typeid, int parameterID);
    Task<ItemBodyUpdateOperationResult> CheckIfSuchBodyCanBeAdded(string name, string myname, string mpn);
    Task<ItemBodyUpdateOperationResult> CheckIfSuchBodyCanBeSaved(int itembodyid, string name, string myname, string mpn);
    Task CurOrds2BackOrds(List<casioukbackorder> cuoList);
    Task<List<casioinvoice>> FindAmongCasioUKInvoices(string mpn);
    Task<bool> FlipReadyToTrack(int itembodyid);
    Task<List<casioukbackorder>> GetCasioUKBackorders();
    Task UpdateQuantityInCasioBackorder(int id, int quantity);
    Task<Answer> GetInvoiceTxns(List<int> orderIds);
    Task<List<casioukcurrentorder>> GetCasioUKCurrentorders();
    Task GetPackage(int locationId);
    Task<int> IncreaseQuantityInHeader(int itemheaderId, int quantity, string whatBroughtYouHere);
    Task MoveDownCechaValue(int parameterValueID);
    Task MoveUpCechaValue(int parameterValueID);
    Task MoveDownTypePar(int parameterID, int typeid);
    Task MoveUpTypePar(int parameterID, int typeid);
    Task MovRegPhotoUp(int photoid, int itembodyid);
    Task RefreshBodies(List<int> ids);
    Task RefreshBody(int itembodyid, bool refreshNeeded);
    Task RefreshMarket(FullItmMarketAss itema);
    Task RefreshQuantitiesInHeaders(Dictionary<KeyValuePair<int, string>, int> quantities, int itemBodyId, int locationID, bool? flipReady = null);
    Task<casioukcurrentorder> RefreshUKCurOrd(casioukcurrentorder cuo);
    Task RemoveBodiesGrouped(int id);
    Task RemoveBodyFromGroup(int id, int itembodyid);
    Task RemoveBodyInTheBox(bodyinthebox bodyinthebox);
    Task RemoveCasioBackorder(int id);
    Task RemoveCecha(int parameterID);
    Task RemoveParameterValue(int parameterValueID);
    Task RemoveDrawer(int multidrawerid);
    Task RemoveMarket(FullItmMarketAss itema);
    Task RemoveMayAlsoFit(int group4bodiesID, int group4watchesID);
    Task<Answer> RemoveType(int typeId);
    Task RemoveTypeParameterRelation(int parameterID, int typeid);
    Task RemoveWatchesGrouped(int id);
    Task RemoveWatchFromGroup(int id, int watchid);
    Task<multidrawer> RenameDrawer(int multidrawerid, string name);
    Task RenameGroup4Bodies(int id, string name);
    Task RenameGroup4Watches(int id, string name);
    Task SaveBodyHeaderCechy(itembody body, itemheader header, List<parametervalue> values);
    Task<Dictionary<parameter, parametervalue>> GetCechy4OneBodyAsync(int itembodyid);
    Task<int> UpdateAddressesAsStrings(Dictionary<int, int> orderIdsBillAddrs, Dictionary<int, string> orderIdsStrings);
    Task<int> SetQuantityInHeader(int itembodyId, int quantity, string whatBroughtYouHere);
    Task<TResult> ExecuteWithUnitOfWorkAsync<T, TResult>(Func<EntityService<T>, Task<TResult>> operation,
        TResult defaultValue) where T : class, new() where TResult : class;

    Task<Dictionary<int, int>> CountordersForCustomers(List<int> kusty);
    Task<(DateTime oldestorderDate, DateTime newestorderDate)> GetorderDateRangeAsync();
    Task<List<amazonmarketplace>> LoadAmazonMarketplacesAsync();
    Task<List<asinsku>> LoadASINSKUSAsync();
    Task<Dictionary<int, string>> LoadBrandsAsync();
    Task<List<bodiesgrouped>> LoadBodiesGroupedAsync();
    Task<Dictionary<int, colourtranslation>> LoadColourTranslationsAsync();
    Task<Dictionary<string, string>> LoadCountryCodesAsync();
    Task<Dictionary<int, parameter>> LoadCechyAsync();
    Task<List<group4body>> LoadGroup4BodiesAsync();
    Task<List<group4watch>> LoadGroup4WatchesAsync();
    Task<List<itmparameter>> LoadItmCechy();
    Task<Dictionary<string, int>> LoadKrajStrefaAsync();
    Task<Dictionary<int, market>> LoadMarketyAsync();
    Task<Dictionary<string, string>> LoadPostageTypes();
    Task<List<marketplatformassociation>> LoadMarketPlatformAssociation();
    Task<List<mayalsofit>> LoadMayalsofitsAsync();
    Task<List<multidrawer>> LoadMultiDrawerAsync();
    Task<Dictionary<int, orderitemtype>> LoadorderItemTypesAsync();
    Task<Dictionary<int, platform>> LoadPlatformyAsync();
    Task<Dictionary<string, string>> LoadStatusesAsync();
    Task<Dictionary<int, string>> LoadStrefyRMAsync();
    Task<Dictionary<int, supplier>> LoadSuppliersAsync();
    Task<Dictionary<int, List<typeparassociation>>> LoadTypeParsAsync();
    Task<Dictionary<int, string>> LoadTypesAsync();
    Task<Dictionary<string, countryvatrrate>> LoadVatCountryRatesAsync();
    Task<Dictionary<int, List<parametervalue>>> LoadCechyValuesAsync();
    Task<List<watch>> LoadWatchesAsync();
    Task<List<watchesgrouped>> LoadWatchesGroupedAsync();
    Task<Dictionary<int, Dictionary<parameter, parametervalue>>> LoadParametersAndValuesForAllBodiesAsync();
    Task<group4body> AddGroup4Body(string name);

    Task<QuickBooksTokenObject> UpdateQuickbooksTokenNew(string refreshToken, string token, int locationId,
        bool logEachEvent);

    Task<OauthTokenObject> UpdateTokenAsync(int locationId, string serviceName,
        string accessToken = null, string refreshToken = null,
        bool logEachEvent = false);

    Task<OauthTokenObject> GetTokenForServiceAsync(int locationId, string serviceName);
}