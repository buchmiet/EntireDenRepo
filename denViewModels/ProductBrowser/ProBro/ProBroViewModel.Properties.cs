using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.DatabaseAccessLayer;
using denModels;
using denSharedLibrary;
using denViewModels.ProductBrowser.Model;
using shookayNET;

namespace denViewModels.ProductBrowser.ProBro;

public partial class ProBroViewModel:ObservableObject
{
    public bool IsDataLoaded;

    private readonly IDialogService _dialogService;

    private ObservableCollection<Idname> _availableSuppliers;

    private CancellationTokenSource _cancellationTokenSource = new();

    private bool _canHistoryBack;

    private bool _canHistoryForward;

    private int _currentIndex = -1;

    private bool _isBusy;

    private bool _isLogoImageLoaded;

    private bool _isLogoImageLoading;

    private bool _isPackageImageLoaded;

    private bool _isPackageImageLoading;

    private bool _isSupplierComboBoxEnabled;

    private imgWithName _logoImage;

    private imgWithName _packageImage;

    private decimal _price;

    private ObservableCollection<ProductViewItem> _productItems;

    private ObservableCollection<ProductViewItem> _productView = new();

    private string _searchByName;

    private List<string> _searchHistory = [];

    private ProductViewItem _selectedItem;

    private Idname _selectedSupplier;

    private ComboBoxViewModel _brandvm;
    private ComboBoxViewModel _typevm;
    private ShookayWrapper<StringInt> _searchEngine;

    public int LocationId;
    public IDatabaseAccessLayer DatabaseAccessLayer;
    private readonly IFileDialogService _fileDialogService;
    private readonly System.Timers.Timer _saveTimer;
    private readonly IDispatcherService _dispatcherService;
    public AsyncRelayCommand FilterMenuCommand { get; set; }
    public ObservableCollection<ActivityViewModel> ActivityViewModels { get; } = [];
    private readonly ActivityTaskWrapper _activityTaskWrapper;
    public AsyncRelayCommand<int> PopulateCechyValuesCommand { get; }
    public AsyncRelayCommand AddAssociatedMarketsCommand { get; }
    public AsyncRelayCommand<ItemBodiesChangedEventArgs> ItemBodiesChangedCommand { get; }
    public AsyncRelayCommand<int> TypeChangedCommand { get; }
    public AsyncRelayCommand UpdateQuantitiesOnMarketsCommand { get; set; }

    public AsyncRelayCommand SetPriceOnTheWebsiteCommand { get; set; }

    private readonly IMarketActions _marketActions;

    private readonly decimal _vatRate;
    public static string CurrencyName;
    public static string CurrencySymbol;
    public AsyncRelayCommand<FilterEventArgs> HandleFilterEventCommand { get; }
    public ProductFiltersPack MyProductFiltersPack;
    public static int? PendingProductId { get; set; }

    public ICommand AddNewMarketCommand { get; set; }

    public ICommand AddProductCommand { get; set; }

    public ICommand AddSupplierCommand { get; set; }

    public ICommand AdvancedSearchCommand { get; set; }
    public AsyncRelayCommand<CancellationToken> DownloadPhotosIfNecessaryCommand { get; set; }
    private CancellationTokenSource _cts = new();
    public ObservableCollection<ParameterItemViewModel> CechyItems { get; set; } = new ObservableCollection<ParameterItemViewModel>();
    public ICommand ChangeSupplierCommand { get; set; }
    public ICommand ClearSearchResultsCommand { get; set; }
    public ICommand DeleteLogoCommand { get; set; }
    public ICommand DeletePackageCommand { get; set; }
    public ICommand DeleteProductCommand { get; set; }
    public ICommand KeyDownCommand { get; }
    public ICommand LeftArrowCommand { get; private set; }
    public ICommand LoadDataCommand { get; }
    public ICommand NextCommand { get; }
    private ObservableCollection<PhotoViewModel> _photos;
    public ICommand PreviousCommand { get; }

    public ICommand PrintLabelCommand { get; set; }

    public ICommand PublishProductCommand { get; set; }

    public ICommand RemoveSupplierCommand { get; set; }

    public ICommand RightArrowCommand { get; private set; }

    public ICommand RowClickCommand { get; private set; }

    public ICommand SaveImageCommand { get; private set; }

    public ICommand SaveProductCommand { get; set; }

    private SimpleDescription _simpleDescription = new();
    public SimpleDescription SimpleDescription
    {
        get => _simpleDescription;
        set => SetProperty(ref _simpleDescription, value);
    }
    //public ObservableCollection<IBaseFieldViewModel> TextFieldItems { get; set; } = [];
    public ICommand TurnTrackOnOffCommand { get; set; }
    public ICommand UpdateQuantityForSeveralCommand { get; set; }
    public AsyncRelayCommand OnSearchChangedCommand { get; set; }
    public ICommand UploadLogoCommand { get; set; }
    public ICommand UploadPackageCommand { get; set; }
    public ICommand UploadRegPhotoCommand { get; set; }
    public ICommand ViewLogsCommand { get; set; }
    private FilterProductsViewModel _filterForProducts;
    private readonly BrandsSuppliersMarkets _brandsSuppliersMarkets = new();

       

    private bool _canPrintLabels ;
    public bool AreAnyFieldsModified => CechyItems.Any(item => item.HasChanged) || SimpleDescription.Items.Any(item => item.HasChanged);//  TextFieldItems.Any(item => item.HasChanged);

    public ICommand AssignNewSupplierCommand { get; set; }
    public ObservableCollection<AssociatedMarketsViewModel> AssociatedMarkets { get; set; } = [];

    public FilterProductsViewModel FilterForProducts
    {
        get => _filterForProducts;
        set => SetProperty(ref _filterForProducts, value);
    }
    public ObservableCollection<Idname> AvailableSuppliers
    {
        get => _availableSuppliers;
        set => SetProperty(ref _availableSuppliers, value);
    }

    public bool CanHistoryBack
    {
        get => _canHistoryBack;
        set => SetProperty(ref _canHistoryBack, value);
    }

    public bool CanHistoryForward
    {
        get => _canHistoryForward;
        set => SetProperty(ref _canHistoryForward, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (!SetProperty(ref _isBusy, value)) return;
            OnPropertyChanged(nameof(IsNotBusy));
            ((AsyncRelayCommand)AddProductCommand).NotifyCanExecuteChanged();
        }
    }

    public bool IsLogoImageLoaded
    {
        get => _isLogoImageLoaded;
        set => SetProperty(ref _isLogoImageLoaded, value);
    }

    public bool IsLogoImageLoading
    {
        get => _isLogoImageLoading;
        set => SetProperty(ref _isLogoImageLoading, value);
    }

    public bool IsNotBusy => !_isBusy;

    public bool IsPackageImageLoaded
    {
        get => _isPackageImageLoaded;
        set => SetProperty(ref _isPackageImageLoaded, value);
    }

    public bool IsPackageImageLoading
    {
        get => _isPackageImageLoading;
        set => SetProperty(ref _isPackageImageLoading, value);
    }

    public bool IsSupplierComboBoxEnabled
    {
        get => _isSupplierComboBoxEnabled;
        set => SetProperty(ref _isSupplierComboBoxEnabled, value);
    }

    public imgWithName LogoImage
    {
        get => _logoImage;
        set
        {
      
            if (value is null)
            {
                IsLogoImageLoaded = true;
                IsLogoImageLoading = false;
                SetProperty(ref _logoImage, value);
                return;
            }
            if (LogoImage is not null && value is  null && value.itembodyid == SelectedItem.Id && value.name == LogoImage.name)
                return;

            IsLogoImageLoaded = true;
            IsLogoImageLoading = false;
            SetProperty(ref _logoImage, value);
        }
    }

    public imgWithName PackageImage
    {
        get => _packageImage;
        set
        {
            if (PackageImage is not null && value is not null && value.itembodyid == SelectedItem.Id && value.name == PackageImage.name)
                return;

            SetProperty(ref _packageImage, value);
        }
    }

    public ObservableCollection<PhotoViewModel> Photos
    {
        get => _photos;
        set => SetProperty(ref _photos, value);
    }

    public string PriceInput
    {
        get => _price == 0 ? "" : _price.ToString("F2");
        set
        {
            if (decimal.TryParse(value, out decimal newValue))
            {
                SetProperty(ref _price, newValue);
            }
        }
    }

    public string SearchByName
    {
        get => _searchByName;
        set
        {
            if (SetProperty(ref _searchByName, value))
            {
                _ = OnSearchChangedCommand.ExecuteAsync(null);
            }
        }
    }

    public List<string> SearchHistory
    {
        get => _searchHistory;
        set => SetProperty(ref _searchHistory, value);
    }

    public ObservableCollection<ProductViewItem> ProductItems
    {
        get => _productItems;
        set => SetProperty(ref _productItems, value);
    }

    public ObservableCollection<ProductViewItem> ProductView
    {
        get => _productView;
        set => SetProperty(ref _productView, value);
    }

    public Idname SelectedSupplier
    {
        get => _selectedSupplier;
        set
        {
            if (value is null) return;

            if (SetProperty(ref _selectedSupplier, value))
            {
                AddValuesFromHeader(DatabaseAccessLayer.items[SelectedItem.Id].ItemHeaders.First(p => p.supplierID == value.Id));
            }
        }
    }

    private double _logoProgressBar;

    public double LogoProgressBar
    {
        get => _logoProgressBar;
        set => SetProperty(ref _logoProgressBar, value);
    }

    private double _packageProgressBar;

    public double PackageProgressBar
    {
        get => _packageProgressBar;
        set => SetProperty(ref _packageProgressBar, value);
    }

    public bool CanPrintLabels
    {
        get => _canPrintLabels;
        set => SetProperty(ref _canPrintLabels, value);
    }


    private bool _filteringProducts ;

    public bool FilteringProducts
    {
        get => _filteringProducts;
        set => SetProperty(ref _filteringProducts, value);
    }
    private string _filterButtonText = FilterProductsButton.InitialText;

    public string FilterButtonText
    {
        get => _filterButtonText;
        set => SetProperty(ref _filterButtonText, value);
    }

      
}