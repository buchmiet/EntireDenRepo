using DataServicesNET80.Models;

namespace DataServicesNET80.DatabaseAccessLayer;

public partial class DatabaseAccessLayer
{
    private AsyncLazy<List<amazonmarketplace>> _lazyAmazonMarketplaces;
    private AsyncLazy<List<asinsku>> _lazyASINSKUS;
    private AsyncLazy<List<bodiesgrouped>> _lazyBodiesGrouped;
    private AsyncLazy<Dictionary<int, string>> _lazyBrands;
    private AsyncLazy<Dictionary<int, parameter>> _lazyCechy;
    private AsyncLazy<Dictionary<int, Dictionary<parameter, parametervalue>>> _lazyParameters4AllBodies;
    private AsyncLazy<Dictionary<int, List<parametervalue>>> _lazyCechyValues;
    private AsyncLazy<Dictionary<int, colourtranslation>> _lazyColourTranslations;
    private AsyncLazy<Dictionary<string, string>> _lazyCountryCodes;
    private AsyncLazy<List<group4body>> _lazyGroup4Bodies;
    private AsyncLazy<List<group4watch>> _lazyGroup4Watches;
    private AsyncLazy<List<itmparameter>> _lazyItemCechy;
    private AsyncLazy<Dictionary<string, int>> _lazyKrajStrefa;
    private AsyncLazy<Dictionary<int, market>> _lazyMarkety;
    private AsyncLazy<List<mayalsofit>> _lazyMayalsofits;
    private AsyncLazy<List<multidrawer>> _lazyMultiDrawer;
    private AsyncLazy<Dictionary<int, orderitemtype>> _lazyorderItemTypes;
    private AsyncLazy<Dictionary<int, platform>> _lazyPlatformy;
    private AsyncLazy<Dictionary<string, string>> _lazyStatuses;
    private AsyncLazy<Dictionary<int, string>> _lazyStrefyRM;
    private AsyncLazy<Dictionary<int, supplier>> _lazySuppliers;
    private AsyncLazy<Dictionary<int, List<typeparassociation>>> _lazyTypePars;
    private AsyncLazy<Dictionary<int, string>> _lazyTypes;
    private AsyncLazy<Dictionary<string, countryvatrrate>> _lazyCountryVatRates;
    private AsyncLazy<List<watch>> _lazyWatches;
    private AsyncLazy<List<watchesgrouped>> _lazyWatchesGrouped;
    private AsyncLazy<Dictionary<string, string>> _lazyPostageTypes;
    private AsyncLazy<List<marketplatformassociation>> _lazyMarketPlatformAssociation;
    private AsyncLazy<Dictionary<string, currency>> _lazyCurrencies;
    private AsyncLazy<Dictionary<int, decimal>> _lazyVatRates;



    public Task<Dictionary<int, string>> _Brands => _lazyBrands.Value;

    public async Task<Dictionary<int, string>> Brands()
    {
        return await _lazyBrands.Value.ConfigureAwait(false);
    }

    public Task<Dictionary<int, decimal>> _VarRates => _lazyVatRates.Value;

    public async Task<Dictionary<int, decimal>> VatRates()
    {
        return await _VarRates.ConfigureAwait(false);
    }

    public Task<Dictionary<string, currency>> _Currencies => _lazyCurrencies.Value;

    public async Task<Dictionary<string, currency>> Currencies()
    {
        return await _Currencies.ConfigureAwait(false);
    }

    public Task<List<marketplatformassociation>> _MarketPlatformAssociations => _lazyMarketPlatformAssociation.Value;

    public async Task<List<marketplatformassociation>> MarketPlatformAssociations()
    {
        return await _MarketPlatformAssociations.ConfigureAwait(false);
    }

    public Task<List<amazonmarketplace>> _AmazonMarketplaces => _lazyAmazonMarketplaces.Value;

    public async Task<List<amazonmarketplace>> AmazonMarketplaces()
    {
        return await _AmazonMarketplaces.ConfigureAwait(false);
    }

    public Task<List<asinsku>> _ASINSKUS => _lazyASINSKUS.Value;

    public async Task<List<asinsku>> ASINSKUS()
    {
        return await _ASINSKUS.ConfigureAwait(false);
    }

    public Task<List<bodiesgrouped>> _BodiesGrouped => _lazyBodiesGrouped.Value;

    public async Task<List<bodiesgrouped>> bodiesgrouped()
    {
        return await _BodiesGrouped.ConfigureAwait(false);
    }

    public Task<Dictionary<int, parameter>> _cechy => _lazyCechy.Value;

    public async Task<Dictionary<int, parameter>> parameter()
    {
        return await _cechy.ConfigureAwait(false);
    }

    public Task<Dictionary<string, string>> _postageTypes => _lazyPostageTypes.Value;

    public async Task<Dictionary<string, string>> PostageTypes()
    {
        return await _postageTypes.ConfigureAwait(false);
    }

    public Task<Dictionary<int, Dictionary<parameter, parametervalue>>> _cechy4all => _lazyParameters4AllBodies.Value;

    public async Task<Dictionary<int, Dictionary<parameter, parametervalue>>> ParametersAndValuesForAllBodies()
    {
        return await _cechy4all.ConfigureAwait(false);
    }

    public Task<Dictionary<int, List<parametervalue>>> _cechyValues => _lazyCechyValues.Value;

    public async Task<Dictionary<int, List<parametervalue>>> cechyValues()
    {
        return await _cechyValues.ConfigureAwait(false);
    }

    public Task<Dictionary<int, colourtranslation>> _ColourTranslations => _lazyColourTranslations.Value;

    public async Task<Dictionary<int, colourtranslation>> ColourTranslations()
    {
        return await _ColourTranslations.ConfigureAwait(false);
    }

    public Task<List<group4body>> _Group4Bodies => _lazyGroup4Bodies.Value;

    public async Task<List<group4body>> Group4Bodies()
    {
        return await _Group4Bodies.ConfigureAwait(false);
    }

    public Task<List<group4watch>> _Group4Watches => _lazyGroup4Watches.Value;

    public async Task<List<group4watch>> Group4Watches()
    {
        return await _Group4Watches.ConfigureAwait(false);
    }

    public Task<List<itmparameter>> _Itmcechies => _lazyItemCechy.Value;

    public async Task<List<itmparameter>> Itmcechies()
    {
        return await _Itmcechies.ConfigureAwait(false);
    }

    public Task<Dictionary<string, string>> _kantry => _lazyCountryCodes.Value;

    public async Task<Dictionary<string, string>> kantry()
    {
        return await _kantry.ConfigureAwait(false);
    }

    public Task<Dictionary<string, int>> _krajStrefa => _lazyKrajStrefa.Value;

    public async Task<Dictionary<string, int>> krajStrefa()
    {
        return await _krajStrefa.ConfigureAwait(false);
    }

    public Task<Dictionary<int, market>> _markety => _lazyMarkety.Value;

    public async Task<Dictionary<int, market>> markety()
    {
        return await _markety.ConfigureAwait(false);
    }

    public Task<List<mayalsofit>> _mayalsofits => _lazyMayalsofits.Value;

    public async Task<List<mayalsofit>> mayalsofits()
    {
        return await _mayalsofits.ConfigureAwait(false);
    }

    public Task<List<multidrawer>> _MultiDrawer => _lazyMultiDrawer.Value;

    public async Task<List<multidrawer>> multidrawer()
    {
        return await _MultiDrawer.ConfigureAwait(false);
    }

    public Task<Dictionary<int, orderitemtype>> _orderItemTypes => _lazyorderItemTypes.Value;

    public async Task<Dictionary<int, orderitemtype>> orderItemTypes()
    {
        return await _orderItemTypes.ConfigureAwait(false);
    }

    public Task<Dictionary<int, platform>> _platformy => _lazyPlatformy.Value;

    public async Task<Dictionary<int, platform>> Platformy()
    {
        return await _platformy.ConfigureAwait(false);
    }

    public Task<Dictionary<string, string>> _statuses => _lazyStatuses.Value;

    public async Task<Dictionary<string, string>> statuses()
    {
        return await _statuses.ConfigureAwait(false);
    }

    public Task<Dictionary<int, string>> _strefyRM => _lazyStrefyRM.Value;

    public async Task<Dictionary<int, string>> strefyRM()
    {
        return await _strefyRM.ConfigureAwait(false);
    }

    public Task<Dictionary<int, supplier>> _Suppliers => _lazySuppliers.Value;

    public async Task<Dictionary<int, supplier>> Suppliers()
    {
        return await _Suppliers.ConfigureAwait(false);
    }

    public Task<Dictionary<int, List<typeparassociation>>> _TypePars => _lazyTypePars.Value;

    public async Task<Dictionary<int, List<typeparassociation>>> TypePars()
    {
        return await _TypePars.ConfigureAwait(false);
    }

    public Task<Dictionary<int, string>> Types => _lazyTypes.Value;

    public async Task<Dictionary<int, string>> types()
    {
        return await Types.ConfigureAwait(false);
    }

    public Task<Dictionary<string, countryvatrrate>> _CountryVatRates => _lazyCountryVatRates.Value;

    public async Task<Dictionary<string, countryvatrrate>> CountryVatRates()
    {
        return await _CountryVatRates.ConfigureAwait(false);
    }

    public Task<List<watch>> _Watches => _lazyWatches.Value;

    public async Task<List<watch>> Watches()
    {
        return await _Watches.ConfigureAwait(false);
    }

    public Task<List<watchesgrouped>> _WatchesGrouped => _lazyWatchesGrouped.Value;

    public async Task<List<watchesgrouped>> watchesgrouped()
    {
        return await _WatchesGrouped.ConfigureAwait(false);
    }


    public async Task<List<amazonmarketplace>> LoadAmazonMarketplacesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<amazonmarketplace, List<amazonmarketplace>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<amazonmarketplace>()
        ).ConfigureAwait(false);
    }

    public async Task<List<asinsku>> LoadASINSKUSAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<asinsku, List<asinsku>>(
            async service => (await service.GetAllAsync().ConfigureAwait(false)).ToList(),
            new List<asinsku>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<string, currency>> LoadCurrenciesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<currency, Dictionary<string, currency>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(b => b.code, b => b);
            },
            new Dictionary<string, currency>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, string>> LoadBrandsAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<brand, Dictionary<int, string>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(b => b.brandID, b => b.name);
            },
            new Dictionary<int, string>()
        ).ConfigureAwait(false);
    }

    public async Task<List<bodiesgrouped>> LoadBodiesGroupedAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<bodiesgrouped, List<bodiesgrouped>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<bodiesgrouped>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, colourtranslation>> LoadColourTranslationsAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<colourtranslation, Dictionary<int, colourtranslation>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.kodKoloru, p => p);
            },
            new Dictionary<int, colourtranslation>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<string, string>> LoadCountryCodesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<countrycode, Dictionary<string, string>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.code, p => p.name);
            },
            new Dictionary<string, string>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, parameter>> LoadCechyAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<parameter, Dictionary<int, parameter>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.parameterID, p => p);
            },
            new Dictionary<int, parameter>()
        ).ConfigureAwait(false);
    }

    public async Task<List<group4body>> LoadGroup4BodiesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<group4body, List<group4body>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<group4body>()
        ).ConfigureAwait(false);
    }

    public async Task<List<group4watch>> LoadGroup4WatchesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<group4watch, List<group4watch>>(
            async service =>
            {
                return (await service.GetAllAsync()).ToList();
            },
            new List<group4watch>()
        ).ConfigureAwait(false);
    }

    public async Task<List<itmparameter>> LoadItmCechy()
    {
        return await ExecuteWithUnitOfWorkAsync<itmparameter, List<itmparameter>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<itmparameter>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<string, int>> LoadKrajStrefaAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<country2rmass, Dictionary<string, int>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.code, p => p.RMZoneID);
            },
            new Dictionary<string, int>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, market>> LoadMarketyAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<market, Dictionary<int, market>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.marketID, p => p);
            },
            new Dictionary<int, market>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<string, string>> LoadPostageTypes()
    {
        return await ExecuteWithUnitOfWorkAsync<postagetype, Dictionary<string, string>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.code, p => p.name);
            },
            new Dictionary<string, string>()
        ).ConfigureAwait(false);
    }

    public async Task<List<marketplatformassociation>> LoadMarketPlatformAssociation()
    {
        return await ExecuteWithUnitOfWorkAsync<marketplatformassociation, List<marketplatformassociation>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<marketplatformassociation>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, decimal>> LoadVatRates()
    {
        return await ExecuteWithUnitOfWorkAsync<vatrate, Dictionary<int, decimal>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.VATRateID, p => p.Rate);
            },
            new Dictionary<int, decimal>()
        ).ConfigureAwait(false);
    }

    public async Task<List<mayalsofit>> LoadMayalsofitsAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<mayalsofit, List<mayalsofit>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<mayalsofit>()
        ).ConfigureAwait(false);
    }

    public async Task<List<multidrawer>> LoadMultiDrawerAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<multidrawer, List<multidrawer>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<multidrawer>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, orderitemtype>> LoadorderItemTypesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<orderitemtype, Dictionary<int, orderitemtype>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.OrderItemTypeId, p => p);
            },
            new Dictionary<int, orderitemtype>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, platform>> LoadPlatformyAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<platform, Dictionary<int, platform>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.platformID, p => p);
            },
            new Dictionary<int, platform>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<string, string>> LoadStatusesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<orderstatus, Dictionary<string, string>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.code, p => p.name);
            },
            new Dictionary<string, string>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, string>> LoadStrefyRMAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<rmzone, Dictionary<int, string>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.RMZoneId, p => p.Zone);
            },
            new Dictionary<int, string>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, supplier>> LoadSuppliersAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<supplier, Dictionary<int, supplier>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.supplierID, p => p);
            },
            new Dictionary<int, supplier>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, List<typeparassociation>>> LoadTypeParsAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<typeparassociation, Dictionary<int, List<typeparassociation>>>(
            async service =>
            {
                var grouped = (await service.GetAllAsync().ConfigureAwait(false)).GroupBy(t => t.typeID);
                return grouped.ToDictionary(g => g.Key, g => g.OrderBy(p => p.pos).ToList());
            },
            new Dictionary<int, List<typeparassociation>>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, string>> LoadTypesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<type, Dictionary<int, string>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.typeID, p => p.name.ToString());
            },
            new Dictionary<int, string>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<string, countryvatrrate>> LoadVatCountryRatesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<countryvatrrate, Dictionary<string, countryvatrrate>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToDictionary(p => p.code, p => p);
            },
            new Dictionary<string, countryvatrrate>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, List<parametervalue>>> LoadCechyValuesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<parameteranditsvalue, Dictionary<int, List<parametervalue>>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList().GroupBy(p => p.parameterID, p => p).ToDictionary(
                    g => g.Key,
                    g => g.Select(p => new parametervalue
                    {
                        parameterValueID = p.parameterValueID,
                        parameterID = p.parameterID,
                        name = p.valueName,
                        pos = p.pos
                    }).ToList());
            },
            new Dictionary<int, List<parametervalue>>()
        ).ConfigureAwait(false);
    }

    public async Task<List<watch>> LoadWatchesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<watch, List<watch>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<watch>()
        ).ConfigureAwait(false);
    }

    public async Task<List<watchesgrouped>> LoadWatchesGroupedAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<watchesgrouped, List<watchesgrouped>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).ToList();
            },
            new List<watchesgrouped>()
        ).ConfigureAwait(false);
    }

    public async Task<Dictionary<int, Dictionary<parameter, parametervalue>>> LoadParametersAndValuesForAllBodiesAsync()
    {
        return await ExecuteWithUnitOfWorkAsync<itemitsparametersandvalue, Dictionary<int, Dictionary<parameter, parametervalue>>>(
            async service =>
            {
                return (await service.GetAllAsync().ConfigureAwait(false)).GroupBy(p => p.itembodyID).ToDictionary(
                    group => group.Key,
                    group => group.OrderBy(item => item.pos).ToDictionary(
                        item => new parameter { parameterID = item.parameterID, name = item.ParameterName },
                        item => new parametervalue { parameterValueID = (int)item.parameterValueID, name = item.ParameterValueName, parameterID = item.parameterID, pos = (int)item.pos }
                    )
                );
            },
            new Dictionary<int, Dictionary<parameter, parametervalue>>()
        ).ConfigureAwait(false);
    }
}