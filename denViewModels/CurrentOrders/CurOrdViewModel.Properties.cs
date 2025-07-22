using denSharedLibrary;
using denModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ColoursOperations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.DatabaseAccessLayer;

namespace denViewModels;

public partial class CurOrdViewModel : ObservableObject
{
    public static Dictionary<string, byte[]> flagi;
    public static Dictionary<int, byte[]> kolorki;
    public List<string> ScottishPostCodes =
    [
        "eh", "ab", "pa", "ky", "ph", "dd", "fk", "dg", "ka", "ml", "iv", "kw", "ze", "hs", "td", "g1", "g2", "g3",
        "g4", "g5", "g9", "g11", "g12", "g13", "g14", "g15", "g20", "g21", "g22", "g23", "g31", "g32", "g33", "g34",
        "g40", "g41", "g42", "g43", "g44", "g45", "g46", "g51", "g52", "g53", "g58", "g60", "g61", "g62", "g63",
        "g64", "g65", "g66", "g67", "g68", "g69", "g70", "g71", "g72", "g73", "g74", "g75", "g76", "g77", "g78",
        "g79", "g81", "g82", "g83", "g84", "g90"
    ];
    public List<StringInt> orderSortingValues =
    [
        new(denLanguageResourses.Resources.Time,0),
        new(denLanguageResourses.Resources.Location,1)

    ];



    public IDatabaseAccessLayer _databaseAccessLayer;
    public MyTerminal Konsola;
    public static IClipboardService _clipboardService;
    private readonly IDialogService _dialogService;
    private readonly IFileDialogService _fileDialogService;
    private denSharedLibrary.Color _borderColor;
    private bool _changeStatusBarVisibility;
    private string _changeStatusButtonText;
    private bool _changeStatusButtonVisibility = true;
    readonly IColourOpsMediator _colourOpsMediator;
    readonly ICompletesToXpsStream _completesToXpsStream;
    readonly ICurrentKomplety _currentKomplety;
    private bool _fetchOrdersBarVisibility;
    private string _fetchOrdersButtonText;
    private bool _isAOBRunning;
    private bool _isBusy;
    private bool _isComboBoxEnabled;
    private bool _isDataLoaded = false;
    private bool _isFetchOrdersProcessing;
    private bool _isUpdatingAll = false;
    private ObservableCollection<DataViewItem> _orders;
    readonly IOrdersSummaryToXpsStream _ordersSummaryToXpsStream;
    private ObservableCollection<string> _printers;
    private bool _printInvoicesBarVisibility;
    private string _printInvoicesButtonText;
    private bool _selectAll;
    private string _selectedPrinter;
    private string _soldMonth;
    private string _soldToday;
    private string _soldYday;
    private bool _someOrdersAreIn;
    private ITerminalScreenViewModel _terminalScreenViewModel;
    readonly IXpsPrinter _xpsPrinter;
    public int locationID;
    public static string currencyName;
    public static string currencySymbol;
    private readonly IMarketActions _marketActions;
    public ObservableCollection<StringInt> OrderSortingItems { get; set; }

    private StringInt _selectedOrderSorthingMethod;
    public StringInt SelectedOrderSorthingMethod
    {
        get => _selectedOrderSorthingMethod;
        set
        {
            if (SetProperty(ref _selectedOrderSorthingMethod, value))
            {
                SettingsService.UpdateSetting("SelectedOrderSorthingMethod", value.Id.ToString());
            }

        }
    }

    public void HandleLargeLabelPrinterChanged(string drukarka)
    {
        if (drukarka != null)
        {
            IsLargeLabelPrinterEnabled = true;
        }
        else
        {
            IsLargeLabelPrinterEnabled = false;
        }
    }

    private bool _isLargeLabelPrinterEnabled;
    public bool IsLargeLabelPrinterEnabled
    {
        get => _isLargeLabelPrinterEnabled;
        set => SetProperty(ref _isLargeLabelPrinterEnabled, value);
    }


    public void DeselectOrder()
    {
        SelectedOrder = null;
    }
    public event Action<int> OrderSelected;

    public event Action<int> ProductSelected;



    private DataViewItem _selectedOrder;
    public DataViewItem SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (SetProperty(ref _selectedOrder, value))
            {


            }
        }
    }


    public RelayCommand HideDetailsCommand { get; set; }

    public ICommand AOBCommand { get; }
    public denSharedLibrary.Color BorderColor
    {
        get => _borderColor;
        set => SetProperty(ref _borderColor, value);
    }
    public bool ChangeStatusBarVisibility
    {
        get => _changeStatusBarVisibility;
        set => SetProperty(ref _changeStatusBarVisibility, value);
    }
    public string ChangeStatusButtonText
    {
        get => _changeStatusButtonText;
        set => SetProperty(ref _changeStatusButtonText, value);
    }

    public bool ChangeStatusButtonVisibility
    {
        get { return _changeStatusButtonVisibility && PrintMeDependentButtonsOn; }
        set => SetProperty(ref _changeStatusButtonVisibility, value);
    }

    public ICommand ChangeStatusesCommand { get; private set; }

    public ICommand CNDCommand { get; }
    public bool FetchOrdersBarVisibility
    {
        get => _fetchOrdersBarVisibility;
        set => SetProperty(ref _fetchOrdersBarVisibility, value);
    }








    public string FetchOrdersButtonText
    {
        get => _fetchOrdersButtonText;
        set => SetProperty(ref _fetchOrdersButtonText, value);
    }

    public ICommand FetchOrdersCommand { get; }

    public ICommand FlipAllCommand { get; }

    public ICommand FlipNewCommand { get; }

    public bool IsAOBRunning
    {
        get => !_isAOBRunning;
        private set => SetProperty(ref _isAOBRunning, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public bool IsComboBoxEnabled
    {
        get => _isComboBoxEnabled;
        set => SetProperty(ref _isComboBoxEnabled, value);
    }

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set => SetProperty(ref _isDataLoaded, value);
    }

    public bool IsFetchOrdersProcessing
    {
        get => !_isFetchOrdersProcessing;
        private set => SetProperty(ref _isFetchOrdersProcessing, value);
    }

    public ICommand LoadDataCommand { get; }


    public ICommand NextThemeCommand { get; private set; }

    public ObservableCollection<DataViewItem> Orders
    {
        get => _orders;
        set => SetProperty(ref _orders, value);
    }

    public ICommand PrintCN22sCommand { get; set; }

    public ObservableCollection<string> Printers
    {
        get => _printers;
        set => SetProperty(ref _printers, value);
    }
    public bool PrintInvoicesBarVisibility
    {
        get => _printInvoicesBarVisibility;
        set => SetProperty(ref _printInvoicesBarVisibility, value);
    }

    public string PrintInvoicesButtonText
    {
        get => _printInvoicesButtonText;
        set => SetProperty(ref _printInvoicesButtonText, value);
    }

    public ICommand PrintInvoicesCommand { get; }

    public bool PrintMeDependentButtonsOn
    {
        get
        {
            return Orders.Any(o => o.PrintMe);
        }
    }
    public ICommand PrintSummaryCommand { get; private set; }



    public ICommand RowDoubleClickCommand { get; }
    public ICommand ProductClickCommand { get; }

    public ICommand SelectAllAmazonCommand { get; }

    public ICommand SelectAllEbayCommand { get; }

    public ICommand SelectAllNewCommand { get; }

    public string SelectedPrinter
    {
        get => _selectedPrinter;
        set
        {
            if (SetProperty(ref _selectedPrinter, value))
            {
                SettingsService.UpdateSetting("default_printer", SelectedPrinter);
            }
            CheckIfPrintIsOk();
        }
    }

    private bool _canPrint = false;
    public bool CanPrint
    {
        get => _canPrint;
        set => SetProperty(ref _canPrint, value);
    }


    public string SoldMonth
    {
        get => _soldMonth;
        set => SetProperty(ref _soldMonth, value);
    }

    public string SoldToday
    {
        get => _soldToday;
        set => SetProperty(ref _soldToday, value);
    }

    public string SoldYday
    {
        get => _soldYday;
        set => SetProperty(ref _soldYday, value);
    }

    public bool SomeOrdersAreIn
    {
        get => _someOrdersAreIn;
        set => SetProperty(ref _someOrdersAreIn, value);
    }

    public ICommand SortingCommand { get; }

    public ITerminalScreenViewModel TerminalScreenViewModelProperty
    {
        get => _terminalScreenViewModel;
        set => SetProperty(ref _terminalScreenViewModel, value);
    }

    readonly List<string> _euCountries =
    [
        "AT", "BE", "BG", "CY", "CZ", "HR",
        "DK", "EE", "FI", "FR", "DE", "GR",
        "HU", "IT", "IE", "LT", "LV", "LU",
        "MT", "NL", "PL", "RO", "PT", "SK",
        "SI", "ES", "SE"
    ];
    public AsyncRelayCommand<CompletesChangedEventArgs> CompeletesChangedCommand { get; set; }
}