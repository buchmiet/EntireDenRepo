using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80;
using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Models;
using denQuickbooksNET80.Models;
using denSharedLibrary;
using denSignalRClient;
using denViewModels;
using denViewModels.LabelManager;
using denViewModels.ProductBrowser.ProBro;
using denWPFSharedLibrary;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrdBroMVVM;
using ParametersMVVM;
using PngAnimator;
using ProBroMVVM;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TypesMVVM;
using WatchExplorerMVVM;


namespace denMVVM;

public class MainWindowViewModel : ObservableObject
{
    // Klucze dla UserControls, aby uniknąć "magicznych stringów"
    private readonly string CurOrdViewKey = "CurOrd";
    private readonly string ProBroViewKey = "ProBro";
    private readonly string OrdBroViewKey = "OrdBro";
    private readonly string WatchExplorerViewKey = "WatchExplorer";
    private readonly string ColourBrowserViewKey = "ColourBrowser";
    private readonly string MayAlsoFitViewKey = "MayAlsoFit";
    private readonly string DeliveriesViewKey = "Deliveries";
    private readonly string StorageExplorerViewKey = "StorageExplorer";
    private readonly string TypesViewKey = "Types";
    private readonly string ParametersViewKey = "Parameters";
    private readonly string LabelsViewKey = "Labels";
    private readonly string BusinessDetailsViewKey = "BusinessDetails";
    private readonly string SalesSummaryViewKey = "SalesSummary";

    public Dictionary<string, UserControl> UserControls = new();
    public int LocationId = 1;

    private bool _isVerificationPanelVisible;
    private UserControl _selectedViewModel;
    private readonly App.GuiClientSettings _guiClientSettings;

    // Wstrzyknięte zależności
    private readonly ViewModelFactory _viewModelFactory;
    private readonly IDialogService _dialogService;
    private readonly IClipboardService _clipboardService;
    private readonly IDispatcherTimerFactory _dispatcherTimerFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabaseAccessLayer _databaseAccessLayer;
    private readonly IFileDialogService _fileDialogService;

    public MainWindowViewModel(
        IOptions<App.GuiClientSettings> guiClientSettings,
        ViewModelFactory viewModelFactory,
        IDialogService dialogService,
        IClipboardService clipboardService,
        IDispatcherTimerFactory dispatcherTimerFactory,
        IHttpClientFactory httpClientFactory,
        IDatabaseAccessLayer databaseAccessLayer,
        IFileDialogService fileDialogService)
    {
        // Przypisanie wstrzykniętych serwisów do pól prywatnych
        _guiClientSettings = guiClientSettings.Value;
        _viewModelFactory = viewModelFactory;
        _dialogService = dialogService;
        _clipboardService = clipboardService;
        _dispatcherTimerFactory = dispatcherTimerFactory;
        _httpClientFactory = httpClientFactory;
        _databaseAccessLayer = databaseAccessLayer;
        _fileDialogService = fileDialogService;

        // Inicjalizacja komend
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        ShowCurOrdCommand = new RelayCommand(ShowCurOrdControl);
        ShowProBroCommand = new RelayCommand(ShowProBroControl);
        ShowOrdBroCommand = new RelayCommand(ShowOrdBroControl);
        ShowWECommand = new RelayCommand(ShowWEControl);
        ShowCBCommand = new RelayCommand(ShowCbControl);
        ShowMAFCommand = new RelayCommand(ShowMafControl);
        ShowDeliveriesCommand = new RelayCommand(ShowDeliveriesControl);
        ShowStorageCommand = new RelayCommand(ShowStorageControl);
        ShowTypesCommand = new RelayCommand(ShowTypesControl);
        ShowParametersCommand = new RelayCommand(ShowParametersControl);
        ShowLabelsCommand = new RelayCommand(ShowLabelsControl);
        ShowBusinessDetailsCommand = new RelayCommand(ShowBusinessDetailsControl);
        ShowSalesSummaryCommand = new RelayCommand(ShowSalesSummaryControl);

 
        var wpfTimeoutWarning = new WpfConnectionTimeoutWarning();
        CurOrdViewModel._clipboardService = _clipboardService;
        var wpfMessageDialog = new WpfMessageDialog(_dialogService);

        var databaseOperationMediator = new DatabaseOperationMediator
        {
            TimeoutWarning = wpfTimeoutWarning,
            MessageDialog = wpfMessageDialog
        };
        DatabaseOperationExecutor.Instance.SetMediator(databaseOperationMediator);
        PngAnimatorControl.dispatcherTimer = _dispatcherTimerFactory.Create();
    }

    public bool IsVerificationPanelVisible
    {
        get => _isVerificationPanelVisible;
        set => SetProperty(ref _isVerificationPanelVisible, value);
    }

    public ICommand LoadDataCommand { get; }

    public UserControl SelectedViewModel
    {
        get => _selectedViewModel;
        set => SetProperty(ref _selectedViewModel, value);
    }
    public ICommand ShowBusinessDetailsCommand { get; private set; }
    public ICommand ShowCBCommand { get; private set; }
    public ICommand ShowCurOrdCommand { get; private set; }
    public ICommand ShowDeliveriesCommand { get; private set; }
    public ICommand ShowLabelsCommand { get; private set; }
    public ICommand ShowMAFCommand { get; private set; }
    public ICommand ShowOrdBroCommand { get; private set; }
    public ICommand ShowParametersCommand { get; private set; }
    public ICommand ShowProBroCommand { get; private set; }
    public ICommand ShowSalesSummaryCommand { get; private set; }
    public ICommand ShowStorageCommand { get; private set; }
    public ICommand ShowTypesCommand { get; private set; }
    public ICommand ShowWECommand { get; private set; }

    private async Task LoadDataAsync()
    {
        IsVerificationPanelVisible = true;
        await VerifyUserCredentials();
        IsVerificationPanelVisible = false;
    }

    private void OnOrderSelected(int OrderId)
    {
        if (!UserControls.TryGetValue(OrdBroViewKey, out var userControl))
        {
            OrdBroViewModel.PendingOrderId = OrderId;
            var viewModel = _viewModelFactory.Create<OrdBroViewModel>();
            viewModel.PrintInvoiceEvent += OnPrintInvoiceEvent;
            viewModel.ProductSelected += OnProductSelected;
            var control = new OrdBroUserControl(_databaseAccessLayer) 
            {
                DataContext = viewModel
            };

            UserControls[OrdBroViewKey] = control;
        }
        else
        {
            var ordBroControl = (OrdBroUserControl)userControl;
            var ordBroViewModel = (OrdBroViewModel)ordBroControl.DataContext;
            ordBroViewModel.ShowOrder(OrderId);
        }

        SelectedViewModel = UserControls[OrdBroViewKey];
    }

    private void OnPrintInvoiceEvent(Complete komplet)
    {
        var curOrdViewModel = (UserControls[CurOrdViewKey].DataContext as CurOrdViewModel);
        curOrdViewModel?.PrintInvoice(komplet);
    }

    private void OnProductSelected(int productId)
    {
        if (!UserControls.TryGetValue(ProBroViewKey, out var userControl))
        {
            ProBroViewModel.PendingProductId = productId;
            ProBroViewModel viewModel = _viewModelFactory.Create<ProBroViewModel>();
            var control = new ProBroMVVM.ProBroUserControl
            {
                DataContext = viewModel
            };

            UserControls[ProBroViewKey] = control;
        }
        else
        {
            var proBroControl = (ProBroUserControl)userControl;
            var proBroViewModel = (ProBroViewModel)proBroControl.DataContext;
            proBroViewModel.ShowProduct(productId);
        }
        SelectedViewModel = UserControls[ProBroViewKey];
    }

    private void ShowBusinessDetailsControl()
    {
        if (!UserControls.ContainsKey(BusinessDetailsViewKey))
        {
            var viewModel = _viewModelFactory.Create<BusinessDetailsViewModel>();

            var control = new BusinessDetailsControl.BusinessDetailsUserControl
            {
                DataContext = viewModel
            };
            UserControls[BusinessDetailsViewKey] = control;
        }
        SelectedViewModel = UserControls[BusinessDetailsViewKey];
    }

    private void ShowCbControl()
    {
        if (!UserControls.ContainsKey(ColourBrowserViewKey))
        {
            var viewModel = _viewModelFactory.Create<ColourBrowserMVVM.ColourBrowserViewModel>();

            var control = new ColourBrowserMVVM.ColourBrowserUserControl
            {
                DataContext = viewModel
            };

            UserControls[ColourBrowserViewKey] = control;
        }

        SelectedViewModel = UserControls[ColourBrowserViewKey];
    }

    private void ShowCurOrdControl()
    {
        if (!UserControls.ContainsKey(CurOrdViewKey))
        {
            var viewModel = _viewModelFactory.Create<CurOrdViewModel>();
            viewModel.ProductSelected += OnProductSelected;
            viewModel.OrderSelected += OnOrderSelected;
            var control = new CurOrdMVVM.CurOrdUserControl
            {
                DataContext = viewModel
            };

            UserControls[CurOrdViewKey] = control;
        }

        SelectedViewModel = UserControls[CurOrdViewKey];
    }

    private void ShowDeliveriesControl()
    {
        if (!UserControls.ContainsKey(DeliveriesViewKey))
        {
            var viewModel = _viewModelFactory.Create<CasioUKDeliveriesMVVM.CasioUKDeliveriesViewModel>();

            var control = new CasioUKDeliveriesMVVM.CasioUKDeliveriesUserControl
            {
                DataContext = viewModel
            };
            UserControls[DeliveriesViewKey] = control;
        }
        SelectedViewModel = UserControls[DeliveriesViewKey];
    }

    private void ShowLabelsControl()
    {
        if (!UserControls.ContainsKey(LabelsViewKey))
        {
            var viewModel = _viewModelFactory.Create<BothLabelsContainerViewModel>();
            var control = new denLabelMVVM.BothLabelsContainerControl
            {
                DataContext = viewModel
            };
            UserControls[LabelsViewKey] = control;
        }
        SelectedViewModel = UserControls[LabelsViewKey];
    }

    private void ShowMafControl()
    {
        if (!UserControls.ContainsKey(MayAlsoFitViewKey))
        {
            var viewModel = _viewModelFactory.Create<MayAlsoFitMVVM.MayAlsoFitViewModel>();

            var control = new MayAlsoFitMVVM.MayAlsoFitUserControl
            {
                DataContext = viewModel
            };

            UserControls[MayAlsoFitViewKey] = control;
        }

        SelectedViewModel = UserControls[MayAlsoFitViewKey];
    }

    private void ShowOrdBroControl()
    {
        if (!UserControls.ContainsKey(OrdBroViewKey))
        {
            var viewModel = _viewModelFactory.Create<OrdBroViewModel>();
            viewModel.PrintInvoiceEvent += OnPrintInvoiceEvent;
            viewModel.ProductSelected += OnProductSelected;
            var control = new OrdBroUserControl(_databaseAccessLayer) 
            {
                DataContext = viewModel
            };

            UserControls[OrdBroViewKey] = control;
        }
        SelectedViewModel = UserControls[OrdBroViewKey];
    }

    private void ShowParametersControl()
    {
        if (!UserControls.ContainsKey(ParametersViewKey))
        {
            var viewModel = _viewModelFactory.Create<PrametersViewModel>();

            var control = new ParametersUserControl
            {
                DataContext = viewModel
            };
            UserControls[ParametersViewKey] = control;
        }
        SelectedViewModel = UserControls[ParametersViewKey];
    }

    private void ShowProBroControl()
    {
        if (!UserControls.ContainsKey(ProBroViewKey))
        {
            var viewModel = _viewModelFactory.Create<ProBroViewModel>();

            var control = new ProBroUserControl
            {
                DataContext = viewModel
            };

            UserControls[ProBroViewKey] = control;
        }
        SelectedViewModel = UserControls[ProBroViewKey];
    }

    private void ShowSalesSummaryControl()
    {
        if (!UserControls.ContainsKey(SalesSummaryViewKey))
        {
            var viewModel = _viewModelFactory.Create<SalesSummaryViewModel>();

            var control = new SalesSummaryControl.SaleSummaryControl
            {
                DataContext = viewModel
            };
            UserControls[SalesSummaryViewKey] = control;
        }
        SelectedViewModel = UserControls[SalesSummaryViewKey];
    }

    private void ShowStorageControl()
    {
        if (!UserControls.ContainsKey(StorageExplorerViewKey))
        {
            var viewModel = _viewModelFactory.Create<StorageExplorerMVVM.StorageExplorerViewModel>();

            var control = new StorageExplorerMVVM.StorageExplorerUserControl
            {
                DataContext = viewModel
            };
            UserControls[StorageExplorerViewKey] = control;
        }

        SelectedViewModel = UserControls[StorageExplorerViewKey];
    }

    private void ShowTypesControl()
    {
        if (!UserControls.ContainsKey(TypesViewKey))
        {
            var viewModel = _viewModelFactory.Create<TypesViewModel>();
            var control = new TypesUserControl
            {
                DataContext = viewModel
            };
            UserControls[TypesViewKey] = control;
        }
        SelectedViewModel = UserControls[TypesViewKey];
    }

    private void ShowWEControl()
    {
        if (!UserControls.ContainsKey(WatchExplorerViewKey))
        {
            var viewModel = _viewModelFactory.Create<WatchExplorerViewModel>();
            viewModel.ProductSelected += OnProductSelected;
            var control = new WatchExplorerUserControl
            {
                DataContext = viewModel
            };
            UserControls[WatchExplorerViewKey] = control;
        }

        SelectedViewModel = UserControls[WatchExplorerViewKey];
    }

    public void LogInForExternalLibs(string input)
    {
        Log.Error(input);
    }

    private async Task VerifyUserCredentials()
    {
        var dc = new denClient(_guiClientSettings.Username, _guiClientSettings.Password,
            _httpClientFactory, LogInForExternalLibs); 
        bool connected = await dc.Connect();
        var sd = await dc.GetSensitiveData();
        if (connected && sd.UserId != -1)
        {
            DbContextFactory.SetDatabaseConfiguration(sd.DBHost, sd.DBPort, sd.DBUserName, sd.DBPassword, sd.DBname);
            ShowCurOrdControl();
        }
    }
}
