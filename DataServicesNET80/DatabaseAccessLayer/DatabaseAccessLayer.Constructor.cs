using EntityEvents;

namespace DataServicesNET80.DatabaseAccessLayer;

public partial class DatabaseAccessLayer
{
    public DatabaseAccessLayer()
    {
        DALState = DatabaseAccessLayerState.UnLoaded;
        _lazyCurrencies = CreateAsyncLazy(LoadCurrenciesAsync);
        _lazyBrands = CreateAsyncLazy(LoadBrandsAsync);
        _lazyASINSKUS = CreateAsyncLazy(LoadASINSKUSAsync);
        _lazyAmazonMarketplaces = CreateAsyncLazy(LoadAmazonMarketplacesAsync);
        _lazyCechyValues = CreateAsyncLazy(LoadCechyValuesAsync);
        _lazyCechy = CreateAsyncLazy(LoadCechyAsync);
        _lazyParameters4AllBodies = CreateAsyncLazy(LoadParametersAndValuesForAllBodiesAsync);
        _lazyCountryCodes = CreateAsyncLazy(LoadCountryCodesAsync);
        _lazyStrefyRM = CreateAsyncLazy(LoadStrefyRMAsync);
        _lazyKrajStrefa = CreateAsyncLazy(LoadKrajStrefaAsync);
        _lazyCountryVatRates = CreateAsyncLazy(LoadVatCountryRatesAsync);
        _lazyGroup4Bodies = CreateAsyncLazy(LoadGroup4BodiesAsync);
        _lazyGroup4Watches = CreateAsyncLazy(LoadGroup4WatchesAsync);
        _lazyStatuses = CreateAsyncLazy(LoadStatusesAsync);
        _lazyorderItemTypes = CreateAsyncLazy(LoadorderItemTypesAsync);
        _lazyTypes = CreateAsyncLazy(LoadTypesAsync);
        _lazyWatches = CreateAsyncLazy(LoadWatchesAsync);
        _lazyPlatformy = CreateAsyncLazy(LoadPlatformyAsync);
        _lazySuppliers = CreateAsyncLazy(LoadSuppliersAsync);
        _lazyMarkety = CreateAsyncLazy(LoadMarketyAsync);
        _lazyASINSKUS = CreateAsyncLazy(LoadASINSKUSAsync);
        _lazyColourTranslations = CreateAsyncLazy(LoadColourTranslationsAsync);
        _lazyBodiesGrouped = CreateAsyncLazy(LoadBodiesGroupedAsync);
        _lazyMultiDrawer = CreateAsyncLazy(LoadMultiDrawerAsync);
        _lazyMayalsofits = CreateAsyncLazy(LoadMayalsofitsAsync);
        _lazyWatchesGrouped = CreateAsyncLazy(LoadWatchesGroupedAsync);
        _lazyTypePars = CreateAsyncLazy(LoadTypeParsAsync);
        _lazyItemCechy = CreateAsyncLazy(LoadItmCechy);
        _lazyPostageTypes = CreateAsyncLazy(LoadPostageTypes);
        _lazyMarketPlatformAssociation = CreateAsyncLazy(LoadMarketPlatformAssociation);
        _lazyVatRates = CreateAsyncLazy(LoadVatRates);
    }

    public DatabaseAccessLayer(IEntityEventsService entityEventsService) : this()
    {
        _entityEventsService = entityEventsService;

    }
}