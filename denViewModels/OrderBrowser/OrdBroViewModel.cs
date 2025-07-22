using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Models;
using denLanguageResourses;
using denModels;
using denQuickbooksNET80;
using denSharedLibrary;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SettingsKeptInFile;
using shookayNET;

namespace denViewModels;

public class OrdBroViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private string searchInput;

    public delegate void PrintInvoiceEventHandler(Complete komplet);

    public event PrintInvoiceEventHandler PrintInvoiceEvent;

    public void OnPrintInvoiceEvent(Complete komplet)
    {
        PrintInvoiceEvent?.Invoke(komplet);
    }

    public void ExecutePrintInvoice()
    {
        OnPrintInvoiceEvent(CompletesActionsObject.AllOrders[SelectedItem.id]);
    }

    public string SearchInput
    {
        get => searchInput;
        set
        {
            searchInput = value;
            OnPropertyChanged();
            Task.Run(OnSearchChanged);
        }
    }

    private ObservableCollection<CompleteOrderViewItem> _orders;

    public ObservableCollection<CompleteOrderViewItem> Orders
    {
        get => _orders;
        set => SetProperty(ref _orders, value);
    }

    private ObservableCollection<CompleteOrderViewItem> _ordersView;

    public ObservableCollection<CompleteOrderViewItem> OrdersView
    {
        get => _ordersView;
        set => SetProperty(ref _ordersView, value);
    }

    private bool _ordersIn;

    public bool OrdersIn
    {
        get => _ordersIn;
        set => SetProperty(ref _ordersIn, value);
    }

    public string SyncWithQbButtonText { get; set; }

    private bool _syncWithQb;

    public bool SyncWithQb
    {
        get => _syncWithQb;
        set
        {
            SetProperty(ref _syncWithQb, value);
            SyncWithQbButtonText = value ? "ON" : "OFF";
            OnPropertyChanged(nameof(SyncWithQbButtonText));
        }
    }

    private int _displayedOrderId;

    public int DisplayedOrderId
    {
        get => _displayedOrderId;
        set => SetProperty(ref _displayedOrderId, value);
    }

    private string _displayedMarket;

    public string DisplayedMarket
    {
        get => _displayedMarket;
        set => SetProperty(ref _displayedMarket, value);
    }

    public static int? PendingOrderId { get; set; }
    public ICommand SaveOrderCommand { get; }
    public ICommand PrintLabelCommand { get; }
    public ICommand PrintInvoiceCommand { get; }
    public ICommand Add2QuickBooksCommand { get; }
    public ICommand Remove4QuickBooksCommand { get; }
    public ICommand SaveAddressCommand { get; }
    public ICommand SaveCustomerCommand { get; }

    public ICommand LoadDataCommand { get; }
    public ICommand GetMarkets2PlatformNamesCommand { get; }
    public ICommand RowClickCommand { get; }
    public ICommand ProductRowClickCommand { get; }
    public ICommand SaveAmazonDetailsCommand { get; }
    private CancellationTokenSource _cancellationTokenSource;
    public IDatabaseAccessLayer DatabaseAccessLayer;
    public CompletesActions CompletesActionsObject;
    private readonly ICurrentKomplety _currentKomplety;
    private readonly IDispatcherService _dispatcherService;
    private readonly IQuickBooksService _quickBooksService;
    public int locationID;


    private readonly ILogger<OrdBroViewModel> _logger;

    public OrdBroViewModel(IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer, ICurrentKomplety currentKomplety, 
        IDispatcherService dispatcherService, IQuickBooksService quickBooksService, ILogger<OrdBroViewModel> logger,ISettingsService settingsService )
    {

        _logger = logger;
        _quickBooksService = quickBooksService;
        _dispatcherService = dispatcherService;
        _currentKomplety = currentKomplety;
        _cancellationTokenSource = new CancellationTokenSource();
        DatabaseAccessLayer = databaseAccessLayer;
        Orders = new ObservableCollection<CompleteOrderViewItem>();
        OrdersView = new ObservableCollection<CompleteOrderViewItem>();
        SaveOrderCommand = new AsyncRelayCommand(ExecuteSaveGeneralOrderDetails);
        PrintLabelCommand = new AsyncRelayCommand(ExecutePrintAddress);
        PrintInvoiceCommand = new RelayCommand(ExecutePrintInvoice);
        Add2QuickBooksCommand = new AsyncRelayCommand(ExecuteAddToQuickBooks);
        Remove4QuickBooksCommand = new AsyncRelayCommand(ExecuteRemoveFromQuickBooks);
        SaveAddressCommand = new AsyncRelayCommand(ExecuteSaveRegAddress);
        SaveCustomerCommand = new AsyncRelayCommand(ExecuteSaveRegCustDetails);
        SaveAmazonDetailsCommand = new AsyncRelayCommand(ExecuteSaveAmazonDetails);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        RowClickCommand = new RelayCommand<CompleteOrderViewItem>(ExecuteRowClick);
        ProductRowClickCommand = new RelayCommand<ItemViewItem>(ExecuteProductRowClick);
        GetMarkets2PlatformNamesCommand = new AsyncRelayCommand(GetMarkets2PlatformNamesExecute);
        _dialogService = dialogService;
        CompletesActionsObject = new CompletesActions(DatabaseAccessLayer, _currentKomplety);
        CompletesActionsObject.CompletesChanged += HandleCompleteChanged;
        locationID = settingsService.LocationId;
        //   Messenger.Subscribe<List<Complete>>(CompletesActionsObject.HandleCompleteChange);
        // AllocConsole();
        _logger.LogInformation("OrdBroViewModel constructor");
    }

    public async Task<bool> Add2QuickBooks(Complete zamo)
    {
           
        //  var er = await Xrates.getXrate(zamo.Order.paidOn, zamo.Order.currency);
        var koko = await _quickBooksService.ConvertMyInvoiceToQuickbooksInvoice(zamo, (await DatabaseAccessLayer.orderItemTypes()).First(p => p.Value.name.ToLower().Equals("item")).Value, emptyShot);

        //  var ttt = await _quickBooksService.AddInvoice(koko, emptyShot, zamo.Order.xchgrate);
        var invoiceResponse = await  _quickBooksService.AddInvoice(koko, Emptyshot, zamo.Order.xchgrate);
        if (invoiceResponse is null)//(await Trier.ResponseHelper.ResponseHelper.TryAsync(() => _quickBooksService.AddInvoice(koko, emptyshot, zamo.Order.xchgrate))).Item1)
        {
            _logger.LogError("returnResponse.Error(invoiceResponse).GetMessages()");
            return false;
        }

        var itx = CompletesActionsObject.invtxns[zamo.Order.orderID];
        itx.qbInvoiceId = invoiceResponse.Id;
        await DatabaseAccessLayer.UpdateInvoiceTXN(itx);
        var order = CompletesActionsObject.AllOrders[zamo.Order.orderID].Order;
        order.quickbooked = true;
        await DatabaseAccessLayer.Updateorder(order);
        CompletesActionsObject.invtxns[zamo.Order.orderID] = itx;
        CompletesActionsObject.AllOrders[zamo.Order.orderID].Order = order;
        return true;
    }

    public async Task<bool> Remove4QuickBooks(int orderid, int invid)
    {
        var te = await _quickBooksService.DeleteInvoice(invid, emptyShot);
        if (te == false)
        {
            return false;
        }
        var order = CompletesActionsObject.AllOrders[orderid].Order;
        order.quickbooked = false;
        await DatabaseAccessLayer.Updateorder(order);

        return true;
    }

    public static void emptyShot(string we)
    {
    }

    public async Task ExecutePrintAddress()
    {
        string adres = "";
        var komplet = CompletesActionsObject.AllOrders[SelectedItem.id];
        if (!komplet.Customer.Title.Equals("")) adres += komplet.Customer.Title + ' ';
        if (!komplet.Customer.GivenName.Equals("")) adres += komplet.Customer.GivenName + ' ';
        if (!string.IsNullOrEmpty(komplet.Customer.MiddleName)) adres += komplet.Customer.MiddleName + ' ';
        if (!komplet.Customer.FamilyName.Equals("")) adres += komplet.Customer.FamilyName + ' ';
        adres += Environment.NewLine;
        if (!komplet.BillAddr.Line1.Equals("")) adres += komplet.BillAddr.Line1 + ' ' + Environment.NewLine;

        if (komplet.BillAddr.Line2 != null && komplet.BillAddr.Line2.Equals("")) adres += komplet.BillAddr.Line2 + ' ' + Environment.NewLine;
        if (!komplet.BillAddr.City.Equals("")) adres += komplet.BillAddr.City + ' ' + Environment.NewLine;
        if (!komplet.BillAddr.PostalCode.Equals("")) adres += komplet.BillAddr.PostalCode + ' ' + Environment.NewLine;
        if (!komplet.BillAddr.CountrySubDivisionCode.Equals("")) adres += komplet.BillAddr.CountrySubDivisionCode + ' ' + Environment.NewLine;
        adres += (await DatabaseAccessLayer.kantry())[komplet.BillAddr.CountryCode];

        var PrintLabel = new PrintConfirmViewModel(adres);
        PrintLabel.RequestClose += async (sender, e) =>
        {
            var result = ((PrintConfirmViewModel)sender).Result;
            if (result != null)
            {
                if (result)
                {
                    // PrintersService.PrintAddressLabel(Width, Height, PrinterName, 1, Image);
                    //      await Dymo.drukujAdres(adres);
                }
            }
        };
        await _dialogService.ShowDialog(PrintLabel);
    }

    public async Task ExecuteAddToQuickBooks()
    {
        Add2QBButtonText = "Adding to QuickBooks...";
        Add2QBProgressBarVisibility = true;
        var tak = await Add2QuickBooks(CompletesActionsObject.AllOrders[SelectedItem.id]);
        if (tak)
        {
            IsQuickBooked = true;
        }
        Add2QBButtonText = "Add to QuickBooks";
        Add2QBProgressBarVisibility = false;
    }

    private bool _orderCanBeEdited = true;

    public bool OrderCanBeEdited
    {
        get => _orderCanBeEdited;
        set => SetProperty(ref _orderCanBeEdited, value);
    }

    public async Task ExecuteRemoveFromQuickBooks()
    {
        Remove4QBButtonText = "Removing from QuickBooks...";

        OrderCanBeEdited = false;
        Remove4QBProgressBarVisibility = true;
        // wywala sie, gdy chce usuwac z qb - trzeba zdiagnozowac
        if (!CompletesActionsObject.invtxns.TryGetValue(SelectedItem.id, out var ktre1))
        {
            var whatToDo2 = await _dialogService.ShowYesNoMessageBox("Error",
                "Error: Invoice-Transaction relation can not be found. Shall I try again?");
            if (!whatToDo2)
            {
                return;
            }

            var response = await DatabaseAccessLayer.GetInvoiceTxns([SelectedItem.id]);
            if (!response.IsSuccess)
            {
                IsQuickBooked = false;
                Remove4QBButtonText = "Remove from QuickBooks";
                OrderCanBeEdited = true;
                Remove4QBProgressBarVisibility = false;
                await _dialogService.ShowMessage("Error", response.Message);
                return;
            }

            ktre1 = response.GetValue<List<invoicetxn>>().First();


            // var ktre1 = CompletesActionsObject.invtxns[SelectedItem.id];
        }

        int qbinvoiceID = Convert.ToInt32(ktre1.qbInvoiceId);

        var te = await Remove4QuickBooks(SelectedItem.id, qbinvoiceID);
        if (!te)
        {
            var whatToDo = await _dialogService.ShowYesNoMessageBox("Error in quickbooks", "Error: QuickBooks can not find associated invoice, therefore the order should be refreshed - shall I proceed?");
            if (whatToDo)
            {
                var invTXN = await DatabaseAccessLayer.FlipQBInvoiceIdinInvoiceTX(ktre1.invoiceTXNID);
                if (!invTXN.IsSuccess)
                {
                    IsQuickBooked = false;
                    Remove4QBButtonText = "Remove from QuickBooks";

                    OrderCanBeEdited = true;

                    Remove4QBProgressBarVisibility = false;
                    await _dialogService.ShowMessage("Error", invTXN.Message);
                    return;
                }
                CompletesActionsObject.invtxns[SelectedItem.id] = invTXN.GetValue<invoicetxn>();
                if (CompletesActionsObject.AllOrders.ContainsKey(SelectedItem.id))
                {
                    CompletesActionsObject.AllOrders[SelectedItem.id].Order.quickbooked = false;
                }
            }
        }
        IsQuickBooked = false;
        Remove4QBButtonText = "Remove from QuickBooks";

        OrderCanBeEdited = true;

        Remove4QBProgressBarVisibility = false;
    }

    public async Task ExecuteSaveAmazonDetails()
    {
        var komplecik = CompletesActionsObject.AllOrders[SelectedItem.id];
        var amazonEmailItem = AmCustomerDetails.FirstOrDefault(x => x.FieldName == "Amazon Email:" && x.HasChanged) as TextFieldViewModel;
        if (amazonEmailItem != null)
        {
            komplecik.Customer.Email = amazonEmailItem.SelectedValue;
        }

        var addressAsStringItem = AmCustomerDetails.FirstOrDefault(x => x.FieldName == "Address as a string:" && x.HasChanged) as TextFieldViewModel;
        if (addressAsStringItem != null)
        {
            komplecik.BillAddr.AddressAsAString = addressAsStringItem.SelectedValue;
        }
        await CompletesActionsObject.RefreshAmazonCustomerDetailsInComplete(komplecik);
        SelectedItem = await komplet2dvItem(komplecik);
    }

    public async Task ExecuteSaveRegAddress()
    {
        var komplecik = CompletesActionsObject.AllOrders[SelectedItem.id];
        var firstLineItem = RegCustomerBillAddr.FirstOrDefault(x => x.FieldName == "First Line:" && x.HasChanged) as TextFieldViewModel;
        if (firstLineItem != null)
        {
            komplecik.BillAddr.Line1 = firstLineItem.SelectedValue;
        }

        var secondLineItem = RegCustomerBillAddr.FirstOrDefault(x => x.FieldName == "Second Line:" && x.HasChanged) as TextFieldViewModel;
        if (secondLineItem != null)
        {
            komplecik.BillAddr.Line2 = secondLineItem.SelectedValue;
        }

        var cityItem = RegCustomerBillAddr.FirstOrDefault(x => x.FieldName == "City:" && x.HasChanged) as TextFieldViewModel;
        if (cityItem != null)
        {
            komplecik.BillAddr.City = cityItem.SelectedValue;
        }

        var postcodeItem = RegCustomerBillAddr.FirstOrDefault(x => x.FieldName == "Postcode:" && x.HasChanged) as TextFieldViewModel;
        if (postcodeItem != null)
        {
            komplecik.BillAddr.PostalCode = postcodeItem.SelectedValue;
        }

        var countyItem = RegCustomerBillAddr.FirstOrDefault(x => x.FieldName == "County:" && x.HasChanged) as TextFieldViewModel;
        if (countyItem != null)
        {
            komplecik.BillAddr.CountrySubDivisionCode = countyItem.SelectedValue;
        }
        await CompletesActionsObject.RefreshBillAddrInComplete(komplecik);
        SelectedItem = await komplet2dvItem(komplecik);
    }

    public async Task ExecuteSaveRegCustDetails()
    {
        var komplecik = CompletesActionsObject.AllOrders[SelectedItem.id];

        var custTitleItem = RegCustomerDetails.FirstOrDefault(x => x.FieldName == "CustTitle:" && x.HasChanged) as TextFieldViewModel;
        if (custTitleItem != null)
        {
            komplecik.Customer.Title = custTitleItem.SelectedValue;
        }

        var firstNameItem = RegCustomerDetails.FirstOrDefault(x => x.FieldName == "Firstname:" && x.HasChanged) as TextFieldViewModel;
        if (firstNameItem != null)
        {
            komplecik.Customer.GivenName = firstNameItem.SelectedValue;
        }

        var middleNameItem = RegCustomerDetails.FirstOrDefault(x => x.FieldName == "Middlename:" && x.HasChanged) as TextFieldViewModel;
        if (middleNameItem != null)
        {
            komplecik.Customer.MiddleName = middleNameItem.SelectedValue;
        }

        var surnameItem = RegCustomerDetails.FirstOrDefault(x => x.FieldName == "Surname:" && x.HasChanged) as TextFieldViewModel;
        if (surnameItem != null)
        {
            komplecik.Customer.FamilyName = surnameItem.SelectedValue;
        }

        var emailItem = RegCustomerDetails.FirstOrDefault(x => x.FieldName == "Email:" && x.HasChanged) as TextFieldViewModel;
        if (emailItem != null)
        {
            komplecik.Customer.Email = emailItem.SelectedValue;
        }

        var phoneItem = RegCustomerDetails.FirstOrDefault(x => x.FieldName == "Phone:" && x.HasChanged) as TextFieldViewModel;
        if (phoneItem != null)
        {
            komplecik.Customer.Phone = phoneItem.SelectedValue;
        }
        await CompletesActionsObject.RefreshCustomerInComplete(komplecik);
        SelectedItem = await komplet2dvItem(komplecik);
    }

    public static string Complete2String(Complete komplet, Dictionary<string, string> countries)
    {
        var zwrotka = new StringBuilder(komplet.Order.tracking).Append(" ");
        if (komplet.Customer.GivenName.Equals("Jeff") && komplet.Customer.FamilyName.Equals("Bezos"))
        {
            if (!string.IsNullOrEmpty(komplet.BillAddr.AddressAsAString))
            {
                zwrotka.Append(komplet.BillAddr.AddressAsAString + " ");
            }
        }
        else
        {
            zwrotka.Append(komplet.Customer.CompanyName + " ");
            zwrotka.Append(komplet.Customer.DisplayName + " ");
            zwrotka.Append(komplet.Customer.FamilyName + " ");
            zwrotka.Append(komplet.Customer.GivenName + " ");
            zwrotka.Append(komplet.Customer.MiddleName + " ");
            zwrotka.Append(komplet.Customer.Phone + " ");
            zwrotka.Append(komplet.BillAddr.City + " ");
            zwrotka.Append(komplet.BillAddr.CountryCode + " ");
            zwrotka.Append(countries[komplet.BillAddr.CountryCode] + " ");
            zwrotka.Append(komplet.BillAddr.Line1 + " ");
            zwrotka.Append(komplet.BillAddr.Line2 + " ");
            zwrotka.Append(komplet.BillAddr.PostalCode + " ");
        }
        zwrotka.Append(komplet.Order.orderID + " ");
        foreach (var itema in komplet.OrderItems)
        {
            zwrotka.Append(itema.itemName + " ");
        }
        return zwrotka.ToString();
    }

    public async void HandleCompleteChanged(List<int> changedData)
    {
        var noweKomplety = CompletesActionsObject.AllOrders.Where(p => changedData.Contains(p.Key)).Select(p => p.Value).ToList();//await _databaseAccessLayer.GetKomplety(changedData);
        foreach (var komplet in noweKomplety)
        {
            var ordek = Orders.FirstOrDefault(p => p.id == komplet.Order.orderID);
            if (ordek != null)
            {
                int i = Orders.IndexOf(ordek);
                Orders[i] = await komplet2dvItem(komplet);
                var viewItem = OrdersView.FirstOrDefault(p => p.id == komplet.Order.orderID);
                if (viewItem != null)
                {
                    int viewIndex = OrdersView.IndexOf(viewItem);
                    _dispatcherService.Invoke(() => OrdersView[viewIndex] = Orders[i]);
                }

                await _searchEngine.RefreshEntry(ordek.id, Complete2String(komplet, await DatabaseAccessLayer.kantry()));

                if (SelectedItem == null) { return; }
                if (SelectedItem.id == komplet.Order.orderID)
                {
                    SelectedItem = Orders[i];
                }
            }
            else
            {
                var nowy = await komplet2dvItem(komplet);
                Orders.Add(nowy);
                _dispatcherService.Invoke(() => OrdersView.Add(nowy));
                var kntry = await DatabaseAccessLayer.kantry();
                await _searchEngine.AddEntry(nowy.id, Complete2String(komplet, kntry));
            }
        }
    }

    public async Task ExecuteSaveGeneralOrderDetails()
    {
        var komplecik = CompletesActionsObject.AllOrders[SelectedItem.id];
        decimal OldPostagePrice = komplecik.Order.postagePrice;
        decimal newPostagePrice = 0;
        var postagePriceItem = OrderGeneralItems.FirstOrDefault(x => x.FieldName == "Postage:" && x.HasChanged) as TextFieldViewModel;
        if (postagePriceItem != null)
        {
            if (decimal.TryParse(postagePriceItem.SelectedValue, out decimal postagePrice))
            {
                var cenaTowaru = komplecik.Order.saletotal - komplecik.Order.postagePrice;
                komplecik.Order.postagePrice = postagePrice;
                komplecik.Order.saletotal = cenaTowaru + postagePrice;
                newPostagePrice = Convert.ToDecimal(Math.Round(postagePrice, 2));
            }
            else
            {
                await _dialogService.ShowMessage("Error", "Enter price in the correct format");
                return;
            }
        }
        var statusItem = OrderGeneralItems.FirstOrDefault(x => x.FieldName == "Statuses" && x.HasChanged) as ComboBoxStringStringViewModel;
        if (statusItem != null)
        {
            komplecik.Order.status = statusItem.SelectedValue.Key;
        }

        var paidOnItem = OrderGeneralItems.FirstOrDefault(x => x.FieldName == "Paid on:" && x.HasChanged) as DateFieldViewModel;
        if (paidOnItem != null)
        {
            komplecik.Order.paidOn = (DateTime)paidOnItem.SelectedValue;
        }

        var sentOnItem = OrderGeneralItems.FirstOrDefault(x => x.FieldName == "Sent on:" && x.HasChanged) as DateFieldViewModel;
        if (sentOnItem != null)
        {
            komplecik.Order.dispatchedOn = (DateTime)sentOnItem.SelectedValue;
        }

        var trackingItem = OrderGeneralItems.FirstOrDefault(x => x.FieldName == "Tracking:" && x.HasChanged) as TextFieldViewModel;
        if (trackingItem != null)
        {
            komplecik.Order.tracking = trackingItem.SelectedValue;
        }

        await CompletesActionsObject.RefreshOrderInComplete(komplecik);
        if (IsQuickBooked && SyncWithQb && OldPostagePrice != newPostagePrice)
        {
            var ktre1 = CompletesActionsObject.invtxns[SelectedItem.id];
            int tre = Convert.ToInt32(ktre1.qbInvoiceId);

            await _quickBooksService.UpdateShippingFee(tre, Convert.ToDecimal(newPostagePrice), emptyShot);
        }

        SelectedItem = await komplet2dvItem(komplecik);
    }

    public void Emptyshot(string we)
    {
    }

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set =>
            SetProperty(ref _isBusy, value);
    }

    private Dictionary<string, string> _statuses;

    public Dictionary<string, string> Statuses
    {
        get => _statuses;
        set =>
            SetProperty(ref _statuses, value);
    }

    private KeyValuePair<string, string> _selectedStatus;

    public KeyValuePair<string, string> SelectedStatus
    {
        get => _selectedStatus;
        set =>
            SetProperty(ref _selectedStatus, value);
    }

    private DateTime? _paidOn;

    public DateTime? PaidOn
    {
        get => _paidOn;
        set => SetProperty(ref _paidOn, value);
    }

    private DateTime? _sentOn;

    public DateTime? SentOn
    {
        get => _sentOn;
        set => SetProperty(ref _sentOn, value);
    }

    private bool _quickBooked;

    public bool IsQuickBooked
    {
        get => _quickBooked;
        set
        {
            if (SetProperty(ref _quickBooked, value))
            {
                OnPropertyChanged(nameof(IsNotQuickBooked));
            }
        }
    }

    public bool IsNotQuickBooked => !_quickBooked;

    public bool ItIsNotAmazonCustomer => !ItIsAmazonCustomer;

    private bool _itIsstAmazonCustomer;

    public bool ItIsAmazonCustomer
    {
        get => _itIsstAmazonCustomer;
        set => SetProperty(ref _itIsstAmazonCustomer, value);
    }

    public ObservableCollection<IBaseFieldViewModel> OrderGeneralItems { get; set; } = new ObservableCollection<IBaseFieldViewModel>();

    public bool AreAnyOrderGeneralItemsModified
    {
        get
        {
            return OrderGeneralItems.Any(item => item.HasChanged);
        }
    }

    public ObservableCollection<IBaseFieldViewModel> RegCustomerDetails { get; set; } = new ObservableCollection<IBaseFieldViewModel>();

    public bool AreAnyCustomerDetailsModified
    {
        get
        {
            if (RegCustomerDetails.Count == 0) return false;
            if ((!RegCustomerDetails.First(p => p.FieldName.Equals("CustTitle:")).HasChanged) &&
                (!RegCustomerDetails.First(p => p.FieldName.Equals("Firstname:")).HasChanged) &&
                (!RegCustomerDetails.First(p => p.FieldName.Equals("Middlename:")).HasChanged) &&
                (!RegCustomerDetails.First(p => p.FieldName.Equals("Surname:")).HasChanged))
                return RegCustomerDetails.Any(item => item.HasChanged);
            {
                var fn = RegCustomerDetails.First(p => p.FieldName.Equals("Full Name:")) as TextBlockFieldModel;
                var komplecik = CompletesActionsObject.AllOrders[SelectedItem.id];
                var builder = new StringBuilder();
                if (!string.IsNullOrEmpty(komplecik.Customer?.Title))
                {
                    builder.Append(komplecik.Customer.Title + " ");
                }
                if (!string.IsNullOrEmpty(komplecik.Customer?.GivenName))
                {
                    builder.Append(komplecik.Customer.GivenName + " ");
                }
                if (!string.IsNullOrEmpty(komplecik.Customer?.MiddleName))
                {
                    builder.Append(komplecik.Customer.MiddleName + " ");
                }
                if (!string.IsNullOrEmpty(komplecik.Customer?.FamilyName))
                {
                    builder.Append(komplecik.Customer.FamilyName + " ");
                }

                string fullname = builder.ToString().TrimEnd();
                fn.SelectedValue = fullname;
            }

            return RegCustomerDetails.Any(item => item.HasChanged);
        }
    }

    public ObservableCollection<IBaseFieldViewModel> RegCustomerBillAddr { get; set; } = [];

    public bool AreAnyRegCustomerBillAddrModified => RegCustomerBillAddr.Any(item => item.HasChanged);

    public ObservableCollection<IBaseFieldViewModel> AmCustomerDetails { get; set; } = new();

    public bool AreAnyAmazonCustomerDetailsModified => AmCustomerDetails.Any(item => item.HasChanged);

    public IBaseFieldViewModel CreateFieldViewModel(string fieldName, FieldType fieldType, string fieldIdentifier, object initialValue, string checker, object values = null)
    {
        var fieldViewModel = FieldViewModels.CreateFieldViewModel(fieldName, fieldType, fieldIdentifier, initialValue, values);
        if (fieldViewModel != null)
        {
            fieldViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanged")
                {
                    OnPropertyChanged(checker);
                }
            };
        }

        return fieldViewModel;
    }

    private string _lordNo;

    public string LordNo
    {
        get => _lordNo;
        set => SetProperty(ref _lordNo, value);
    }

    public ObservableCollection<ItemViewItem> ItemViewItems { get; set; } = new ObservableCollection<ItemViewItem>();

    private string _add2QBButtonText;

    public string Add2QBButtonText
    {
        get => _add2QBButtonText;
        set => SetProperty(ref _add2QBButtonText, value);
    }

    private bool _add2QBProgressBarVisibility;

    public bool Add2QBProgressBarVisibility
    {
        get => _add2QBProgressBarVisibility;
        set => SetProperty(ref _add2QBProgressBarVisibility, value);
    }

    private string _remove4QBButtonText;

    public string Remove4QBButtonText
    {
        get => _remove4QBButtonText;
        set => SetProperty(ref _remove4QBButtonText, value);
    }

    private bool _remove4QBProgressBarVisibility;

    public bool Remove4QBProgressBarVisibility
    {
        get => _remove4QBProgressBarVisibility;
        set => SetProperty(ref _remove4QBProgressBarVisibility, value);
    }

    public async Task<CompleteOrderViewItem> komplet2dvItem(Complete komp)
    {
        var ox = new CompleteOrderViewItem();
        var kuusd = komp.OrderItems.FirstOrDefault();
        ox.Item = kuusd == null ? "null" : komp.OrderItems.First().itemName;
        ox.PaidOn = komp.Order.paidOn;
        ox.Country = komp.BillAddr.CountryCode;
        ox.Status = (await DatabaseAccessLayer.statuses())[komp.Order.status];
        ox.id = komp.Order.orderID;
        ox.Buyer = komp.Customer.GivenName + " " + komp.Customer.FamilyName;
        return ox;
    }

    private Dictionary<int, string> Markets2PlatformNames;

    public async Task GetMarkets2PlatformNamesExecute()
    {
        Markets2PlatformNames = await DatabaseAccessLayer.GetMarkets2PlatformTypesDictionary();
    }

    private CompleteOrderViewItem _selectedItem;

    public CompleteOrderViewItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (value == null) { return; }

            SetProperty(ref _selectedItem, value);

            Add2QBButtonText = Resources.AddToQuickBooks;
            Add2QBProgressBarVisibility = false;
            Remove4QBButtonText = Resources.RemoveFromQuickBooks;
            Remove4QBProgressBarVisibility = false;

            var komplecik = CompletesActionsObject.AllOrders[SelectedItem.id];
            DisplayedOrderId = _selectedItem.id;
            DisplayedMarket = DatabaseAccessLayer.markety().Result[komplecik.Order.market].name;
            if (CompletesActionsObject.invtxns.ContainsKey(_selectedItem.id))
            {
                LordNo = CompletesActionsObject.invtxns[_selectedItem.id].platformTXN;
                if (CompletesActionsObject.invtxns[_selectedItem.id].qbInvoiceId == null)
                {
                    IsQuickBooked = false;
                    SyncWithQb = false;
                }
                else
                {
                    IsQuickBooked = true;
                    SyncWithQb = true;
                }
            }
            OrderGeneralItems.Clear();
            OnPropertyChanged(nameof(AreAnyOrderGeneralItemsModified));

            var values = DatabaseAccessLayer.statuses().Result.Select(pair => new StringString
            {
                Key = pair.Key,
                Value = pair.Value
            }).ToList();

            var initialValue = values.First(p => p.Key.Equals(komplecik.Order.status));

            OrderGeneralItems.Add(CreateFieldViewModel("Statuses", FieldType.ComboBox, "Statuses", initialValue, nameof(AreAnyOrderGeneralItemsModified), values));
            OrderGeneralItems.Add(CreateFieldViewModel("Paid on:", FieldType.DatePicker, "Paid on:", komplecik.Order.paidOn, nameof(AreAnyOrderGeneralItemsModified)));
            OrderGeneralItems.Add(CreateFieldViewModel("Sent on:", FieldType.DatePicker, "Sent on:", komplecik.Order.dispatchedOn, nameof(AreAnyOrderGeneralItemsModified)));
            OrderGeneralItems.Add(CreateFieldViewModel("Tracking:", FieldType.TextBox, "Tracking:", komplecik.Order.tracking, nameof(AreAnyOrderGeneralItemsModified)));
            OrderGeneralItems.Add(CreateFieldViewModel("Postage:", FieldType.TextBox, "Postage:", komplecik.Order.postagePrice.ToString(), nameof(AreAnyOrderGeneralItemsModified)));
            if (CompletesActionsObject.invtxns.ContainsKey(_selectedItem.id))
            {
                if (CompletesActionsObject.invtxns[_selectedItem.id].qbInvoiceId != null)
                {
                    IsQuickBooked = true;
                }
                else { IsQuickBooked = false; }
            }
            //  GetMarkets2PlatformNamesCommand.Execute(null);

            if (Markets2PlatformNames[komplecik.Order.market].ToLower().Contains("amazon"))
            {
                ItIsAmazonCustomer = true;
                AmCustomerDetails.Clear();
                OnPropertyChanged(nameof(AreAnyAmazonCustomerDetailsModified));
                AmCustomerDetails.Add(CreateFieldViewModel("Amazon Email:", FieldType.TextBox, "Amazon Email:", komplecik.Customer?.Email ?? string.Empty, nameof(AreAnyAmazonCustomerDetailsModified)));
                AmCustomerDetails.Add(CreateFieldViewModel("Address as a string:", FieldType.TextBox, "Address as a string:", komplecik.BillAddr.AddressAsAString ?? string.Empty, nameof(AreAnyAmazonCustomerDetailsModified)));
            }
            else
            {
                ItIsAmazonCustomer = false;
                RegCustomerDetails.Clear();
                OnPropertyChanged(nameof(AreAnyCustomerDetailsModified));
                var builder = new StringBuilder();
                if (!string.IsNullOrEmpty(komplecik.Customer?.Title))
                {
                    builder.Append(komplecik.Customer.Title + " ");
                }
                if (!string.IsNullOrEmpty(komplecik.Customer?.GivenName))
                {
                    builder.Append(komplecik.Customer.GivenName + " ");
                }
                if (!string.IsNullOrEmpty(komplecik.Customer?.MiddleName))
                {
                    builder.Append(komplecik.Customer.MiddleName + " ");
                }
                if (!string.IsNullOrEmpty(komplecik.Customer?.FamilyName))
                {
                    builder.Append(komplecik.Customer.FamilyName + " ");
                }

                string fullname = builder.ToString().TrimEnd();
                RegCustomerDetails.Add(CreateFieldViewModel("Customer Title:", FieldType.TextBox, "Customer Title:", komplecik.Customer?.Title ?? string.Empty, nameof(AreAnyCustomerDetailsModified)));
                RegCustomerDetails.Add(CreateFieldViewModel("Firstname:", FieldType.TextBox, "Firstname:", komplecik.Customer?.GivenName ?? string.Empty, nameof(AreAnyCustomerDetailsModified)));
                RegCustomerDetails.Add(CreateFieldViewModel("Middlename:", FieldType.TextBox, "Middlename:", komplecik.Customer?.MiddleName ?? string.Empty, nameof(AreAnyCustomerDetailsModified)));
                RegCustomerDetails.Add(CreateFieldViewModel("Surname:", FieldType.TextBox, "Surname:", komplecik.Customer?.FamilyName ?? string.Empty, nameof(AreAnyCustomerDetailsModified)));
                RegCustomerDetails.Add(CreateFieldViewModel("Full Name:", FieldType.TextBlock, "Full Name:", fullname, nameof(AreAnyCustomerDetailsModified)));

                RegCustomerDetails.Add(CreateFieldViewModel("Email:", FieldType.TextBox, "Email:", komplecik.Customer?.Email ?? string.Empty, nameof(AreAnyCustomerDetailsModified)));
                RegCustomerDetails.Add(CreateFieldViewModel("Phone:", FieldType.TextBox, "Phone:", komplecik.Customer?.Phone ?? string.Empty, nameof(AreAnyCustomerDetailsModified)));

                RegCustomerBillAddr.Clear();
                OnPropertyChanged(nameof(AreAnyRegCustomerBillAddrModified));
                RegCustomerBillAddr.Add(CreateFieldViewModel("First Line:", FieldType.TextBox, "First Line:", komplecik.BillAddr?.Line1 ?? string.Empty, nameof(AreAnyRegCustomerBillAddrModified)));
                RegCustomerBillAddr.Add(CreateFieldViewModel("Second Line:", FieldType.TextBox, "Second Line:", komplecik.BillAddr?.Line2 ?? string.Empty, nameof(AreAnyRegCustomerBillAddrModified)));
                RegCustomerBillAddr.Add(CreateFieldViewModel("City:", FieldType.TextBox, "City:", komplecik.BillAddr?.City ?? string.Empty, nameof(AreAnyRegCustomerBillAddrModified)));
                RegCustomerBillAddr.Add(CreateFieldViewModel("Postcode:", FieldType.TextBox, "Postcode:", komplecik.BillAddr?.PostalCode ?? string.Empty, nameof(AreAnyRegCustomerBillAddrModified)));
                RegCustomerBillAddr.Add(CreateFieldViewModel("County:", FieldType.TextBox, "County:", komplecik.BillAddr?.CountrySubDivisionCode ?? string.Empty, nameof(AreAnyRegCustomerBillAddrModified)));
            }

            string waluta = currencies[komplecik.Order.salecurrency].symbol;

            ItemViewItems.Clear();
            foreach (var ite in komplecik.OrderItems)
            {
                var predm = new ItemViewItem
                {
                    Name = ite.itemName,
                    Quantity = ite.quantity.ToString(),
                    Mpn = DatabaseAccessLayer.items[ite.itembodyID].itembody.mpn,
                    Price = waluta + Math.Round(Convert.ToDouble(ite.price), 2),
                    itemBodyid = ite.itembodyID
                };
                ItemViewItems.Add(predm);
            }
        }
    }

    private void ExecuteRowClick(CompleteOrderViewItem item)
    {
        SelectedItem = item;
    }

    public event Action<int> ProductSelected;

    public void ExecuteProductRowClick(ItemViewItem product)
    {
        ProductSelected?.Invoke(product.itemBodyid);
    }

    public ShookayWrapper<StringInt> _searchEngine;

    private bool _isDataLoaded;

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set => SetProperty(ref _isDataLoaded, value);
    }

    private Dictionary<string, currency> currencies;

    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;

        IsBusy = true;

        var listaIds = await CompletesActionsObject.GetAllInvoicesAndOrderIds(12);
        var kantrries = await DatabaseAccessLayer.kantry();
        currencies = await DatabaseAccessLayer.Currencies();
        //Dictionary<int, HashSet<string>> allOrdersO = new();
        List<StringInt> allOrdersO = new();

        await foreach (var completeObject in DatabaseAccessLayer.GetKompletyAsyncStream(listaIds))
        {
            int i = 0;
            foreach (var item2 in completeObject.OrderItems)
            {
                completeObject.OrderItems[i].itemName += " " + DatabaseAccessLayer.items[completeObject.OrderItems[i].itembodyID].itembody.mpn;
                i++;
            }
            CompletesActionsObject.AllOrders.Add(completeObject.Order.orderID, completeObject);
            Orders.Add((await komplet2dvItem(completeObject)));
            allOrdersO.Add(new StringInt(Complete2String(completeObject, kantrries), completeObject.Order.orderID));
            //        allOrdersO.Add(completeObject.Order.orderID, await SearchEngine.Complete2Words(completeObject, kantrries));
        }
        _searchEngine = new ShookayWrapper<StringInt>(allOrdersO);
        string json = JsonConvert.SerializeObject(allOrdersO);


        await _searchEngine.PrepareEntries();
        //searchEngine32=new SearchEngine32Bit(allOrdersO);

        await GetMarkets2PlatformNamesExecute();

        if (PendingOrderId.HasValue)
        {
            ShowOrder(PendingOrderId.Value);
            PendingOrderId = null;
        }
        await OnSearchChanged();
        IsBusy = false;
        IsDataLoaded = true;
        OrdersIn = true;
    }

    public async void ShowOrder(int id)
    {
        //todo : co zrobic, kiedt nie ma zamowienia
        var ordek = Orders.FirstOrDefault(p => p.id == id);
        if (ordek != null)
        {
            SelectedItem = Orders.First(p => p.id == id);
        }
        else
        {
            var komplet = await DatabaseAccessLayer.GetKomplet(id);
            await CompletesActionsObject.AddInvTXN(id);
            var kvi = await komplet2dvItem(komplet);
            Orders.Add(kvi);
            OrdersView.Add(kvi);
        }
    }

    private async Task OnSearchChanged()
    {
        _cancellationTokenSource.Cancel(); // Cancel any ongoing search
        _cancellationTokenSource = new CancellationTokenSource(); // Create new cancellation token source
        var token = _cancellationTokenSource.Token;
        var searchInput = SearchInput?.ToLower();  // use ToLower for string comparison
        try
        {
            await Task.Delay(200, token);  // Wait for a short delay to debounce
        }
        catch (TaskCanceledException)
        {
            return; // If task is cancelled, stop executing method.
        }

        if (token.IsCancellationRequested)
        {
            return;
        }
        if (string.IsNullOrEmpty(searchInput))
        {
            foreach (var result in Orders)
            {
                OrdersView.Add(result);
            }
        }
        else
        {
            _dispatcherService.Invoke(() => OrdersView.Clear());

            var lista = await _searchEngine.FindWithin(searchInput);
            _dispatcherService.Invoke(() =>
                {
                    foreach (var result in lista)
                    {
                        var order = Orders.FirstOrDefault(item => item.id == result);
                        if (order != null)
                        {
                            OrdersView.Add(order);
                        }
                    }
                }

            );


        }
            
    }
}