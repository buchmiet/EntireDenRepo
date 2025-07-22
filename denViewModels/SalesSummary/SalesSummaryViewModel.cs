using DataServicesNET80;
using DataServicesNET80.Models;
using denModels;
using denSharedLibrary;
using denViewModels.SalesSummary;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SettingsKeptInFile;
using static denViewModels.SalesSummary.StatsViewModel;
using DataServicesNET80.DatabaseAccessLayer;

namespace denViewModels;

public enum DateOptionEnum
{
    CustomDate,
    LastQuarter,
    LastFinancialYear,
    EntirePeriod
}

public enum MarketCountriesOptionEnum
{
    Marketplaces,
    Countries
}

public class SalesSummaryViewModel : ObservableObject
{
    public ObservableCollection<MonthItem> Months { get; } = new ObservableCollection<MonthItem>
    {
        new MonthItem { MonthName = denLanguageResourses.Resources.January,OrderedNumber=1 },
        new MonthItem { MonthName = denLanguageResourses.Resources.February,OrderedNumber=2 },
        new MonthItem { MonthName = denLanguageResourses.Resources.March,OrderedNumber=3 },
        new MonthItem { MonthName = denLanguageResourses.Resources.April,OrderedNumber=4 },
        new MonthItem { MonthName = denLanguageResourses.Resources.May,OrderedNumber=5 },
        new MonthItem { MonthName = denLanguageResourses.Resources.June,OrderedNumber=6 },
        new MonthItem { MonthName = denLanguageResourses.Resources.July,OrderedNumber=7 },
        new MonthItem { MonthName = denLanguageResourses.Resources.August,OrderedNumber=8 },
        new MonthItem { MonthName = denLanguageResourses.Resources.September,OrderedNumber=9 },
        new MonthItem { MonthName = denLanguageResourses.Resources.October,OrderedNumber=10 },
        new MonthItem { MonthName = denLanguageResourses.Resources.November,OrderedNumber=11 },
        new MonthItem { MonthName = denLanguageResourses.Resources.December,OrderedNumber=12 }
    };

    private bool _isUKChecked;

    public bool IsUKChecked
    {
        get => _isUKChecked;
        set
        {
            SetProperty(ref _isUKChecked, value);
            UpdateSelectedCountriesInSettings("uk", value);
        }
    }

    private bool _isEUChecked;

    public bool IsEUChecked
    {
        get => _isEUChecked;
        set
        {
            SetProperty(ref _isEUChecked, value);
            UpdateSelectedCountriesInSettings("eu", value);
        }
    }

    private bool _isEUPlusChecked;

    public bool IsEUPlusChecked
    {
        get => _isEUPlusChecked;
        set
        {
            SetProperty(ref _isEUPlusChecked, value);
            UpdateSelectedCountriesInSettings("euplus", value);
        }
    }

    private bool _isWorldChecked;

    public bool IsWorldChecked
    {
        get => _isWorldChecked;
        set
        {
            SetProperty(ref _isWorldChecked, value);
            UpdateSelectedCountriesInSettings("world", value);
        }
    }

    public void UpdateSelectedCountriesInSettings(string countryName, bool checker)
    {
        var kantries = SettingsService.GetSetting("salestatsSelectedCountries");
        if (kantries != null)
        {
            var selectedc = SettingsService.GetSetting("salestatsSelectedCountries").GetValue<string>() .Split(',').ToList();
            var sc = selectedc.FirstOrDefault(p => p.Equals(countryName));
            if (checker && sc == null)
            {
                selectedc.Add(countryName);
            }
            if (!checker && sc != null)
            {
                selectedc.Remove(sc);
            }
            if (selectedc.Count > 0)
            {
                SettingsService.UpdateSetting("salestatsSelectedCountries", string.Join(",", selectedc));
            }
        }
        else
        {
            SettingsService.UpdateSetting("salestatsSelectedCountries", countryName);
        }
    }

    public  void UpdateSelectedMarketsInSettings(string marketName, bool checker)
    {
        var kantries = SettingsService.GetSetting("salestatsSelectedMarkets").GetValue<string>();
        if (kantries != null)
        {
            var selectedc = SettingsService.GetSetting("salestatsSelectedMarkets").GetValue<string>().Split(',').ToList();
            var sc = selectedc.FirstOrDefault(p => p.Equals(marketName));
            if (checker && sc == null)
            {
                selectedc.Add(marketName);
            }
            if (!checker && sc != null)
            {
                selectedc.Remove(sc);
            }
            if (selectedc.Count > 0)
            {
                SettingsService.UpdateSetting("salestatsSelectedMarkets", string.Join(",", selectedc));
            }
        }
        else
        {
            SettingsService.UpdateSetting("salestatsSelectedMarkets", marketName);
        }
    }

    public ICommand LoadDataCommand { get; }

    private bool _isDataLoaded = false;

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set => SetProperty(ref _isDataLoaded, value);
    }

    private DateTime _oldestDate;

    public DateTime OldestDate
    {
        get => _oldestDate;
        set
        {
            SetProperty(ref _oldestDate, value);
        }
    }

    private DateTime _newestDate;

    public DateTime NewestDate
    {
        get => _newestDate;
        set
        {
            SetProperty(ref _newestDate, value);
        }
    }

    private DateTime _fromDate;
    private DateTime _toDate;

    public DateTime FromDate
    {
        get => _fromDate;
        set
        {
            SetProperty(ref _fromDate, value);
        }
    }

    public DateTime ToDate
    {
        get => _toDate;
        set
        {
            SetProperty(ref _toDate, value);
        }
    }

    public RelayCommand<DateOptionEnum> RadioButtonPeriodsCheckedCommand => new RelayCommand<DateOptionEnum>(OnRadioButtonPeriodsChecked);

    private void OnRadioButtonPeriodsChecked(DateOptionEnum selectedOption)
    {
        SelectedDateOption = selectedOption;
    }

    public RelayCommand<MarketCountriesOptionEnum> RadioButtonCountriesMarketsCheckedCommand => new RelayCommand<MarketCountriesOptionEnum>(RadioButtonCountriesMarketsChecked);

    private void RadioButtonCountriesMarketsChecked(MarketCountriesOptionEnum selectedOption)
    {
        SelectedMarketCountriesOption = selectedOption;
    }

    private MarketCountriesOptionEnum _selectedMarketCountriesOption;

    public MarketCountriesOptionEnum SelectedMarketCountriesOption
    {
        get => _selectedMarketCountriesOption;
        set
        {
            if (SetProperty(ref _selectedMarketCountriesOption, value))
            {
                OnPropertyChanged(nameof(AreMarketsSelected));
                OnPropertyChanged(nameof(AreCountriesSelected));
            }
        }
    }

    public bool AreMarketsSelected => _selectedMarketCountriesOption == MarketCountriesOptionEnum.Marketplaces;
    public bool AreCountriesSelected => _selectedMarketCountriesOption == MarketCountriesOptionEnum.Countries;

    private DateOptionEnum _selectedDateOption;

    public DateOptionEnum SelectedDateOption
    {
        get => _selectedDateOption;
        set
        {
            if (SetProperty(ref _selectedDateOption, value))
            {
                OnPropertyChanged(nameof(IsCustomDateSelected));
                OnPropertyChanged(nameof(IsLastQuarterSelected));
                OnPropertyChanged(nameof(IsLastFinancialYearSelected));
                OnPropertyChanged(nameof(IsEntirePeriodSelected));

                if (SelectedDateOption == DateOptionEnum.CustomDate)
                {
                    SettingsService.UpdateSetting("salestatsDateRange", "customdate");
                }
                if (SelectedDateOption == DateOptionEnum.LastQuarter)
                {
                    SettingsService.UpdateSetting("salestatsDateRange", "lastquarter");
                }
                if (SelectedDateOption == DateOptionEnum.LastFinancialYear)
                {
                    SettingsService.UpdateSetting("salestatsDateRange", "lastfinancialyear");
                }
                if (SelectedDateOption == DateOptionEnum.EntirePeriod)
                {
                    SettingsService.UpdateSetting("salestatsDateRange", "entireperiod");
                }
            }
        }
    }

    public bool IsCustomDateSelected => _selectedDateOption == DateOptionEnum.CustomDate;
    public bool IsLastQuarterSelected => _selectedDateOption == DateOptionEnum.LastQuarter;
    public bool IsLastFinancialYearSelected => _selectedDateOption == DateOptionEnum.LastFinancialYear;
    public bool IsEntirePeriodSelected => _selectedDateOption == DateOptionEnum.EntirePeriod;
    private IDispatcherService _dispatcherService;
    public IDatabaseAccessLayer _databaseAccessLayer;
    private IDialogService _dialogService;

    private bool _ordersDataDownloaded = false;

    public bool OrdersDataDownloaded
    {
        get => _ordersDataDownloaded;
        set
        {
            SetProperty(ref _ordersDataDownloaded, value);
        }
    }

    private bool _showWaitingWidget;

    public bool ShowWaitingWidget
    {
        get => _showWaitingWidget;
        set
        {
            if (SetProperty(ref _showWaitingWidget, value))
            {
                OnPropertyChanged(nameof(IsMainViewEnabled));
            }
        }
    }

    public bool IsMainViewEnabled
    {
        get => !ShowWaitingWidget;
    }

    public ICommand GetOrdersDataCommand { get; set; }
    public ICommand MonthSelectedCommand { get; set; }
    public ICommand GenerateCSVFileCommand { get; set; }

    public ObservableCollection<CountryViewModel> ExtraCountries { get; set; } = new ObservableCollection<CountryViewModel>();
    public ObservableCollection<MarketViewModel> ExtraMarkets { get; set; } = new ObservableCollection<MarketViewModel>();

    private IDispatcherTimer _dispatcherTimer;
    private IDispatcherTimerFactory _dispatcherTimerFactory;
    private IFileDialogService _fileDialogService;
    private int locationID;
    private ISettingsService SettingsService;

    public SalesSummaryViewModel(IDatabaseAccessLayer databaseAccessLayer, IDispatcherService dispatcherService, IDialogService dialogService, IDispatcherTimerFactory dispatcherTimerFactory,
        IFileDialogService fileDialogService,ISettingsService settingsService)
    {
        locationID = SettingsService.LocationId;
        _fileDialogService = fileDialogService;
        _dispatcherTimerFactory = dispatcherTimerFactory;
        _databaseAccessLayer = databaseAccessLayer;
        _dispatcherService = dispatcherService;
        _dialogService = dialogService;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        GetOrdersDataCommand = new AsyncRelayCommand(GetOrdersDataExecute);
        MonthSelectedCommand = new RelayCommand(MonthSelectedExecute);
        FromDate = DateTime.Today;
        ToDate = DateTime.Today;
        _ordersDataDownloaded = false;

        if (SettingsService.GetSetting("financialYearStart") != null)
        {
            if (int.TryParse(SettingsService.GetSetting("financialYearStart").GetValue<string>(), out int miesiac))
            {
                if (miesiac > 0 && miesiac < 13)
                {
                    Months.First(p => p.OrderedNumber == miesiac).IsSelected = true;
                }
            }
        }

        Ccodes = new ObservableCollection<string>();
        Mcodes = new ObservableCollection<StringInt>();
        AddCountryCommand = new RelayCommand(AddCountryExecute);
        AddMarketPlaceCommand = new RelayCommand(AddMarketPlaceExecute);
        ShowChartsCommand = new AsyncRelayCommand(ShowChartsExecute);
        NextWeekCommand = new AsyncRelayCommand(NextWeekExecute);
        PreviousWeekCommand = new AsyncRelayCommand(PreviousWeekExecute);
        GenerateCSVFileCommand = new AsyncRelayCommand(SaveOrders2CSVFile);
    }

    public async Task SaveOrders2CSVFile()
    {
        var stream = await _fileDialogService.SaveFileAsync(denLanguageResourses.Resources.SaveImageFile, "CSV Files|*.csv");
        StringBuilder csvcontent = new StringBuilder();
        csvcontent.Append("Order ID");
        csvcontent.Append(',');
        csvcontent.Append("Marketplace");
        csvcontent.Append(',');
        csvcontent.Append("Total");
        csvcontent.Append(',');
        csvcontent.Append("Net total");
        csvcontent.Append(',');
        csvcontent.AppendLine('"' + "Country" + '"');
        foreach (var orders in MyViewableCompletes.GroupedOrders.Values)
        {
            foreach (var order in orders)
            {
                csvcontent.Append(order.orderId);
                csvcontent.Append(',');
                csvcontent.Append(AllMarkets[order.MarketId].name);
                csvcontent.Append(',');
                csvcontent.Append(order.Total);
                csvcontent.Append(',');
                csvcontent.Append(order.NetTotal);
                csvcontent.Append(',');
                csvcontent.AppendLine('"' + CountryCodes[order.CountryCode] + '"');
            }
        }
        byte[] byteArray = Encoding.UTF8.GetBytes(csvcontent.ToString());
        if (stream != null)
        {
            using (stream)
            {
                stream.Write(byteArray, 0, byteArray.Length);
            }
        }
    }

    private OrderDetailsOneColumnsViewModel _orderDetailsViewModel;

    public OrderDetailsOneColumnsViewModel OrderDetailsViewModel
    {
        get { return _orderDetailsViewModel; }
        set { SetProperty(ref _orderDetailsViewModel, value); }
    }

    public void AddMarketPlaceExecute()
    {
        var mvm = new MarketViewModel(new KeyValuePair<List<int>, string>
            (
                new List<int> { SelectedMarket.Id }, SelectedMarket.Name)
        );
        mvm.IsChecked = true;
        mvm.CanRemove = true;
        int id = SelectedMarket.Id;
        mvm.RemoveCommand = new RelayCommand(() =>
        {
            var em = ExtraMarkets.FirstOrDefault(p => p.id.First() == id && p.CanRemove);
            ExtraMarkets.Remove(em);
            UpdateSelectedMarketsInSettings(em.MarketName, false);
        });
        var marketname = mvm.MarketName;
        mvm.CheckChangedCommand = new RelayCommand<MarketViewModel.MarketViewModelProperties>(param =>
        {
            UpdateSelectedMarketsInSettings(param.MarketName, param.IsChecked);
        });
        ExtraMarkets.Add(mvm);
        UpdateSelectedMarketsInSettings(marketname, true);
        SelectedMarket = null;
    }

    public void MonthSelectedExecute()
    {
        var ts = Months.FirstOrDefault(p => p.IsSelected);
        SettingsService.UpdateSetting("financialYearStart", ts.OrderedNumber.ToString());
    }

    public string Currency;

    private List<string> EUCountries = new List<string> {  "AT", // Austria
        "BE", // Belgia
        "BG", // Bułgaria
        "HR", // Chorwacja
        "CY", // Cypr
        "CZ", // Czechy
        "DK", // Dania
        "EE", // Estonia
        "FI", // Finlandia
        "FR", // Francja
        "GR", // Grecja
        "ES", // Hiszpania
        "NL", // Holandia
        "IE", // Irlandia
        "LT", // Litwa
        "LU", // Luksemburg
        "LV", // Łotwa
        "MT", // Malta
        "DE", // Niemcy
        "PL", // Polska
        "PT", // Portugalia
        "RO", // Rumunia
        "SK", // Słowacja
        "SI", // Słowenia
        "SE", // Szwecja
        "HU", // Węgry

        "IT"  // Włochy
    };

    private List<string> EUPLusCountries = new List<string>{
        "AT", // Austria
        "BE", // Belgia
        "BG", // Bułgaria
        "HR", // Chorwacja
        "CY", // Cypr
        "CZ", // Czechy
        "DK", // Dania
        "EE", // Estonia
        "FI", // Finlandia
        "FR", // Francja
        "GR", // Grecja
        "ES", // Hiszpania
        "NL", // Holandia
        "IE", // Irlandia
        "IS", // Islandia
        "LI", // Liechtenstein
        "LT", // Litwa
        "LU", // Luksemburg
        "LV", // Łotwa
        "MT", // Malta
        "DE", // Niemcy
        "NO", // Norwegia
        "PL", // Polska
        "PT", // Portugalia
        "RO", // Rumunia
        "SK", // Słowacja
        "SI", // Słowenia
        "SE", // Szwecja
        "CH", // Szwajcaria
        "HU", // Węgry

        "IT"  // Włochy
    };

    public async Task GetOrdersDataExecute()
    {
        if (AreCountriesSelected && !IsUKChecked && !IsEUChecked && !IsEUPlusChecked && !IsWorldChecked && !ExtraCountries.Any(p => p.IsChecked))
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, denLanguageResourses.Resources.SelectCountryOrGroup);
            return;
        }
        var ts = Months.FirstOrDefault(p => p.IsSelected);
        List<Tuple<DateTime, DateTime>> periods = null;
        if (SelectedDateOption == DateOptionEnum.CustomDate)
        {
            if (FromDate >= ToDate)
            {
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, string.Format(denLanguageResourses.Resources.DateRangeError, FromDate.ToString("yyyy/MM/dd"), ToDate.ToString("yyyy/MM/dd")));
                return;
            }
            periods = new List<Tuple<DateTime, DateTime>>
            {
                Tuple.Create(FromDate, ToDate)
            };
        }
        if (SelectedDateOption == DateOptionEnum.LastQuarter)
        {
            if (!Months.Any(p => p.IsSelected))
            {
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, denLanguageResourses.Resources.SelectTaxYearStartMonth);
                return;
            }
            periods = GetTaxYearQuarters(Months.First(p => p.IsSelected).OrderedNumber, DateTime.Today);
        }
        if (SelectedDateOption == DateOptionEnum.LastFinancialYear)
        {
            if (!Months.Any(p => p.IsSelected))
            {
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, denLanguageResourses.Resources.SelectTaxYearStartMonth);
                return;
            }
            periods = GetTaxYears(Months.First(p => p.IsSelected).OrderedNumber, DateTime.Today);
        }
        if (SelectedDateOption == DateOptionEnum.EntirePeriod)
        {
            periods = new List<Tuple<DateTime, DateTime>> { Tuple.Create(OldestDate, DateTime.Today) };
        }
        HashSet<string> Countries = new();
        Dictionary<List<int>, string> MarketsGrouped = new();
        Dictionary<List<string>, string> CountriesGrouped = new();
        if (AreCountriesSelected)
        {
            if (IsWorldChecked)
            {
                Countries = CountryCodes.Keys.ToHashSet();
            }
            else
            {
                if (IsEUPlusChecked)
                {
                    Countries = new HashSet<string>(EUPLusCountries)
                        ;
                }
                else
                {
                    if (IsEUChecked)
                    {
                        Countries = new HashSet<string>(EUCountries);
                    }
                }
                if (IsUKChecked)
                {
                    Countries.Add("GB");
                }
            }
            foreach (var country in ExtraCountries)
            {
                if (country.IsChecked)
                {
                    Countries.Add(CountryCodes.First(p => p.Value.Equals(country.CountryName)).Key);
                }
            }
            if (IsWorldChecked)
            {
                CountriesGrouped.Add(CountryCodes.Keys.ToList(), denLanguageResourses.Resources.World);
            }
            if (IsEUPlusChecked)
            {
                CountriesGrouped.Add(EUPLusCountries, denLanguageResourses.Resources.EUPlus);
            }
            if (IsEUChecked)
            {
                CountriesGrouped.Add(EUPLusCountries, denLanguageResourses.Resources.EU);
            }
            if (IsUKChecked)
            {
                CountriesGrouped.Add(new List<string> { "GB" }, denLanguageResourses.Resources.UK);
            }
            foreach (var kraj in ExtraCountries.Where(p => p.IsChecked))
            {
                CountriesGrouped.Add(new List<string> { CountryCodes.First(p => p.Value.Equals(kraj.CountryName)).Key }, kraj.CountryName);
            }
        }
        else
        {
            Countries = CountryCodes.Keys.ToHashSet();
            foreach (var market in ExtraMarkets.Where(p => p.IsChecked))
            {
                MarketsGrouped.Add(market.id, market.MarketName);
            }
        }

        if (SettingsService.GetSetting("currency") != null)
        {
            Currency = SettingsService.GetSetting("currency").GetValue<string>();
        }
        else
        {
            Currency = "GBP";
            SettingsService.UpdateSetting("currency", "GBP");
        }
        ShowWaitingWidget = true;
        var tuuu = await _databaseAccessLayer.GetOrderDataAsync(Currency, periods.First(), Countries, locationID);
        Dictionary<DateTime, List<orderData>> _GroupedOrders = tuuu.GroupBy(order => order.PaidOn.Date).ToDictionary(group => group.Key, group => group.ToList());
        ShowWaitingWidget = false;

        ChartsPack chartsPack = new ChartsPack
        {
            GroupedOrders = _GroupedOrders,
            YSymbol = '£'
        };
        if (AreMarketsSelected)
        {
            chartsPack.WhatAreWeComparing = ComparedData.MarketPlaces;
            chartsPack.marketGroups = MarketsGrouped;
        }
        else
        {
            chartsPack.WhatAreWeComparing = ComparedData.Countries;
            chartsPack.countriesGroups = CountriesGrouped;
        }
        OrdersDataDownloaded = true;
        MyChartsPack = chartsPack;
        await TinyOrdersListBoxLogic(_GroupedOrders);
    }

    public async Task TinyOrdersListBoxLogic(Dictionary<DateTime, List<orderData>> groupedOrders)
    {
        MyViewableCompletes = new ViewableCompletes(this, groupedOrders, _databaseAccessLayer);
        var tolb = await MyViewableCompletes.GetCurrentOrderViewItems();
        _dispatcherService.Invoke(() =>
        {
            TotalNumberOfOrders = MyViewableCompletes.TotalOrders;
            TinyOrdersListBox.Clear();
            foreach (var item in tolb)
            {
                TinyOrdersListBox.Add(item);
            }
            CanSeePreviousWeeks = MyViewableCompletes.CanSeePreviousWeeks;
            CanSeeFollowingWeek = MyViewableCompletes.CanSeeFollowingWeek;
        });
    }

    public ObservableCollection<IOrderWidgetViewModel> OrderWidgetItems { get; set; } = new ObservableCollection<IOrderWidgetViewModel>();
    private OrderViewItem _selectedOrder;

    public OrderViewItem SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (SetProperty(ref _selectedOrder, value))
            {
                SetWidgetItems();
                OrderDetailsViewModel = new OrderDetailsOneColumnsViewModel(OrderWidgetItems.ToList(), denLanguageResourses.Resources.OrderDetailsLabel);
            }
        }
    }

    public async void SetWidgetItems()
    {
        OrderWidgetItems.Clear();
        if (SelectedOrder == null)
        {
            return;
        }
        var komplet = MyViewableCompletes.GetKomplet(SelectedOrder.OrderID);
        OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.OrderIDLabel, SelectedOrder.OrderID.ToString()));
        OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.MarketLabel, (await _databaseAccessLayer.markety())[komplet.Order.market].name));
        OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.PaidOnLabel, komplet.Order.paidOn.ToString()));
        OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.TotalLabel, komplet.Order.salecurrency + ' ' + Math.Round(Convert.ToDecimal(komplet.Order.saletotal), 2)));
        OrderWidgetItems.Add(new OrderWidgetViewModelTextBlockBold(denLanguageResourses.Resources.ItemsLabel));

        int i = 1;
        List<string> LeftColumn = new List<string>();
        List<string> RightColumn = new List<string>();
        foreach (var predm in komplet.OrderItems)
        {
            var cialko = _databaseAccessLayer.items[predm.itembodyID];
            if (cialko.itembody.mpn.ToLower().Equals("tool"))
            {
                LeftColumn.Add("Tool");
                RightColumn.Add("£1");
            }
            else
            {
                LeftColumn.Add(i.ToString() + " : " + (cialko.itembody.myname + "\\mpn:" + cialko.itembody.mpn
                    ));
                RightColumn.Add(Currencies[komplet.Order.salecurrency] + (Math.Round(Convert.ToDecimal(predm.price), 2)).ToString());
            }
            i++;
        }
        OrderWidgetItems.Add(new OrderWidgetViewModelGrid4x4(LeftColumn, RightColumn));

        OrderWidgetItems.Add(new OrderWidgetViewModelTextBlockBold(denLanguageResourses.Resources.BuyerLabel));

        if (!string.IsNullOrWhiteSpace(komplet.Customer.CompanyName))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.CompanyLabel, komplet.Customer?.CompanyName));
        }

        if (!string.IsNullOrWhiteSpace(komplet.Customer?.GivenName))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.FirstNameLabel, komplet.Customer?.GivenName));
        }

        if (!string.IsNullOrWhiteSpace(komplet.Customer?.MiddleName))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.SecondNameLabel, komplet.Customer?.MiddleName));
        }

        if (!string.IsNullOrWhiteSpace(komplet.Customer?.FamilyName))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.SurnameLabel, komplet.Customer?.FamilyName));
        }

        if (!string.IsNullOrWhiteSpace(komplet.Customer?.Email))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.EmailLabel, komplet.Customer?.Email));
        }

        if (!string.IsNullOrWhiteSpace(komplet.Customer?.Phone))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.PhoneLabel, komplet.Customer?.Phone));
        }

        OrderWidgetItems.Add(new OrderWidgetViewModelTextBlockBold(denLanguageResourses.Resources.AddressLabel));

        if (!string.IsNullOrWhiteSpace(komplet.BillAddr?.Line1))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.FirstLineLabel, komplet.BillAddr?.Line1));
        }

        if (!string.IsNullOrWhiteSpace(komplet.BillAddr?.Line2))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.SecondLineLabel, komplet.BillAddr?.Line2));
        }

        if (!string.IsNullOrWhiteSpace(komplet.BillAddr?.City))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.CityLabel, komplet.BillAddr?.City));
        }

        if (!string.IsNullOrWhiteSpace(komplet.BillAddr?.PostalCode))
        {
            OrderWidgetItems.Add(new OrderWidgetViewModelUniformGrid(denLanguageResourses.Resources.PostcodeLabel, komplet.BillAddr?.PostalCode));
        }
    }

    public ObservableCollection<OrderViewItem> TinyOrdersListBox { get; set; } = new ObservableCollection<OrderViewItem>();

    private ChartsPack MyChartsPack;

    private bool _canSeePreviousWeek = false;

    public bool CanSeePreviousWeeks
    {
        get => _canSeePreviousWeek;
        set => SetProperty(ref _canSeePreviousWeek, value);
    }

    private bool _canSeeFollowingWeek = false;

    public bool CanSeeFollowingWeek
    {
        get => _canSeeFollowingWeek;
        set => SetProperty(ref _canSeeFollowingWeek, value);
    }

    public AsyncRelayCommand ShowChartsCommand { get; set; }
    public AsyncRelayCommand PreviousWeekCommand { get; set; }

    public async Task ShowChartsExecute()
    {
        var StatsWindowVM = new StatsViewModel(MyChartsPack, _dispatcherTimerFactory,SettingsService);
        StatsWindowVM.RequestClose += async (sender, e) =>
        {
            var result = ((StatsViewModel)sender).Response;
            if (result != null)
            {
                SettingsService.UpdateSettings(new Dictionary<string, string>
                {
                    { "chartsWindowTopLeftX", result.X.ToString() },
                    { "chartsWindowTopLeftY", result.Y.ToString() },
                    { "chartsWindowWidth", result.Width.ToString() },
                    { "chartsWindowHeight", result.Height.ToString() }
                });
            }
        };
        await _dialogService.ShowDialog(StatsWindowVM);
    }

    public ViewableCompletes MyViewableCompletes;
    public AsyncRelayCommand NextWeekCommand { get; set; }
    private int _totalNumberOfOrders;

    public int TotalNumberOfOrders
    {
        get => _totalNumberOfOrders;
        set => SetProperty(ref _totalNumberOfOrders, value);
    }

    public async Task NextWeekExecute()
    {
        _dispatcherService.Invoke(() =>
        {
            CanSeePreviousWeeks = false;
            CanSeeFollowingWeek = false;
            ShowWaitingWidget = true;
        });
        MyViewableCompletes.CurrentPage++;
        var tolb = await MyViewableCompletes.GetCurrentOrderViewItems();
        _dispatcherService.Invoke(() =>
        {
            ShowWaitingWidget = false;
            TinyOrdersListBox.Clear();
            foreach (var item in tolb)
            {
                TinyOrdersListBox.Add(item);
            }
            CanSeePreviousWeeks = MyViewableCompletes.CanSeePreviousWeeks;
            CanSeeFollowingWeek = MyViewableCompletes.CanSeeFollowingWeek;
        });
    }

    public async Task PreviousWeekExecute()
    {
        _dispatcherService.Invoke(() =>
        {
            ShowWaitingWidget = true;
            CanSeePreviousWeeks = false;
            CanSeeFollowingWeek = false;
        });
        MyViewableCompletes.CurrentPage--;
        var tolb = await MyViewableCompletes.GetCurrentOrderViewItems();
        _dispatcherService.Invoke(() =>
        {
            ShowWaitingWidget = false;
            TinyOrdersListBox.Clear();
            foreach (var item in tolb)
            {
                TinyOrdersListBox.Add(item);
            }
            CanSeePreviousWeeks = MyViewableCompletes.CanSeePreviousWeeks;
            CanSeeFollowingWeek = MyViewableCompletes.CanSeeFollowingWeek;
        });
    }

    public class OrderViewItem : ObservableObject
    {
        private int _orderID;
        private int _number;
        private string _customerName;
        private string _marketplace;
        private string _country;
        private int _items;
        private decimal _total;
        private string _orderDate;

        public int OrderID
        {
            get => _orderID;
            set => SetProperty(ref _orderID, value);
        }

        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }

        public string Marketplace
        {
            get => _marketplace;
            set => SetProperty(ref _marketplace, value);
        }

        public string Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        public int Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public decimal Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        public string OrderDate
        {
            get => _orderDate;
            set => SetProperty(ref _orderDate, value);
        }
    }

    public class ViewableCompletes
    {
        public int TotalOrders { get; set; }
        public int TotalWeeks { get; set; }
        public Dictionary<int, List<int>> OrderIds { get; set; } = new Dictionary<int, List<int>>();
        public Dictionary<int, int> OrderIdOrderNumber { get; set; } = new Dictionary<int, int>();
        private IDatabaseAccessLayer _databaseAccessLayer;
        public int CurrentPage { get; set; } = 0;

        private bool _canSeePreviousWeek = false;

        public bool CanSeePreviousWeeks
        {
            get => CurrentPage > 0;
        }

        private bool _canSeeFollowingWeek = false;

        public bool CanSeeFollowingWeek
        {
            get => CurrentPage < TotalWeeks - 1;
        }

        public Dictionary<DateTime, List<orderData>> GroupedOrders;
        private SalesSummaryViewModel ViewModel;

        public ViewableCompletes(SalesSummaryViewModel viewModel, Dictionary<DateTime, List<orderData>> groupedOrders, IDatabaseAccessLayer databaseAccessLayer)
        {
            _databaseAccessLayer = databaseAccessLayer;
            GroupedOrders = groupedOrders;
            DateTime startDate = groupedOrders.Keys.Min();
            DateTime endDate = groupedOrders.Keys.Max();
            ViewModel = viewModel;
            // Ustal początek i koniec pierwszego tygodnia.
            DateTime startOfWeek = startDate.AddDays(-(int)startDate.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            int weekNumber = 0;
            int ordernumber = 1;
            while (startOfWeek <= endDate)
            {
                List<int> orderIdsForWeek = new List<int>();
                for (DateTime day = startOfWeek; day <= endOfWeek; day = day.AddDays(1))
                {
                    if (groupedOrders.ContainsKey(day))
                    {
                        foreach (var order in groupedOrders[day])
                        {
                            orderIdsForWeek.Add(order.orderId);
                            OrderIdOrderNumber.Add(order.orderId, ordernumber);
                            ordernumber++;
                        }
                    }
                }

                if (orderIdsForWeek.Count > 0)
                {
                    OrderIds[weekNumber] = orderIdsForWeek;
                    weekNumber++;
                }

                startOfWeek = startOfWeek.AddDays(7);
                endOfWeek = endOfWeek.AddDays(7);
            }

            TotalOrders = groupedOrders.Sum(kvp => kvp.Value.Count);
            TotalWeeks = weekNumber;
        }

        private Complete[] Komplety;
        private int WeekNumberOfCompletes = -1;

        public Complete GetKomplet(int OrderId)
        {
            return Komplety.First(p => p.Order.orderID == OrderId);
        }

        public async Task<Complete[]> GetCurrentComplety()
        {
            if (WeekNumberOfCompletes != CurrentPage)
            {
                Komplety = (await _databaseAccessLayer.GetKomplety(OrderIds[CurrentPage])).ToArray();
                WeekNumberOfCompletes = CurrentPage;
            }
            return Komplety;
        }

        public async Task<List<OrderViewItem>> GetCurrentOrderViewItems()
        {
            var zwrotka = new List<OrderViewItem>();
            var proc = await GetCurrentComplety();
            foreach (var pro in proc)
            {
                zwrotka.Add(Komplet2OrderViewItem(pro));
            }
            return zwrotka;
        }

        public OrderViewItem Komplet2OrderViewItem(Complete komplet)
        {
            string name = "";
            if (!string.IsNullOrWhiteSpace(komplet.Customer.Title))
            {
                name = komplet.Customer.Title + " ";
            }
            if (!string.IsNullOrWhiteSpace(komplet.Customer.GivenName))
            {
                name += komplet.Customer.GivenName + " ";
            }
            if (!string.IsNullOrWhiteSpace(komplet.Customer.MiddleName))
            {
                name += komplet.Customer.MiddleName + " ";
            }
            if (!string.IsNullOrWhiteSpace(komplet.Customer.FamilyName))
            {
                name += komplet.Customer.FamilyName;
            }
            var oi = new OrderViewItem();

            oi.OrderID = komplet.Order.orderID;
            oi.CustomerName = name;
            oi.Number = OrderIdOrderNumber[komplet.Order.orderID];
            oi.Items = komplet.OrderItems.Count();
            oi.Country = ViewModel.CountryCodes[komplet.BillAddr.CountryCode];
            oi.Total = komplet.Order.saletotal;
            oi.OrderDate = komplet.Order.paidOn.ToShortDateString();
            if (!AllMarkets.ContainsKey(komplet.Order.market))
            {
                var tsts = ";";
            }
            oi.Marketplace = AllMarkets[komplet.Order.market].name;

            return oi;
        }
    }

    public List<Tuple<DateTime, DateTime>> GetTaxYearQuarters(int startMonth, DateTime today)
    {
        List<Tuple<DateTime, DateTime>> quarters = new List<Tuple<DateTime, DateTime>>();
        DateTime taxYearStart = new DateTime(today.Year, startMonth, 1);
        if (today < taxYearStart)
            taxYearStart = taxYearStart.AddYears(-1);

        for (int i = 0; i < 4; i++)
        {
            DateTime quarterStart = taxYearStart.AddMonths(i * 3);
            DateTime quarterEnd = quarterStart.AddMonths(3).AddDays(-1);

            if (quarterEnd > today)
                quarterEnd = today;

            if (quarterStart < OldestDate)
                continue;  // pomiń kwartał, jeśli data rozpoczęcia jest starsza niż OldestDate

            quarters.Add(Tuple.Create(quarterStart, quarterEnd));

            if (quarterEnd == today)
                break;
        }
        return quarters;
    }

    public List<Tuple<DateTime, DateTime>> GetTaxYears(int startMonth, DateTime today)
    {
        List<Tuple<DateTime, DateTime>> taxYears = new List<Tuple<DateTime, DateTime>>();
        DateTime currentTaxYearStart = new DateTime(today.Year, startMonth, 1);
        if (today < currentTaxYearStart)
            currentTaxYearStart = currentTaxYearStart.AddYears(-1);

        DateTime currentTaxYearEnd = currentTaxYearStart.AddYears(1).AddDays(-1);
        if (currentTaxYearEnd > today)
            currentTaxYearEnd = today;

        DateTime previousTaxYearStart = currentTaxYearStart.AddYears(-1);
        DateTime previousTaxYearEnd = currentTaxYearStart.AddDays(-1);

        if (previousTaxYearStart >= OldestDate)
            taxYears.Add(Tuple.Create(previousTaxYearStart, previousTaxYearEnd));

        taxYears.Add(Tuple.Create(currentTaxYearStart, currentTaxYearEnd));

        return taxYears;
    }

    public Dictionary<string, string> CountryCodes;

    private ObservableCollection<string> _ccodes;

    public ObservableCollection<string> Ccodes
    {
        get => _ccodes;
        set => SetProperty(ref _ccodes, value);
    }

    private ObservableCollection<StringInt> _mcodes;

    public ObservableCollection<StringInt> Mcodes
    {
        get => _mcodes;
        set => SetProperty(ref _mcodes, value);
    }

    private bool _canAddCountry;

    public bool CanAddCountry
    {
        get => _canAddCountry;
        set
        {
            SetProperty(ref _canAddCountry, value);
        }
    }

    private bool _canAddMarket;

    public bool CanAddMarket
    {
        get => _canAddMarket;
        set
        {
            SetProperty(ref _canAddMarket, value);
        }
    }

    private string _selectedCountry;

    public string SelectedCountry
    {
        get => _selectedCountry;
        set
        {
            if (SetProperty(ref _selectedCountry, value))
            {
                if (value == null || value.Equals(CountryCodes["GB"]) || ExtraCountries.Any(p => p.CountryName.Equals(value)))
                {
                    CanAddCountry = false;
                }
                else
                {
                    CanAddCountry = true;
                }
            }
        }
    }

    private StringInt _selectedMarket;

    public StringInt SelectedMarket
    {
        get => _selectedMarket;
        set
        {
            if (SetProperty(ref _selectedMarket, value))
            {
                if (value == null || (ExtraMarkets.Any(p => p.id.Contains(value.Id) && p.CanRemove)))
                {
                    CanAddMarket = false;
                }
                else
                {
                    CanAddMarket = true;
                }
            }
        }
    }

    public ICommand AddCountryCommand { get; set; }
    public ICommand AddMarketPlaceCommand { get; set; }
    public static Dictionary<int, market> AllMarkets;

    public void AddCountryExecute()
    {
        var cvm = new CountryViewModel(SelectedCountry);
        cvm.IsChecked = true;
        string ccode = SelectedCountry.ToString();
        cvm.RemoveCommand = new RelayCommand(() =>
        {
            var ec = ExtraCountries.FirstOrDefault(p => p.CountryName.Equals(ccode));
            ExtraCountries.Remove(ec);
            UpdateSelectedCountriesInSettings(CountryCodes.First(p => p.Value.Equals(ccode)).Key, false);
        });
        ExtraCountries.Add(cvm);
        UpdateSelectedCountriesInSettings(CountryCodes.First(p => p.Value.Equals(ccode)).Key, true);
        SelectedCountry = null;
    }

    private Dictionary<List<int>, string> SelectedMarkets = new();
    private Dictionary<string, currency> Currencies;

    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        ShowWaitingWidget = true;
        var (oldestOrderDate, newestOrderDate) = await _databaseAccessLayer.GetorderDateRangeAsync();

        CountryCodes = await _databaseAccessLayer.kantry();
        Currencies = (await _databaseAccessLayer.Currencies());
        var set = SettingsService.GetSetting("salestatsDateRange").GetValue<string>();
        if (set != null)
        {
            if (set.ToLower().Equals("customdate"))
            {
                SelectedDateOption = DateOptionEnum.CustomDate;
            }
            if (set.ToLower().Equals("lastquarter"))
            {
                SelectedDateOption = DateOptionEnum.LastQuarter;
            }
            if (set.ToLower().Equals("lastfinancialyear"))
            {
                SelectedDateOption = DateOptionEnum.LastFinancialYear;
            }
            if (set.ToLower().Equals("entireperiod"))
            {
                SelectedDateOption = DateOptionEnum.EntirePeriod;
            }
        }

        if (SettingsService.GetSetting("salestatsSelectedCountries") != null)
        {
            var selectedc = SettingsService.GetSetting("salestatsSelectedCountries").GetValue<string>().Split(',').ToList();
            var sc = selectedc.FirstOrDefault(p => p.Equals("world"));
            if (sc != null)
            {
                IsWorldChecked = true;
                selectedc.Remove(sc);
            }
            sc = selectedc.FirstOrDefault(p => p.Equals("uk"));
            if (sc != null)
            {
                IsUKChecked = true;
                selectedc.Remove(sc);
            }
            sc = selectedc.FirstOrDefault(p => p.Equals("eu"));
            if (sc != null)
            {
                IsEUChecked = true;
                selectedc.Remove(sc);
            }
            sc = selectedc.FirstOrDefault(p => p.Equals("euplus"));
            if (sc != null)
            {
                IsEUPlusChecked = true;
                selectedc.Remove(sc);
            }
            foreach (var c in selectedc)
            {
                if (CountryCodes.ContainsKey(c))
                {
                    var cvm = new CountryViewModel(CountryCodes[c]);
                    cvm.IsChecked = true;
                    var ccodekod = c;
                    string ccode = CountryCodes[c];
                    cvm.RemoveCommand = new RelayCommand(() =>
                    {
                        var ec = ExtraCountries.FirstOrDefault(p => p.CountryName.Equals(ccode));
                        ExtraCountries.Remove(ec);
                        UpdateSelectedCountriesInSettings(ccodekod, false);
                    });
                    ExtraCountries.Add(cvm);
                }
            }
        }
        // w allmarkets mam wszystkie uzywane marketplaces
        AllMarkets = (await _databaseAccessLayer.markety()).Where(p => p.Value.IsInUse).ToDictionary(p => p.Key, q => q.Value);
        var Platforms = await _databaseAccessLayer.Platformy();
        var pma = await _databaseAccessLayer.MarketPlatformAssociations();

        foreach (var platforma in Platforms)
        {
            var pmas = pma.Where(p => p.platformID == platforma.Key).ToList();
            var markety = AllMarkets.Where(p => pmas.Select(p => p.marketID).Contains(p.Key)).ToList();
            if (markety.Any())
            {
                SelectedMarkets.Add(markety.Select(p => p.Key).ToList(), platforma.Value.Description);
            }
        }
        foreach (var sm in SelectedMarkets)
        {
            var mvm = new MarketViewModel(sm);
            mvm.CheckChangedCommand = new RelayCommand<MarketViewModel.MarketViewModelProperties>(param =>
            {
                UpdateSelectedMarketsInSettings(param.MarketName, param.IsChecked);
            });
            ExtraMarkets.Add(mvm);
        }

        foreach (var mark in AllMarkets)
        {
            Mcodes.Add(new StringInt(mark.Value.name, mark.Key));
        }

        if (SettingsService.GetSetting("salestatsSelectedMarkets") != null)
        {
            var selectedm = SettingsService.GetSetting("salestatsSelectedMarkets").GetValue<string>().Split(',').ToList();
            List<string> selectedMCopy = new List<string>();
            //from platforms
            foreach (var m in selectedm)
            {
                selectedMCopy.Add(m);
            }
            foreach (var m in selectedm)
            {
                if (ExtraMarkets.Any(p => p.MarketName.Equals(m)))
                {
                    int ind = selectedMCopy.IndexOf(m);
                    selectedMCopy.RemoveAt(ind);
                    ExtraMarkets.First(p => p.MarketName.Equals(m)).IsChecked = true;
                }
            }
            //teraz dodane markety
            foreach (var m in selectedMCopy)
            {
                var mrk = AllMarkets.FirstOrDefault(p => p.Value.name.Equals(m));
                if (mrk.Key != 0)
                {
                    var mvm = new MarketViewModel(new KeyValuePair<List<int>, string>
                        (
                            new List<int> { mrk.Key }, mrk.Value.name)
                    );
                    mvm.IsChecked = true;
                    mvm.CanRemove = true;
                    int id = mrk.Key;
                    mvm.RemoveCommand = new RelayCommand(() =>
                    {
                        var em = ExtraMarkets.FirstOrDefault(p => p.id.First() == id && p.CanRemove);
                        ExtraMarkets.Remove(em);
                        UpdateSelectedMarketsInSettings(em.MarketName, false);
                    });

                    mvm.CheckChangedCommand = new RelayCommand<MarketViewModel.MarketViewModelProperties>(param =>
                    {
                        UpdateSelectedMarketsInSettings(param.MarketName, param.IsChecked);
                    });
                    ExtraMarkets.Add(mvm);
                }
                SelectedMarket = null;
            }
        }

        _dispatcherService.Invoke(() =>
        {
            Ccodes = new ObservableCollection<string>(CountryCodes.Values.OrderBy(p => p));

            OldestDate = oldestOrderDate;
            NewestDate = newestOrderDate;
            IsDataLoaded = true;
            ShowWaitingWidget = false;
        });
    }

    public class MonthItem : ObservableObject
    {
        public string MonthName { get; set; }
        public bool IsSelected { get; set; }
        public byte OrderedNumber { get; set; }
    }
}