using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80;
using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Models;

using denMethods;
using denModels;
using denSharedLibrary;
using EntityEvents;
using Printers;
using SettingsKeptInFile;
using shookayNET;
using static denViewModels.ConfirmDecimalValueViewModel;


namespace denViewModels.ProductBrowser.ProBro;

public partial class ProBroViewModel : ObservableObject
{
    private ISettingsService SettingsService;
    private IPrintersService _printersService;
    public ProBroViewModel(IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer, IFileDialogService fileDialogService,
        IDispatcherService dispatcherService, IMarketActions marketActions, IEntityEventsService entityEventsService,
        ISettingsService settingsService,IPrintersService printersService)
    {
        _printersService = printersService;
        SettingsService = settingsService;
        //ObservableCollection<Idname> availableSuppliers,
        LocationId = SettingsService.LocationId;
        DatabaseAccessLayer = databaseAccessLayer;
        _fileDialogService = fileDialogService;
        _dispatcherService = dispatcherService;
        SetColumnsWidths();
        ProductItems = [];
        Photos = [];
        AddProductCommand = new AsyncRelayCommand(ExecuteAddProduct, () => !IsBusy);
        AddNewMarketCommand = new AsyncRelayCommand(ExecuteAddNewMarketCommand, () => !IsBusy);
        SaveProductCommand = new AsyncRelayCommand(ExecuteSaveProduct);
        TurnTrackOnOffCommand = new AsyncRelayCommand(ExecuteTurnTrackOnOff);
        AssignNewSupplierCommand = new AsyncRelayCommand(ExecuteAssignNewSupplier);
        PrintLabelCommand = new AsyncRelayCommand(ExecutePrintLabel);
        ViewLogsCommand = new AsyncRelayCommand(ExecuteViewLogs);
        AddSupplierCommand = new AsyncRelayCommand(ExecuteAddSupplier);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        RowClickCommand = new RelayCommand<ProductViewItem>(RowClick, CanExecuteRowClick);
        SaveImageCommand = new AsyncRelayCommand<imgWithName>(SaveImageAsync);
        DeleteLogoCommand = new AsyncRelayCommand(ExecuteDeleteLogoPhoto);
        UploadLogoCommand = new AsyncRelayCommand(AddLogoPhoto);
        UploadPackageCommand = new AsyncRelayCommand(AddPackagePhoto);
        DeletePackageCommand = new AsyncRelayCommand(ExecuteDeletePackagePhoto);
        UploadRegPhotoCommand = new AsyncRelayCommand(AddRegPhoto);
        PreviousCommand = new RelayCommand(ExecuteHistoryBack);
        NextCommand = new RelayCommand(ExecuteHistoryForward);
        FilterMenuCommand = new AsyncRelayCommand(FilterMenuExecute); //ItemBodiesChangedCommand
        DatabaseAccessLayer.ItemBodiesChanged += async (_, e) => await ItemBodiesChangedCommand.ExecuteAsync(e); //HandleDataChanged;
        //  DatabaseAccessLayer.TypeChanged += async (_, e) => await PopulateCechyValuesCommand.ExecuteAsync(e.Type);
        entityEventsService.Subscribe<type>(this, TypeChangedHandler);
        //DatabaseAccessLayer.TypesChanged += PropertyValueChanged;
        LeftArrowCommand = new RelayCommand(ExecuteLeftArrowCommand);
        RightArrowCommand = new RelayCommand(ExecuteRightArrowCommand);
        _dialogService = dialogService;
        _saveTimer = new System.Timers.Timer(5000); // 5000 ms = 5 sekundy
        _saveTimer.Elapsed += OnTimerElapsed;
        _saveTimer.AutoReset = false; // Uruchom tylko raz po każdym starcie
        _activityTaskWrapper = new ActivityTaskWrapper(ActivityViewModels, _dispatcherService);
        PopulateCechyValuesCommand = new AsyncRelayCommand<int>(PopulateCechyValuesAsync);
        AddAssociatedMarketsCommand = new AsyncRelayCommand(AddAssociatedMarkets);
        HandleFilterEventCommand = new AsyncRelayCommand<FilterEventArgs>(HandleFilterEvent);
        OnSearchChangedCommand = new AsyncRelayCommand(OnSearchChanged);
        ItemBodiesChangedCommand = new AsyncRelayCommand<ItemBodiesChangedEventArgs>(HandleItemBodiesChanged);
        DownloadPhotosIfNecessaryCommand = new AsyncRelayCommand<CancellationToken>(DownloadPhotosIfNecessary);
        UpdateQuantitiesOnMarketsCommand = new AsyncRelayCommand(UpdateQuantitiesOnMarketsExecute);
        SetPriceOnTheWebsiteCommand = new AsyncRelayCommand(SetPriceOnTheWebsiteExecute);
        _vatRate = 20;
        _marketActions = marketActions;
        ////   _availableSuppliers = availableSuppliers;
        //   _logoImage = logoImage;
        //   _packageImage = packageImage;
        //   _productItems = productItems;
        //   _searchByName = searchByName;
        //   _selectedItem = selectedItem;
        //   _selectedSupplier = selectedSupplier;
        //   _brandvm = brandvm;
        //   _typevm = typevm;
        //   _searchEngine = searchEngine;
        //   MyProductFiltersPack = myProductFiltersPack;
        //   AdvancedSearchCommand = advancedSearchCommand;
        //   ChangeSupplierCommand = changeSupplierCommand;
        //   ClearSearchResultsCommand = clearSearchResultsCommand;
        //   DeleteProductCommand = deleteProductCommand;
        //   KeyDownCommand = keyDownCommand;
        //   _photos = photos;
        //   PublishProductCommand = publishProductCommand;
        //   RemoveSupplierCommand = removeSupplierCommand;
        //   UpdateQuantityForSeveralCommand = updateQuantityForSeveralCommand;
        //   _filterForProducts = filterForProducts;
    }

    public async Task TypeChangedHandler(EntityEventArgs args, object sender)
    {
        switch (args.EntityActionType)
        {
            case EntityActionType.Delete:
                break;

            case EntityActionType.Update or EntityActionType.Add:
                var refreshedTypes = (await DatabaseAccessLayer.types())
                    .Where(p => args.EntityIds.Contains(p.Key))
                    .ToList();

                foreach (var refreshedType in refreshedTypes)
                {
                    var existingType = _typevm.Values.FirstOrDefault(p => p.Id == refreshedType.Key);
                    if (existingType is not null)
                    {
                        existingType.Name = refreshedType.Value;
                    }
                    else
                    {
                        _typevm.Values.Add(new Idname(refreshedType.Key, refreshedType.Value));
                    }
                }
                break;
        }

    }


    public async Task SetPriceOnTheWebsiteExecute()
    {
        var shopItem = await DatabaseAccessLayer.GetShopItemByItembodyId(SelectedItem.Id);
        var updatePriceOnTheWebsiteDialog = new ConfirmDecimalValueViewModel(shopItem.price);
        updatePriceOnTheWebsiteDialog.RequestClose += async (sender, e) =>
        {
            if (e is ConfirmDecimalValueEventArgs resultEventArgs && resultEventArgs.Result != shopItem.price)
            {
                shopItem.price = resultEventArgs.Result;
                await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.UpdateShopItem(shopItem), denLanguageResourses.Resources.RefreshingProductDetails);

            }
        };
        await _dialogService.ShowDialog(updatePriceOnTheWebsiteDialog);
    }


    public async Task UpdateQuantitiesOnMarketsExecute()
    {
        int qua = DatabaseAccessLayer.items[SelectedItem.Id].ItemHeaders.Sum(p => p.quantity);
        var updateqvm = new SyncQuantitiesWithMarketPlacesViewModel(SelectedItem.Id, qua, LocationId, DatabaseAccessLayer, _dialogService, _dispatcherService, _marketActions);
        updateqvm.RequestClose += async (sender, e) =>
        {

        };
        await _dialogService.ShowDialog(updateqvm);
    }

       

    public static class FilterProductsButton
    {
        public static string InitialText => denLanguageResourses.Resources.InitialText;
        public static string CloseText => denLanguageResourses.Resources.CloseText;

    }

       

    public async Task FilterMenuExecute()
    {
        if (FilteringProducts)
        {
            FilterButtonText = FilterProductsButton.InitialText;
            FilteringProducts = false;
            FilterForProducts.FiltersChanged -= async (sender, e) => await HandleFilterEventCommand.ExecuteAsync(e);
            return;
        }
        FilteringProducts = true;
        FilterButtonText = FilterProductsButton.CloseText;

        var suppliers = (await DatabaseAccessLayer.Suppliers()).Values.ToDictionary(p => p.supplierID, q => q.name);
        var markets = await DatabaseAccessLayer.markety();
        var types = await DatabaseAccessLayer.types();
        MyProductFiltersPack = new ProductFiltersPack
        {
            Suppliers = suppliers.Select(p => p.Key).ToList(),
            Markets = markets.Select(p => p.Key).ToList(),
            Types = types.Select(p => p.Key).ToList()
        };

        var tempFilter = new FilterProductsViewModel
        (
            suppliers,
            markets.ToDictionary(p => p.Key, q => q.Value.name),
            types,
            _dialogService
        );
        FilterForProducts = tempFilter;
        FilterForProducts.FiltersChanged += async (sender, e) => await HandleFilterEventCommand.ExecuteAsync(e);
    }

        




    public async Task HandleFilterEvent(FilterEventArgs filterEventArgs)
    {
        MyProductFiltersPack = new ProductFiltersPack
        {
            Suppliers = filterEventArgs.Suppliers,
            Markets = filterEventArgs.Markets,
            Types = filterEventArgs.Types
        };
        await OnSearchChanged();
    }

      


    public async void PropertyValueChanged(List<int> typesid)
    {
        if (SelectedItem == null)
        {
            return;
        }

        var selectedItemType = DatabaseAccessLayer.items[SelectedItem.Id].itembody.typeId;

        if (typesid.Contains(selectedItemType))
        {
            await PopulateCechyValuesCommand.ExecuteAsync(-1);
        }
    }

   

     


    public void HandleSearchHistoryAfterSelectedItemChanged()
    {
        if (!string.IsNullOrEmpty(SearchByName))
        {
            if (_currentIndex == -1)
            {
                SearchHistory.Add(SearchByName);
                _currentIndex = 0;
                CanHistoryForward = false;
            }
            else
            {
                if (!SearchByName.Equals(Enumerable.ElementAt<string>(SearchHistory, (int) _currentIndex)))
                {
                    if (SearchHistory.Count - 1 > _currentIndex)
                    {
                        _searchHistory.RemoveRange(_currentIndex, SearchHistory.Count - _currentIndex);
                        SearchHistory.Add(SearchByName);
                        CanHistoryForward = false;
                    }
                    else
                    {
                        SearchHistory.Add(SearchByName);
                        _currentIndex++;
                    }
                    if (SearchHistory.Count > 1)
                    {
                        CanHistoryBack = true;
                    }
                }
            }
        }
    }


      

       


        

    public ProductViewItem SelectedItem
    {
        get => _selectedItem;
        set
        {

            if (value is null)
            {
                return;
            }
            SetProperty(ref _selectedItem, value);
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            DownloadPhotosIfNecessaryCommand.Execute(_cts.Token);
            HandleSearchHistoryAfterSelectedItemChanged();
            IsSupplierComboBoxEnabled = true;
            PopulateCechyValuesCommand.ExecuteAsync(-1);
            PopulateTextFields();
            var dx = GetAssignedSuppliers(value.Id);
            AvailableSuppliers =new ObservableCollection<Idname>(dx
                .OrderByDescending(tuple => tuple.Item2) // Sortowanie po quantity (Item2 to element int w krotce)
                .Select(tuple => tuple.Item1));  // Wybieramy tylko Idname (Item1 z krotki)
            // quality of life improvement
            // select first supplier whose quantity is not 0

            SelectedSupplier = AvailableSuppliers.First();
            AddAssociatedMarketsCommand.ExecuteAsync(null);

            var pri = SettingsService.GetSetting("label_printer");
             
            CanPrintLabels = pri is not null;


        }
    }

       

    public void PopulateTextFields()
    {
        var value = SelectedItem;
        SimpleDescription.Items.Clear();

        var textItemVm = new TextFieldViewModel
        {
            FieldName = denLanguageResourses.Resources.FullName,
            InitialValue = value.FullName,
            SelectedValue = value.FullName,
            FieldIdentifier = "FullName"
        };
        textItemVm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "HasChanged")
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
            }
        };
        SimpleDescription.Items.Insert(0, textItemVm);

        textItemVm = new TextFieldViewModel
        {
            FieldName = denLanguageResourses.Resources.ShortName,
            InitialValue = value.MyName,
            SelectedValue = value.MyName,
            FieldIdentifier = "ShortName"
        };
        textItemVm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "HasChanged")
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
            }
        };
        SimpleDescription.Items.Insert(1, textItemVm);

        textItemVm = new TextFieldViewModel
        {
            FieldName = "MPN",
            InitialValue = value.Mpn,
            SelectedValue = value.Mpn,
            FieldIdentifier = "MPN"
        };
        textItemVm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "HasChanged")
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
            }
        };
        SimpleDescription.Items.Insert(2, textItemVm);

        var brands = new List<Idname>(Enumerable.Select<KeyValuePair<int, string>, Idname>(_brandsSuppliersMarkets.Brands, kvp => new Idname ( kvp.Key,  kvp.Value )));
        _brandvm = new ComboBoxViewModel
        {
            FieldName = denLanguageResourses.Resources.Brand,
            Values = brands,
            InitialValue = brands.First(p => p.Id == DatabaseAccessLayer.items[SelectedItem.Id].itembody.brandID),
            FieldType = FieldType.ComboBox,
            FieldIdentifier = "Brand"
        };
        _brandvm.SelectedValue = _brandvm.InitialValue;
        _brandvm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "HasChanged")
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
            }
        };
        SimpleDescription.Items.Insert(3, _brandvm);
        var types = new List<Idname>(_brandsSuppliersMarkets.Types.Select<KeyValuePair<int, string>, Idname>(kvp => new Idname (  kvp.Key, kvp.Value )));
        _typevm = new ComboBoxViewModel
        {
            FieldName = denLanguageResourses.Resources.Type,
            Values = types,
            InitialValue = types.First(p => p.Id == DatabaseAccessLayer.items[SelectedItem.Id].itembody.typeId),
            FieldType = FieldType.ComboBox,
            FieldIdentifier = "Type"
        };
        _typevm.SelectedValue = _typevm.InitialValue;
        _typevm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "HasChanged")
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
            }
        };
        SimpleDescription.Items.Insert(4, _typevm);


        textItemVm = new TextFieldViewModel
        {
            FieldName = denLanguageResourses.Resources.Weight,
            InitialValue = value.Weight.ToString(),
            SelectedValue = value.Weight.ToString(),
            FieldIdentifier = "Weight"
        };
        textItemVm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "HasChanged")
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
            }
        };
        SimpleDescription.Items.Insert(5, textItemVm);

        if (!string.IsNullOrWhiteSpace(value.LocatedAt))
        {
            textItemVm = new TextFieldViewModel
            {
                FieldName = denLanguageResourses.Resources.Location,
                InitialValue = value.LocatedAt,
                SelectedValue = value.LocatedAt,

                FieldType = FieldType.Location,
                FieldIdentifier = "Location"
            };
            SimpleDescription.Items.Add(textItemVm);
        }
    }


    private async Task PopulateCechyValuesAsync(int typeID)
    {
        if (SelectedItem == null)
        {
            return;
        }
        if (typeID != -1)
        {
            if (DatabaseAccessLayer.items[SelectedItem.Id].itembody.typeId != typeID)
            {
                return;
            }
        }

        _dispatcherService.Invoke(() =>
        {
            CechyItems.Clear();
        });

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));

        var tro = await DatabaseAccessLayer.GetCechy4OneBodyAsync(SelectedItem.Id);



        foreach (var cechaPair in tro)
        {
            var cechaItemVM = new ParameterItemViewModel
            {
                CechaName = (await DatabaseAccessLayer.parameter())[cechaPair.Key.parameterID].name,  // nazwa cechy


                DostepneCechyValues = (await DatabaseAccessLayer.cechyValues())[cechaPair.Key.parameterID].OrderBy(p => p.pos).ToList()
            };

            if (cechaPair.Value.parameterValueID == -1)
            {
                cechaItemVM.SelectedCechaValue = ParameterItemViewModel.NotSetCechaValue;
            }
            else
            {
                cechaItemVM.SelectedCechaValue = (await DatabaseAccessLayer.cechyValues())[cechaPair.Key.parameterID].First(p => p.parameterValueID == cechaPair.Value.parameterValueID);
            }
            cechaItemVM.InitialCechaValue = cechaItemVM.SelectedCechaValue;
            cechaItemVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanged")
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
                }
            };
            _dispatcherService.Invoke(() =>
            {
                CechyItems.Add(cechaItemVM);
            });
        }

    }







    public List<(Idname, int)> GetAssignedSuppliers(int body)
    {
        var cialko = DatabaseAccessLayer.items[body];

        if (cialko.ItemHeaders == null)
        {
            return new List<(Idname, int)>();
        }

        return cialko.ItemHeaders
            .Where(header => _brandsSuppliersMarkets.Suppliers.ContainsKey(header.supplierID))
            .Select(header =>
                (
                    new Idname (header.supplierID,_brandsSuppliersMarkets.Suppliers[header.supplierID]) ,
                    header.quantity
                )
            )
            .ToList();
    }



    public async Task AddAssociatedMarkets()
    {
        AssociatedMarkets.Clear();
        foreach (var item in DatabaseAccessLayer.items[_selectedItem.Id].ItmMarketAssocs.Where(p => !p.itemNumber.ToLower().Equals("none")))
        {
            var numberOfAmazonMarkets = (await DatabaseAccessLayer.ASINSKUS()).Where(p => p.asin.Equals(item.itemNumber)).ToList();
            if (numberOfAmazonMarkets.Count > 0)
            {
                foreach (var g in numberOfAmazonMarkets)
                {
                    var fuu = new FullItmMarketAss
                    {
                        itmmarketassoc = item,
                        SKU = g
                    };
                    AssociatedMarkets.Add(new AssociatedMarketsViewModel(fuu, DatabaseAccessLayer)
                    {
                        EditCommand = new AsyncRelayCommand(() => ExecuteEditMarket(fuu)),
                        RemoveCommand = new AsyncRelayCommand(() => RemoveMarket(fuu))
                    });
                }
            }
            else
            {
                var fuu = new FullItmMarketAss
                {
                    itmmarketassoc = item,
                    SKU = null
                };
                AssociatedMarkets.Add(new AssociatedMarketsViewModel(fuu, DatabaseAccessLayer)
                {
                    EditCommand = new AsyncRelayCommand(() => ExecuteEditMarket(fuu)),
                    RemoveCommand = new AsyncRelayCommand(() => RemoveMarket(fuu))
                });
            }
        }
    }


      

    public void AddValuesFromHeader(itemheader header)
    {
        if (Enumerable.FirstOrDefault<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldIdentifier.Equals("pricepaidOriginal")) is not TextFieldViewModel priceVm)
        {
            priceVm = new TextFieldViewModel
            {

                FieldIdentifier = "pricepaidOriginal"
            };
            priceVm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanged")
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
                }
            };
            SimpleDescription.Items.Add(priceVm);
        }
        priceVm.FieldName = string.Format(denLanguageResourses.Resources.PricePaidWithCurrencyExVat, header.purchasecurrency);


        priceVm.InitialValue = header.pricePaid.ToString();
        priceVm.SelectedValue = priceVm.InitialValue;

        decimal actualPrice = header.pricePaid * header.xchgrate * (1 + _vatRate / 100);


        if (Enumerable.FirstOrDefault<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldIdentifier.Equals("priceAcquired")) is not TextFieldViewModel actualpriceVM)
        {
            actualpriceVM = new TextFieldViewModel
            {
                FieldName = string.Format(denLanguageResourses.Resources.PricePaidWithCurrencyIncVat, header.purchasecurrency),
                FieldIdentifier = "priceAcquired",
                FieldType = FieldType.TextBlock12
            };
            actualpriceVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanged")
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
                }
            };
            SimpleDescription.Items.Add(actualpriceVM);
        }
        else
        {
            actualpriceVM.FieldName = string.Format(denLanguageResourses.Resources.PricePaidWithCurrencyIncVat, CurrencyName);
        }
        actualpriceVM.InitialValue = actualPrice.ToString();
        actualpriceVM.SelectedValue = actualpriceVM.InitialValue;

        if (Enumerable.FirstOrDefault<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldIdentifier.Equals("quantity")) is not TextFieldViewModel quantiutyVM)
        {
            quantiutyVM = new TextFieldViewModel
            {
                FieldName = denLanguageResourses.Resources.Quantity,
                FieldIdentifier = "quantity"
            };
            quantiutyVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanged")
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
                }
            };
            SimpleDescription.Items.Add(quantiutyVM);
        }

        quantiutyVM.InitialValue = header.quantity.ToString();
        quantiutyVM.SelectedValue = quantiutyVM.InitialValue;


        var sumVm = Enumerable.FirstOrDefault<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldIdentifier.Equals("SumInStock")) as TextFieldViewModel;
        if (sumVm != null)
        {

            sumVm.InitialValue = DatabaseAccessLayer.items[SelectedItem.Id].ItemHeaders.Sum(p => p.quantity).ToString();
            sumVm.SelectedValue = sumVm.InitialValue;
        }
        else
        {
            sumVm = new TextFieldViewModel
            {
                FieldName = denLanguageResourses.Resources.InStockFromAllSuppliers,
                FieldType = FieldType.TextBlock24,
                InitialValue = DatabaseAccessLayer.items[SelectedItem.Id].ItemHeaders.Sum(p => p.quantity).ToString(),
                FieldIdentifier = "SumInStock"
            };
            sumVm.SelectedValue = sumVm.InitialValue;
            sumVm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanged")
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
                }
            };
            SimpleDescription.Items.Add(sumVm);
        }
        var nazwaPola = string.Format(denLanguageResourses.Resources.MarkupEqualsWithThesePrices, Environment.NewLine);
        var wybraneWartosci = new List<string>
        {
            "0%\r" + MarkUpPrices.GetZeroPercentMarkupPrice(CurrencySymbol, actualPrice),
            "20%\r" + MarkUpPrices.Get20PercentMarkupPrice(CurrencySymbol, actualPrice),
            "30%\r" + MarkUpPrices.Get30PercentMarkupPrice(CurrencySymbol, actualPrice)
        };
        var initialValueCollection = new ObservableCollection<string>(wybraneWartosci);

        if (Enumerable.FirstOrDefault<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldType.Equals(FieldType.UniformGrid)) is UniformGridFieldViewModel textFieldVm)
        {
            textFieldVm.FieldName = nazwaPola;
            textFieldVm.InitialValue = initialValueCollection;
            textFieldVm.SelectedValue = initialValueCollection;
        }
        else
        {
            var newItem = new UniformGridFieldViewModel
            {
                FieldName = nazwaPola,
                FieldType = FieldType.UniformGrid,
                InitialValue = initialValueCollection,
                SelectedValue = initialValueCollection
            };
            newItem.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanged")
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
                }
            };
            SimpleDescription.Items.Add(newItem);
        }
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreAnyFieldsModified)));
    }

    public void ExecuteHistoryBack()
    {
        _currentIndex--;
        SearchByName = Enumerable.ElementAt<string>(SearchHistory, (int) _currentIndex);
        if (_currentIndex == 0) { CanHistoryBack = false; }
        CanHistoryForward = true;
    }

    public void ExecuteHistoryForward()
    {
        _currentIndex++;
        SearchByName = Enumerable.ElementAt<string>(SearchHistory, (int) _currentIndex);
        if (_currentIndex == SearchHistory.Count - 1)
        {
            CanHistoryForward = false;
        }

        CanHistoryBack = true;
    }

    public async Task HandleItemBodiesChanged(ItemBodiesChangedEventArgs e)
    {
        if (IsDataLoaded)
            await _activityTaskWrapper.ExecuteTaskAsync(LoadSomeData(e.ChangedBodies),
                denLanguageResourses.Resources.UpdatingData);
    }


   

      

    public void ShowProduct(int id)=> SelectedItem = Enumerable.First<ProductViewItem>(ProductItems, p => p.Id == id);
        

    private bool CanExecuteRowClick(ProductViewItem item)
    {
        // Tę funkcję można dostosować, jeśli chcesz kontrolować, kiedy polecenie jest dostępne.
        // W tym przykładzie polecenie jest zawsze dostępne, o ile DataViewItem nie jest null.
        return item != null;
    }

    private async Task ExecuteAddNewMarketCommand()
    {
        var marketki = (await DatabaseAccessLayer.markety()).Where(p => p.Key != 2 && p.Key != 8 && p.Key != 9)
            .Select(s => new Idname(s.Key, s.Value.name)).ToList();


        var itema = new FullItmMarketAss { itmmarketassoc = new itmmarketassoc { itembodyID = SelectedItem.Id, itmmarketassID = -1 } };
        var addNewMarket = new AddMarketViewModel(marketki, _dialogService, DatabaseAccessLayer, LocationId, itema);
        addNewMarket.RequestClose += async (sender, e) =>
        {
            var result = ((AddMarketViewModel)sender).Result;
            if (result != null)
            {
                result.SKU.locationID = LocationId;
                await DatabaseAccessLayer.AddNewMarket(result);
                await AddAssociatedMarkets();
            }
        };
        await _dialogService.ShowDialog(addNewMarket);
    }

    private async Task ExecuteAddProduct()
    {
        var addNewProductViewModel = new AddNewProductViewModel(

            (await DatabaseAccessLayer.Suppliers()).Values.Select(s => new Idname(s.supplierID, s.name)).ToList(),
            (await DatabaseAccessLayer.Brands()).Select(s => new Idname(s.Key, s.Value)).ToList(),
            (await DatabaseAccessLayer.types()).Select(s => new Idname(s.Key, s.Value)).ToList(),

            _dialogService, DatabaseAccessLayer);
        addNewProductViewModel.RequestClose += async (sender, e) =>
        {
            Dictionary<string, object> result = ((AddNewProductViewModel)sender).Result;
            if (result != null)
            {
                var os = await StringsToBodyAndHeader(result);
                var result2 = await DatabaseAccessLayer.AddFreshBody(os.Item1, os.Item2);
                if (result2 == ItemBodyDBOperationStatus.OperationSuccessful)
                {
                    var message = string.Format(denLanguageResourses.Resources.ProductAddedMessage, result["ShortName"].ToString());
                    await _dialogService.ShowMessage(denLanguageResourses.Resources.ProductAddedTitle, message);
                }
            }
        };
        await _dialogService.ShowDialog(addNewProductViewModel);
    }

    public async Task<Tuple<itembody, itemheader>> StringsToBodyAndHeader(Dictionary<string, object> wejscie)
    {
        var cialko = new itembody
        {
            name = wejscie["Name"].ToString(),
            myname = wejscie["ShortName"].ToString(),
            mpn = wejscie["MPN"].ToString(),
            brandID = (wejscie["SelectedBrand"] as Idname).Id,
            typeId = (wejscie["SelectedType"] as Idname).Id,
            weight = Convert.ToInt32(wejscie["Weight"]),

        };
        var supplier = (await DatabaseAccessLayer.Suppliers())[(wejscie["SelectedSupplier"] as Idname).Id];
        var xchgrate = await Xrates.getXrate(DateTime.Today, supplier.currency);
        var hedzik = new itemheader
        {
            itembodyID = cialko.itembodyID,
            locationID = LocationId,
            pricePaid = Convert.ToDecimal(wejscie["Price"]),
            purchasedOn = DateTime.Now,
            supplierID = supplier.supplierID,
            acquiredcurrency = supplier.currency,
            xchgrate = xchgrate,
            VATRateID = 1
        };
        return new Tuple<itembody, itemheader>(cialko, hedzik);
    }

    private async Task ExecuteAddSupplier()
    {
        var addNewSupplierViewModel = new AddSupplierViewModel(_dialogService);
        addNewSupplierViewModel.RequestClose += async (sender, e) =>
        {
            var result = ((AddSupplierViewModel)sender).Result;
            if (result != null)
            {
                await DatabaseAccessLayer.AddSupplier(result);
            }
        };
        await _dialogService.ShowDialog(addNewSupplierViewModel);
    }


    private async Task ExecuteAssignNewSupplier()
    {
        var gu = (await DatabaseAccessLayer.Suppliers()).Values
            .Where(p => p.supplierID != SelectedSupplier.Id)
            .Select(s => new Idname(s.supplierID, s.name))
            .ToList();

        var addExtraSupplierViewModel = new AddExtraSupplierViewModel(gu, _dialogService);
        addExtraSupplierViewModel.RequestClose += async (sender, e) =>
        {
            var result = ((AddExtraSupplierViewModel)sender).Result;
            if (result != null)
            {
                var hed = await DatabaseAccessLayer.AddNewItemHeader(result.Item1, Convert.ToDecimal(result.Item2), SelectedItem.Id, 0, LocationId, 1, 1, "GBP", "GBP");
            }
        };
        await _dialogService.ShowDialog(addExtraSupplierViewModel);
    }

       

    private async Task ExecuteEditMarket(FullItmMarketAss itema)
    {
        var marketki = (await DatabaseAccessLayer.markety())
            .Where(p => p.Key != 2 && p.Key != 8 && p.Key != 9)
            .Select(s => new Idname(s.Key, s.Value.name))
            .ToList();

        var addNewMarket = new AddMarketViewModel(marketki, _dialogService, DatabaseAccessLayer, LocationId, itema);
        addNewMarket.RequestClose += async (sender, e) =>
        {
            var result = ((AddMarketViewModel)sender).Result;
            if (result != null)
            {
                await DatabaseAccessLayer.RefreshMarket(itema);
                await AddAssociatedMarkets();
            }
        };
        await _dialogService.ShowDialog(addNewMarket);
    }

    private void ExecuteLeftArrowCommand()
    {
        if (CanHistoryBack)
        {
            ExecuteHistoryBack();
        }
    }

    private async Task ExecutePrintLabel()
    {
        var prePrintLabel = new PrePrintLabelViewModel(_dialogService,SettingsService);
        prePrintLabel.RequestClose += async (sender, e) =>
        {
            var result = ((PrePrintLabelViewModel)sender).Response;

            if (!result.Equals(default(KeyValuePair<string, short>)))
            {
                var lb = LabelPropertiesManager.GetProperty(result.Key);
                var np = await LabelPropertiesManager.GetLabelNamePack(DatabaseAccessLayer, SelectedItem.Id);
                _printersService.PrintBWLabel(lb, np, SettingsService.GetAllSettings().First(p => p.Key.Equals("label_printer")).Value, result.Value);
            }
        };
        await _dialogService.ShowDialog(prePrintLabel);
    }

    private void ExecuteRightArrowCommand()
    {
        if (CanHistoryForward)
        {
            ExecuteHistoryForward();
        }
    }

    private async Task ExecuteSaveProduct()
    {
        var weight = (Enumerable.First<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldName.Equals(denLanguageResourses.Resources.Weight)) as TextFieldViewModel).SelectedValue;
        if (!int.TryParse(weight, out int waga))
        {
            var errorMessage = string.Format(denLanguageResourses.Resources.ErrorValidatingMessage, waga);
            await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorValidatingTitle, errorMessage);

            return;
        }

        var body = new itembody
        {
            itembodyID = SelectedItem.Id,
            brandID = _brandvm.SelectedValue.Id,
            typeId = _typevm.SelectedValue.Id,
            name = (Enumerable.First<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldName.Equals(denLanguageResourses.Resources.FullName)) as TextFieldViewModel).SelectedValue,// SelectedItem.FullName,
            myname = (Enumerable.First<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldName.Equals(denLanguageResourses.Resources.ShortName)) as TextFieldViewModel).SelectedValue,// SelectedItem.MyName,
            mpn = (Enumerable.First<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldName.Equals("MPN")) as TextFieldViewModel).SelectedValue,  //SelectedItem.Mpn
            weight = Convert.ToInt32((Enumerable.First<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldName.Equals(denLanguageResourses.Resources.Weight)) as TextFieldViewModel).SelectedValue)
        };

        ItemBodyUpdateOperationResult rezultat = null;
        if (body.name.ToLower().Equals("discontinued"))
        {
            rezultat = new ItemBodyUpdateOperationResult
            {
                Status = ItemBodyDBOperationStatus.OperationSuccessful,
                itembody = body
            };
        }
        else
        {
            rezultat = await DatabaseAccessLayer.CheckIfSuchBodyCanBeSaved(SelectedItem.Id, body.name, body.myname, body.mpn);
        }

        switch (rezultat.Status)
        {
            case ItemBodyDBOperationStatus.ObjectWithNameExists:
                var nameErrorMessage = string.Format(denLanguageResourses.Resources.ProductNameExists, rezultat.itembody.name);
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, nameErrorMessage);
                break;

            case ItemBodyDBOperationStatus.ObjectWithShortNameExists:
                var shortNameErrorMessage = string.Format(denLanguageResourses.Resources.ProductShortNameExists, rezultat.itembody.myname);
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, shortNameErrorMessage);
                break;

            case ItemBodyDBOperationStatus.ObjectWithMpnExists:
                var mpnErrorMessage = string.Format(denLanguageResourses.Resources.ProductMpnExists, rezultat.itembody.mpn);
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, mpnErrorMessage);
                break;

            case ItemBodyDBOperationStatus.UnknownError:
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, denLanguageResourses.Resources.UnknownError);
                break;

            case ItemBodyDBOperationStatus.OperationSuccessful:

                var header = new itemheader
                {
                    itemheaderID = DatabaseAccessLayer.items[SelectedItem.Id].ItemHeaders.First(p => p.supplierID == SelectedSupplier.Id).itemheaderID,
                    quantity = Convert.ToInt32((Enumerable.First<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldIdentifier.Equals("quantity")) as TextFieldViewModel).SelectedValue),
                    pricePaid = Convert.ToDecimal((Enumerable.First<IBaseFieldViewModel>(SimpleDescription.Items, p => p.FieldIdentifier.Equals("pricepaidOriginal")) as TextFieldViewModel).SelectedValue)
                };
                List<parametervalue> cechy = Enumerable.Select<ParameterItemViewModel, parametervalue>(CechyItems, p => p.SelectedCechaValue).Where(p => p.parameterID != -1).ToList();
                await DatabaseAccessLayer.SaveBodyHeaderCechy(body, header, cechy);

                break;

            default:
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, denLanguageResourses.Resources.UnknownError);
                break;
        }
    }


    private async Task ExecuteTurnTrackOnOff()
    {
        var xo = SelectedItem.ReadyToTrack;
        if (!xo)
        {
            var Suppliers = (await DatabaseAccessLayer.Suppliers()).Values.ToDictionary(p => p.supplierID, q => q.name);

            Dictionary<Idname, int> result = DatabaseAccessLayer.items[SelectedItem.Id].ItemHeaders
                .Where(header => Suppliers.ContainsKey(header.supplierID)) // filtruje tylko te, które mają dostawcę
                .ToDictionary(
                    header => new Idname(header.supplierID, Suppliers[header.supplierID]), // tworzy nowy obiekt Idname jako klucz

                    header => header.quantity // używa quantity jako wartość
                );
            var launchTrackingViewModel = new LaunchTrackingViewModel(result);
            launchTrackingViewModel.RequestClose += async (sender, e) =>
            {
                var result = ((LaunchTrackingViewModel)sender).Result;
                if (result is not null)
                {
                    Dictionary<KeyValuePair<int, string>, int> redoneResult = new Dictionary<KeyValuePair<int, string>, int>();
                    foreach (var item in result)
                    {
                        KeyValuePair<int, string> newKey = new KeyValuePair<int, string>(item.Key.Id, item.Key.Name);
                        redoneResult.Add(newKey, item.Value);
                    }
                    var mainTask = Task.WhenAll(
                        DatabaseAccessLayer.RefreshQuantitiesInHeaders(redoneResult, SelectedItem.Id, LocationId, true),
                        _marketActions.AssignQuantities2markets(SelectedItem.Id, 1, pisz));
                    await _activityTaskWrapper.ExecuteTaskAsync(mainTask, denLanguageResourses.Resources.UpdatingNumbersOnMarkets);

                }
            };
            await _dialogService.ShowDialog(launchTrackingViewModel);
        }
        else
        {
            var tgtgt = await DatabaseAccessLayer.FlipReadyToTrack(SelectedItem.Id);
        }
        void pisz(string s)
        { }
    }

    private async Task ExecuteViewLogs()
    {
        var ro = await GetProductLog.GetLog(SelectedItem.Id, LocationId);

        var me = new ItemEventViewModel(ro, _dialogService);
        await _dialogService.ShowDialog(me);
    }


        

    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        IsBusy = true;
        await DatabaseAccessLayer.GetPackage(LocationId);
        _brandsSuppliersMarkets.Markets = await DatabaseAccessLayer.markety();
        _brandsSuppliersMarkets.Suppliers = (await DatabaseAccessLayer.Suppliers()).Values.ToDictionary(p => p.supplierID, q => q.name);
        _brandsSuppliersMarkets.Brands = await DatabaseAccessLayer.Brands();
        _brandsSuppliersMarkets.Types = await DatabaseAccessLayer.types();
        int i = 1;
        Dictionary<int, multidrawer> multiDrawers = (await DatabaseAccessLayer.multidrawer()).ToDictionary(p => p.MultiDrawerID, q => q);
        List<bodiesgrouped> bodiesGrouped = await DatabaseAccessLayer.bodiesgrouped();
        var Brands = _brandsSuppliersMarkets.Brands;
        var Types = await DatabaseAccessLayer.types();

        foreach (var item in DatabaseAccessLayer.items)
        {
            var tre = new ProductViewItem
            {
                Assigned = bodiesGrouped.Any(p => p.itemBodyID == item.Key),
                Brand = Brands[item.Value.itembody.brandID],
                FullName = item.Value.itembody.name,
                Id = item.Key,
                Mpn = item.Value.itembody.mpn,
                MyName = item.Value.itembody.myname,
                Type = Types[item.Value.itembody.typeId],
                Quantity = item.Value.ItemHeaders.Sum(p => p.quantity),
                LocatedAt = GetBodyInTheBoxForId(item.Key),
                ReadyToTrack = item.Value.itembody.readyTotrack,
                Weight = item.Value.itembody.weight,
                Number = i
            };
            i++;
            ProductItems.Add(tre);
        }

        string GetBodyInTheBoxForId(int itemBodyId)
        {
            string output = "";

            var jest = DatabaseAccessLayer.items[itemBodyId].bodyinthebox;
            if (jest != null)
            {
                var sza = multiDrawers[jest.MultiDrawerID].name;
                output = sza + '[' + (char)(65 + jest.column) + ',' + (jest.row + 1) + ']';
            }
            return output;
        }




        //    Dictionary<int, HashSet<int[]>> AllProducts = new Dictionary<int, HashSet<int[]>>();
        List<StringInt> allProducts = new();
        foreach (var item in ProductItems)
        {
            var id = item.Id;
            var ke = item.FullName + ' ' + item.MyName + ' ' + item.Mpn;
            //AllProducts.Add(id, SearchEngine32Bit.ConvertStringToInt32Entry(ke));
            allProducts.Add(new StringInt(ke, id));
        }
        //_searchEngine = new SearchEngine32Bit(AllProducts);
        _searchEngine = new ShookayWrapper<StringInt>(allProducts);
        await _searchEngine.PrepareEntries();

        var currencyNameResponse = SettingsService.GetSetting("currency");
        if (currencyNameResponse.IsSuccess)
        {
            CurrencyName = currencyNameResponse.GetValue<string>();
        }
        else
        {
            CurrencyName = "GBP";
            SettingsService.UpdateSetting("currency", "GBP");
        }


        var currencies = await DatabaseAccessLayer.Currencies();

        CurrencySymbol = currencies[CurrencyName].symbol;

        IsBusy = false;
        IsDataLoaded = true;
        if (PendingProductId.HasValue)
        {
            ShowProduct(PendingProductId.Value);
            PendingProductId = null;
        }
        await OnSearchChanged();
    }



    private async Task LoadSomeData(List<int> ids)
    {
        Dictionary<int, multidrawer> MultiDrawers = (await DatabaseAccessLayer.multidrawer()).ToDictionary(p => p.MultiDrawerID, q => q);


        foreach (var id in ids)
        {
            var item = DatabaseAccessLayer.items[id];
            var tre = new ProductViewItem
            {
                Assigned = (await DatabaseAccessLayer.bodiesgrouped()).Any(p => p.itemBodyID == item.itembody.itembodyID),
                Brand = (await DatabaseAccessLayer.Brands())[item.itembody.brandID],
                FullName = item.itembody.name,
                Id = id,
                Mpn = item.itembody.mpn,
                MyName = item.itembody.myname,
                Type = (await DatabaseAccessLayer.types())[item.itembody.typeId],
                Quantity = item.ItemHeaders.Sum(p => p.quantity),
                LocatedAt = GetBodyInTheBoxForId(id),
                ReadyToTrack = item.itembody.readyTotrack,
                Weight = item.itembody.weight
            };

            var prod = Enumerable.FirstOrDefault<ProductViewItem>(ProductItems, p => p.Id == id);
            if (prod != null)
            {
                int index = ProductItems.IndexOf(prod);
                tre.Number = prod.Number;
                ProductItems[index] = tre;

                await _searchEngine.RefreshEntry(tre.Id, tre.FullName + ' ' + tre.MyName + ' ' + tre.Mpn);
                //_searchEngine.Refresh(new KeyValuePair<int, HashSet<int[]>>(tre.Id, SearchEngine32Bit.ConvertStringToInt32Entry(tre.FullName + ' ' + tre.MyName + ' ' + tre.Mpn)));
                var prodView = Enumerable.FirstOrDefault<ProductViewItem>(ProductView, p => p.Id == id);
                if (prodView != null)
                {
                    _dispatcherService.Invoke(() =>
                    {
                        int viewIndex = ProductView.IndexOf(prodView);
                        ProductView[viewIndex] = tre;
                    });
                }
                else
                {
                    _dispatcherService.Invoke(() => ProductView.Add(tre));
                }
            }
            else
            {
                await _searchEngine.AddEntry(tre.Id, tre.FullName + ' ' + tre.MyName + ' ' + tre.Mpn);
                _dispatcherService.Invoke(async () =>
                {
                    tre.Number = ProductItems.Count + 1;
                    ProductItems.Add(tre);
                    //    _searchEngine.AddEntry(new KeyValuePair<int, HashSet<int[]>>(tre.Id, SearchEngine32Bit.ConvertStringToInt32Entry(tre.FullName + ' ' + tre.MyName + ' ' + tre.Mpn)));

                    ProductView.Add(tre);
                });
            }
            if (SelectedItem == null)
            {
                return;
            }

            if (SelectedItem.Id == id)
            {
                _dispatcherService.Invoke(() => SelectedItem = tre);
            }
        }



        string GetBodyInTheBoxForId(int itemBodyId)
        {
            string output = "";

            var jest = DatabaseAccessLayer.items[itemBodyId].bodyinthebox;
            if (jest != null)
            {
                var sza = MultiDrawers[jest.MultiDrawerID].name;
                output = sza + '[' + (char)(65 + jest.column) + ',' + (jest.row + 1) + ']';
            }
            return output;
        }

    }

        

    private async Task OnSearchChanged()
    {
        _cancellationTokenSource.Cancel(); // Cancel any ongoing search
        _cancellationTokenSource = new CancellationTokenSource(); // Create new cancellation token source

        var token = _cancellationTokenSource.Token;
        var searchByName = _searchByName?.ToLower();  // use ToLower for string comparison

        try
        {
            await Task.Delay(200, token);  // Wait for a short delay to debounce
        }
        catch (TaskCanceledException)
        {
            return; // If task is cancelled, stop executing method.
        }

        if (!token.IsCancellationRequested)  // Proceed only if there were no new changes during the delay
        {
            List<ProductViewItem> results;

            if (string.IsNullOrEmpty(searchByName))// && string.IsNullOrEmpty(searchByMpn))
            {
                // If both search fields are empty, return all products
                results = Enumerable.ToList<ProductViewItem>(_productItems);
            }
            else
            {
                // Otherwise, perform the search

                var gux =// await _searchEngine.Find(searchByName); 
                    await _searchEngine.FindWithin(searchByName);
                results = Enumerable.Where<ProductViewItem>(ProductItems, item => Enumerable.Contains<int>(gux, item.Id)).ToList();

            }

            if (!token.IsCancellationRequested) // Proceed only if there were no new changes during the search
            {
                if (FilteringProducts)
                {
                    results = await FilterProductsOut(results);
                }
                _dispatcherService.Invoke(() =>
                {
                    _productView.Clear();


                    foreach (var result in results)
                    {
                        _productView.Add(result);
                    }
                });
            }
        }
    }

    public async Task<List<ProductViewItem>> FilterProductsOut(List<ProductViewItem> products)
    {
        var zwrotka = new List<ProductViewItem>();

        foreach (var product in products)
        {
            bool cango = true;
            if (!MyProductFiltersPack.Types.Contains(DatabaseAccessLayer.items[product.Id].itembody.typeId))
            {
                cango = false;
            }
            if (cango)
            {
                cango = DatabaseAccessLayer.items[product.Id].ItemHeaders.Select(p => p.supplierID).All(item => MyProductFiltersPack.Suppliers.Contains(item));
            }
            if (cango)
            {
                cango = DatabaseAccessLayer.items[product.Id].ItmMarketAssocs.Select(p => p.marketID).All(item => MyProductFiltersPack.Markets.Contains(item));
            }
            if (cango)
            {
                zwrotka.Add(product);
            }
        }

        return zwrotka;
    }

    private async Task RemoveMarket(FullItmMarketAss itema)
    {
        await DatabaseAccessLayer.RemoveMarket(itema);
        await AddAssociatedMarkets();
    }

    private void RowClick(ProductViewItem item)=> SelectedItem = item;

       


}