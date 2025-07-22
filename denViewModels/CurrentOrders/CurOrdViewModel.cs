using AmazonSPAPIClient;
using ColoursOperations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Models;
using denModels;
using denSharedLibrary;
using denViewModels.CurrentOrders;
using EmailService;
using Microsoft.Extensions.Options;
using Printers;
using Serilog;
using SettingsKeptInFile;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text;


namespace denViewModels;

public partial class CurOrdViewModel 
{

     

    public void GetSettings()
    {
        var themeResponse = SettingsService.GetSetting("terminal_theme");
        if (themeResponse.IsSuccess)
        {
            string theme = themeResponse.GetValue<string>();
            TerminalScreenViewModelProperty.SetTheme(theme);
        }
        else
        {
            string theme = ThemesActions.MYThemes.First().Name;
            SettingsService.UpdateSetting("terminal_theme", theme);
            TerminalScreenViewModelProperty.SetTheme(theme);
        }

        var largeLabelPrinterResponse = SettingsService.GetSetting("4x6Labelpriner");
        if (largeLabelPrinterResponse.IsSuccess)
        {
            IsLargeLabelPrinterEnabled = true;
        }

        var selectedSortingMethodResponse = SettingsService.GetSetting("SelectedOrderSorthingMethod");
        if (selectedSortingMethodResponse.IsSuccess)
        {
            int selectedSortingMethod =Convert.ToInt32(selectedSortingMethodResponse.GetValue<string>());
            SelectedOrderSorthingMethod = orderSortingValues.First(p => p.Id == selectedSortingMethod);
        }
        else
        {
            SelectedOrderSorthingMethod = orderSortingValues.First();
        }
    }

    private IEmailService EmailService;
    private readonly ISettingsService SettingsService;
    private IPrintersService PrintersService;
    private IColoursService ColoursService;
    private readonly EmailSettings _emailSettings;

    public CurOrdViewModel(IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer,
        IFileDialogService fileDialogService, ICompletesToXpsStream completesToXpsStream, IXpsPrinter xpsPrinter,
        IOrdersSummaryToXpsStream ordersSummaryToXpsStream, IColourOpsMediator colourOpsMediator,
        ITerminalScreenViewModel terminalScreenViewModel,ICurrentKomplety currentKomplety,IMarketActions marketActions,IEmailService emailService,ISettingsService settingsService,
        IPrintersService printersService,IColoursService coloursService, IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
        PrintersService = printersService;
        SettingsService = settingsService;
        OrderSortingItems = new(orderSortingValues);
        EmailService = emailService;
        _marketActions = marketActions;
        _colourOpsMediator = colourOpsMediator;
        _fileDialogService = fileDialogService;
        _dialogService = dialogService;
        _databaseAccessLayer = databaseAccessLayer;
        _completesToXpsStream = completesToXpsStream;
        _xpsPrinter = xpsPrinter;
        _ordersSummaryToXpsStream = ordersSummaryToXpsStream;
        locationID = SettingsService.LocationId;
        _currentKomplety = currentKomplety;
        _currentKomplety.SetLocation(locationID);
        TerminalScreenViewModelProperty = terminalScreenViewModel;
        Konsola = new MyTerminal(TerminalScreenViewModelProperty);
        FetchOrdersCommand = new AsyncRelayCommand(ExecuteFetchOrders);
        Orders = new();
        NextThemeCommand = new RelayCommand(NextTheme);
        AOBCommand = new AsyncRelayCommand(ExecuteAOB);
        CNDCommand = new AsyncRelayCommand(ExecuteCND);
        PrintInvoicesCommand = new AsyncRelayCommand(ExecutePrintInvoices);
        PrintCN22sCommand = new AsyncRelayCommand(ExecutePrintCn22S);
        SelectAllNewCommand = new RelayCommand(ExecuteSelectAllNew);
        SelectAllEbayCommand = new RelayCommand(ExecuteSelectAllEbay);
        SelectAllAmazonCommand = new RelayCommand(ExecuteSelectAllAmazon);
        FlipAllCommand = new RelayCommand(ExecuteFlipAll);
        FlipNewCommand = new RelayCommand(ExecuteFlipNew);
        ChangeStatusesCommand = new AsyncRelayCommand(ExecuteChangeStatuses);
        PrintSummaryCommand = new AsyncRelayCommand(ExecutePrintSummary);
        ProductClickCommand = new RelayCommand<ProductLine>(ProductClickedExecute);
        RowDoubleClickCommand = new RelayCommand<DataViewItem>(ExecuteRowDoubleClick);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        HideDetailsCommand = new RelayCommand(DeselectOrder);
        KeyValuePair<string, List<string>> druczki = PrintersService.GetPrinters("default_printer");
        Printers = new ObservableCollection<string>(druczki.Value);

        if (Printers.Count > 0)
        {
            string defaultPrinter = druczki.Key;

            SelectedPrinter = defaultPrinter;
            IsComboBoxEnabled = true;
        }
        else
        {
            IsComboBoxEnabled = false;
        }
        Orders.CollectionChanged += OnOrdersCollectionChanged;
        CompeletesChangedCommand = new AsyncRelayCommand<CompletesChangedEventArgs>(CompletesChangedExecute);
        _currentKomplety.CompletesChanged += CompletesChangedHandler;
        FetchOrdersButtonText = denLanguageResourses.Resources.FetchOrdersButtonText; 
        FetchOrdersBarVisibility = false;
        PrintInvoicesButtonText = denLanguageResourses.Resources.PrintInvoicesButtonText;
        PrintInvoicesBarVisibility = false;
        ChangeStatusButtonText = denLanguageResourses.Resources.ChangeStatusButtonText;
        ChangeStatusBarVisibility = false;
        GetSettings();
        SettingsService.SettingsMessenger.Subscribe("4x6Labelpriner", HandleLargeLabelPrinterChanged);
        ColoursService = coloursService;
    }

     
     
    public bool SelectAll
    {
        get=>  _selectAll; 
        set
        {
            if (SetProperty(ref _selectAll, value))
            {


                _isUpdatingAll = true;
                foreach (var item in Orders)
                {
                    item.PrintMe = _selectAll;
                }

                _isUpdatingAll = false;
                DisplaySelectedItemsCount();
            }
        }
    }

     

    
    public void CheckIfPrintIsOk()
    {
        if (!string.IsNullOrWhiteSpace(SelectedPrinter))
        {
            CanPrint = PrintMeDependentButtonsOn;
        }
    }

      

    public async Task<KeyValuePair<OperationStatus, List<int>>> ParseAmazonPackingSlipsFromCLipboard(IClipboardService clipboardService)
    {
        var returnValue = new KeyValuePair<OperationStatus, List<int>>();
        Konsola.Napisz(denLanguageResourses.Resources.ProcessingAmazonOrdersMessage);

        if (!clipboardService.TryGettingText(out var tekst))
        {
            Konsola.NapiszLinie(denLanguageResourses.Resources.NoTextInClipboardMessage);
            return new KeyValuePair<OperationStatus, List<int>>(OperationStatus.NoTextInClipboard, null);
        }
           
        var orderAdres = new Dictionary<string, string>();
        var linijki = tekst.Split('\r', '\n').ToList();
        while (linijki.Count > 0)
        {
            if (!ContainsAnyPair(linijki, AmazonSpApi.AmazonPairs))
            {
                Konsola.NapiszLinie(denLanguageResourses.Resources.IncorrectFormatMessage);
                return new KeyValuePair<OperationStatus, List<int>>(OperationStatus.InvalidClipboardFormat, null);
            }
            var wyw = _marketActions.AmaOrdExtractor(linijki);
            orderAdres.Add(wyw.orderid, wyw.adres);
            linijki = wyw.reszta;
        }
        var ku = new Dictionary<int, string>();
        bool niewczytane = false;
        var invy = await _databaseAccessLayer.PlatformTXN2InvoiceTXN(orderAdres.Keys.ToList());
        foreach (var xx in orderAdres)
        {

            if (!invy.TryGetValue(xx.Key, out var hoi))
            {
                niewczytane = true;
            }
            else
            {
                ku.Add(hoi.orderID, xx.Value);
            }
        }

        switch (niewczytane)
        {
            case true when invy.Count == 0:
                Konsola.NapiszLinie(denLanguageResourses.Resources.OrderNotDownloadedMessage);
                return new KeyValuePair<OperationStatus, List<int>>(OperationStatus.OrderNotProcessed, null);
            case true:
                return new KeyValuePair<OperationStatus, List<int>>(OperationStatus.OrderNotProcessed, null);
        }

        var orderki = invy.Values.Select(p => p.orderID).ToList();

        if (_currentKomplety.GetKomplety() == null)
        {
            Konsola.NapiszLinie(denLanguageResourses.Resources.NoOrdersDownloadedMessage);
            return new KeyValuePair<OperationStatus, List<int>>(OperationStatus.NoOrdersDownloadedForProcessing, null);
        }
        Dictionary<int, int> adresikiOrderki = _currentKomplety.GetKomplety().Where(p => orderki.Contains(p.Order.orderID)).ToDictionary(p => p.Order.orderID, q => q.BillAddr.billaddrID);//(p=>p.BillAddr).Select(p=>p.billaddrID).ToList();

        var rezultat = await _databaseAccessLayer.UpdateAddressesAsStrings(adresikiOrderki, ku);
        if (rezultat == 2)
        {
            Konsola.NapiszLinie(denLanguageResourses.Resources.NoDatabaseConnectionMessage);
            return new KeyValuePair<OperationStatus, List<int>>(OperationStatus.NoDatabaseConnection, null);
        }
        var listka = new List<Complete>();
        foreach (var li in adresikiOrderki)
        {
            _currentKomplety.GetKomplety().First(p => p.Order.orderID == li.Key).BillAddr.AddressAsAString = ku[li.Key];
            listka.Add(_currentKomplety.GetKomplety().First(p => p.Order.orderID == li.Key));
        }

        Konsola.NapiszLinieKolorowo(string.Format(denLanguageResourses.Resources.OrdersProcessedMessage, orderki.Count));
        return new KeyValuePair<OperationStatus, List<int>>(OperationStatus.Success, orderki.Cast<int>().ToList());
    }

    public static bool ContainsAnyPair(List<string> we, Dictionary<string, string> amazonPairs)
    {
        foreach (var pair in amazonPairs)
        {
            int keyIndex = -1;
            int valueIndex = -1;

            for (int i = 0; i < we.Count; i++)
            {
                if (we[i].Contains(pair.Key) && keyIndex == -1)
                {
                    keyIndex = i;
                }

                if (we[i].Contains(pair.Value) && valueIndex == -1)
                {
                    valueIndex = i;
                }

                if (keyIndex != -1 && valueIndex != -1)
                {
                    break;
                }
            }

            if (keyIndex != -1 && valueIndex > keyIndex)
            {
                return true;
            }
        }

        return false;
    }


    public async Task CAD_Click(Stream outputStream, List<int> listka)
    {
        var builder = new StringBuilder("\"Full Name or Company name\",\"Shipping address - Address line 1\",\"Shipping address - Address line 2\",\"Shipping address - City\",\"County\",\"Shipping address - Postcode\",\"Country\",\"IOSS Number\"");
        builder.AppendLine();

        Konsola.Napisz(denLanguageResourses.Resources.PreparingDataForCnDMessagePart1);
        Konsola.Napisz(listka.Count.ToString(), 1);
        Konsola.Napisz(denLanguageResourses.Resources.PreparingDataForCnDMessagePart2);

        Complete[] ttta = _currentKomplety.CompletesDict.Where(p => listka.Contains(p.Key)).Select(q => q.Value).ToArray();
        Dictionary<string, string> kantry = (await _databaseAccessLayer.kantry());
          
        foreach (var tta in ttta)
        {
            if ((tta.Order.market == 1) || (tta.Order.market == 9))
            {
                var city = tta.BillAddr.City;
                if (tta.BillAddr.CountryCode.Equals("SG") && string.IsNullOrEmpty(city))
                {
                    city = "Singapore";
                }
                if (tta.BillAddr.CountryCode.Equals("HK") && string.IsNullOrEmpty(city))
                {
                    city = "Hong Kong";
                }
                builder.Append('"').Append(tta.Customer.GivenName ?? " ").Append(' ').Append(tta.Customer.FamilyName ?? " ").Append("\",")
                    .Append('"').Append(tta.BillAddr.Line1 ?? " ").Append("\",")
                    .Append('"').Append(tta.BillAddr.Line2 ?? " ").Append("\",")
                    .Append('"').Append(city ?? " ").Append("\",")
                    .Append('"').Append(tta.BillAddr.CountrySubDivisionCode ?? " ").Append("\",")
                    .Append('"').Append(tta.BillAddr.PostalCode ?? " ").Append("\",")
                    .Append('"').Append(kantry[tta.BillAddr.CountryCode]).Append("\",");
                if (_euCountries.Contains(tta.BillAddr.CountryCode))
                {
                    builder.Append("\"IM2760000742\"");
                }
                else
                {
                    builder.Append("\"\"");
                }
                builder.AppendLine();
            }
        }

        await using (var writer = new StreamWriter(outputStream))
        {
            await writer.WriteAsync(builder);
        }

        Konsola.NapiszLinie(denLanguageResourses.Resources.DoneMessage, 3);

    }

    public async Task ChangeStatuses(List<orderStatusUpdateModel> listain)
    {
        var amazonTrackingNos = new List<DataNededToMarkAmazonOrderAsShipped>();
        var ebayTrackingNos = new Dictionary<string, string>();
        var ebayoweOrdy = new List<string>();
        var lista = listain.Select(item => new orderStatusUpdateModel
        {
            Id = item.Id,
            status = item.status,
            tracking = item.tracking.Replace(" ", "")
        }).ToList();
        List<Complete> zamowienia = _currentKomplety.GetKomplety().Where(o => lista.Select(p => p.Id).Contains(o.Order.orderID)).ToList();
        Dictionary<int, string> fakturyTransakcje = (await _databaseAccessLayer.GetInvoiceTxns(zamowienia.Select(p => p.Order.orderID).ToList())).GetValue<List<invoicetxn>>().ToDictionary(p => p.orderID, q => q.platformTXN);
        Dictionary<int, int> listaCustomerId = zamowienia.Where(p => p.Order.market == 9).ToDictionary(p => p.Order.orderID, q => q.Order.customerID);
        Dictionary<int, string> CustomerIdsEmails = zamowienia.Where(p => listaCustomerId.Values.Contains(p.Order.customerID)).ToDictionary(p => p.Order.customerID, q => q.Customer.Email);
        Dictionary<int, (string Status, string Tracking)> orderUpdates = lista.ToDictionary(p => p.Id, p => (Status: p.status, Tracking: p.tracking));
        List<order> ords = await _databaseAccessLayer.UpdateorderStatusesAndTrackings(orderUpdates);
         
        var ChangedCompletes = new List<Complete>();
        foreach (Complete komplet in _currentKomplety.GetKomplety())
        {
            order zamowienie = ords.FirstOrDefault(o => o.orderID == komplet.Order.orderID);

            if (zamowienie != null)
            {
                komplet.Order = zamowienie;
                ChangedCompletes.Add(komplet);
            }
        }
        if (ChangedCompletes.Count > 0)
        {
            await _currentKomplety.OrdersNeedRefreshing(ChangedCompletes);
        }
        int inde = 1;

        Dictionary<int, string> m2p = await _databaseAccessLayer.GetMarkets2PlatformTypesDictionary();

        List<amazonmarketplace> mrkplcs = await _databaseAccessLayer.AmazonMarketplaces();
        foreach (orderStatusUpdateModel or in lista)
        {
            try
            {
                order ord = ords.FirstOrDefault(p => p.orderID == or.Id);
                if (ord == null)
                {
                    Log.Warning("Zamówienie o ID {OrderId} nie zostało znalezione.", or.Id);
                    continue;
                }

                if (or.status.Equals("SHIP"))
                {
                    if (m2p[ord.market].ToLower().Contains("amazon"))
                    {
                        DataNededToMarkAmazonOrderAsShipped el = new DataNededToMarkAmazonOrderAsShipped
                        {
                            Orderid = ord.orderID,
                            AmazonOrderId = fakturyTransakcje[ord.orderID],
                            Tracking = or.tracking,
                            MarketPlaceId = mrkplcs.First(p => p.marketID == ord.market).code
                        };
                        amazonTrackingNos.Add(el);
                        Log.Debug("Dodano do Amazon Tracking: {Details}", el);
                    }
                    else if (m2p[ord.market].ToLower().Contains("ebay"))
                    {
                        string trata = fakturyTransakcje[ord.orderID];
                        if (!string.IsNullOrEmpty(or.tracking))
                        {
                            ebayTrackingNos.Add(trata, or.tracking.ToUpper());
                        } else
                        {
                            ebayTrackingNos.Add(trata, "");
                        }
                        ebayoweOrdy.Add(trata);
                        Log.Debug("Dodano do eBay Tracking: OrderID = {OrderId}, Tracking = {Tracking}", ord.orderID, or.tracking);
                    }
                    else if (ord.market == 9) // Specyficzna logika dla marketu o ID 9
                    {
                        string tekst = "Your order has been processed and was dispatched. We're hoping that it will be with you shortly.\n\n";
                        if (!string.IsNullOrEmpty(ord.tracking))
                        {
                            tekst += $"The tracking number is {ord.tracking} and we have used Royal Mail to ship it. Please do check with Royal Mail for updates";
                        }
                        var answer=await EmailService.SendEmail(_emailSettings.Username, _emailSettings.Username, _emailSettings.Password, CustomerIdsEmails[ord.customerID], "Purchase confirmation from Time4Parts", tekst);
                        Konsola.NapiszLinie($"{answer.Message} : {(answer.IsSuccess ? "OK" : "Error")}");

                        Log.Debug("Wysłano email dla CustomerID = {CustomerId} z potwierdzeniem wysyłki.", ord.customerID);
                    }
                }
                else
                {
                    Log.Debug("Zamówienie o ID {OrderId} nie jest oznaczone jako SHIP.", or.Id);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Wystąpił wyjątek podczas przetwarzania zamówienia o ID {OrderId}. Wiadomość: {Message}", or.Id, ex.Message);
            }
            inde++; // Uwaga: Zastanów się nad znaczeniem i użyciem tej zmiennej w kontekście logowania.
        }

        // Podsumowanie po przetworzeniu wszystkich zamówień
        Log.Information("Przetworzono wszystkie zamówienia. Liczba przetworzonych elementów: {Count}", lista.Count);




        try
        {
            List<Task> tasks = new List<Task>();
            if (amazonTrackingNos.Count > 0)
            {
                tasks.Add(_marketActions.MarkAmazonOrdersAsShipped(amazonTrackingNos, Konsola.NapiszKolorowo, locationID));
                Log.Debug("Zadanie MarkAmazonOrdersAsShipped dodane. Liczba Amazon Tracking Numbers: {Count}", amazonTrackingNos.Count);
            }
            if (ebayoweOrdy.Count > 0)
            {
                tasks.Add(Task.Run(async () =>
                {
                    //  var result = await _marketActions.MarkEbayOrdersAsDispatched2(ebayTrackingNos,Konsola.NapiszLinieKolorowo, locationID);
                  
                    var feedbackResult = await _marketActions.LeaveFeedbackOnEbay(ebayoweOrdy.ToArray(), Konsola.NapiszLinieKolorowo, locationID);
                    await _marketActions.MarkEbayOrdersAsDispatched(feedbackResult, ebayTrackingNos, Konsola.NapiszLinieKolorowo, locationID);
                    Log.Debug("Zadanie LeaveFeedbackOnEbay i MarkEbayOrdersAsDispatched wykonane. Liczba eBay Orders: {Count}", ebayoweOrdy.Count);
                }));
            }

            await Task.WhenAll(tasks);
            Log.Debug("Wszystkie zadania zostały zakończone. Liczba zadań: {TasksCount}", tasks.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Wystąpił wyjątek podczas wykonywania zadań w ChangeStatuses. Wiadomość: {Message}", ex.Message);
        }
        finally
        {
            // Tutaj możesz zalogować stan końcowy zmiennych, jeśli jest to potrzebne
            Log.Debug("Koniec metody ChangeStatuses. Stan zmiennych: AmazonTrackingNosCount: {AmazonCount}, EbayTrackingNosCount: {EbayCount}, EbayoweOrdyCount: {EbayoweCount}", amazonTrackingNos.Count, ebayTrackingNos.Count, ebayoweOrdy.Count);
        }

    }


       

    public async Task CompletesChangedExecute( CompletesChangedEventArgs e)
    {

        foreach (int id in e.ChangedCompletes)
        {
            var komplet = _currentKomplety.CompletesDict[id];
            bool ret = _currentKomplety.retCusts[komplet.Customer.customerID] > 1;
            var updatedItem = await Komplet2dataviewItem(komplet, ret);

             

            //  _dispatcherService.Invoke(() =>
            //  {
            var ord = Orders.FirstOrDefault(p => p.orderid == id);
            if (ord == null)
            {
                return;
            }
            var index = Orders.IndexOf(ord);
            if (index != -1)
            {
                Orders[index] = updatedItem;

            }
            //  });
        }
    }

    private void CompletesChangedHandler(object sender, CompletesChangedEventArgs e)
    {
        // Wywołaj asynchroniczną komendę
        CompeletesChangedCommand.ExecuteAsync(e);
    }



    public async Task ExecuteAOB()
    {
        IsAOBRunning = true;
        IsBusy = true;
        var rezultat = await ParseAmazonPackingSlipsFromCLipboard(_clipboardService);
        IsBusy = false;
        IsAOBRunning = false;
        switch (rezultat.Key)
        {
            case OperationStatus.NoTextInClipboard:
                await _dialogService.ShowMessage(
                    denLanguageResourses.Resources.NoTextInClipboardMessage,
                    denLanguageResourses.Resources.NoTextInClipboardTitle);
                break;

            case OperationStatus.InvalidClipboardFormat:
                await _dialogService.ShowMessage(
                    denLanguageResourses.Resources.IncorrectFormatMessage,
                    denLanguageResourses.Resources.InvalidClipboardFormatTitle);
                break;

            case OperationStatus.NoDatabaseConnection:
                await _dialogService.ShowMessage(
                    denLanguageResourses.Resources.NoDatabaseConnectionMessage,
                    denLanguageResourses.Resources.NoDatabaseConnectionTitle);
                break;

            case OperationStatus.OrderNotProcessed:
                await _dialogService.ShowMessage(
                    denLanguageResourses.Resources.OrderNotDownloadedMessage,
                    denLanguageResourses.Resources.OrderNotProcessedTitle);
                break;

            case OperationStatus.NoOrdersDownloadedForProcessing:
                await _dialogService.ShowMessage(
                    denLanguageResourses.Resources.NoOrdersDownloadedMessage,
                    denLanguageResourses.Resources.NoOrdersDownloadedTitle);
                break;
            case OperationStatus.PartialOrderNotProcessed:
                await _dialogService.ShowMessage(denLanguageResourses.Resources.PartialOrderNotProcessedMessage, denLanguageResourses.Resources.PartialOrderNotProcessedTitle);
                break;
        }
        if (rezultat.Key == OperationStatus.Success)
        {
            var komplety = _currentKomplety.GetKomplety().Where(p => rezultat.Value.Contains(p.Order.orderID)).ToList();

            foreach (var komplet in komplety)
            {
                var orderToUpdate = Orders.FirstOrDefault(o => o.orderid == komplet.Order.orderID);
                if (orderToUpdate != null)
                {
                    bool ret = _currentKomplety.retCusts[komplet.Customer.customerID] > 1; 
                    
                    var updatedItem = await Komplet2dataviewItem(komplet, ret);

                    // zaktualizuj element w kolekcji
                    var index = Orders.IndexOf(orderToUpdate);
                    if (index != -1)
                    {
                        Orders[index] = updatedItem;
                    }
                }
            }
        }
    }

    public async Task ExecuteChangeStatuses()
    {
        ChangeStatusButtonText = denLanguageResourses.Resources.ChangingStatuses;
        ChangeStatusBarVisibility = true;
        ChangeStatusButtonVisibility = false;
        var listOfOrders = new List<StatusChangeViewItem>();
        foreach (var item in Orders.Where(p => p.PrintMe))
        {
            StringBuilder itemy = new();
            for (int i = 0; i < item.Items.Length; i++)
            {
                string? xo = item.Items[i];
                itemy.Append(xo);
                if (i < item.Items.Length - 1)
                {
                    itemy.AppendLine();
                }
            }
            var gu = new StatusChangeViewItem
            {
                orderId = item.orderid,
                Buyer = item.Buyer,
                Country = item.Country,
                Market = item.Market,
                PostalCode = item.PostCode,
                Product = itemy.ToString(),
                Status = item.Status,
                Tracking = item.Tracking
            };
            listOfOrders.Add(gu);
        }
        var wybory = new Dictionary<string, string>
        {
            { "NONE", denLanguageResourses.Resources.SelectStatus }
        };
        var wyby = await _databaseAccessLayer.statuses();
        foreach (var mar in wyby)
        {
            wybory.Add(mar.Key, mar.Value);
        }
        var changeStatusViewModel = new ChangeStatusViewModel(listOfOrders, wybory, _dialogService);
        changeStatusViewModel.RequestClose += async (sender, e) =>
        { 
            var result = ((ChangeStatusViewModel)sender).Result;
            if (result != null)
            {
                await ChangeStatuses(result);

                await RefreshItems(result.Select(p => p.Id).ToList());
            }
            ChangeStatusButtonText = denLanguageResourses.Resources.ChangeStatusButtonText;
            ChangeStatusBarVisibility = false;
            ChangeStatusButtonVisibility = true;
        };
        await _dialogService.ShowDialog(changeStatusViewModel);
        ChangeStatusButtonText = denLanguageResourses.Resources.ChangeStatusButtonText;
        ChangeStatusBarVisibility = false;
        ChangeStatusButtonVisibility = true;
    }

    public async Task ExecuteCND()
    {
        var stream = await _fileDialogService.SaveFileAsync("Save Click and Drop data", "CSV files | *.csv");
        if (stream != null)
        {
            var listka = Orders.Where(p => p.PrintMe).Select(q => q.orderid).ToList();
            await CAD_Click(stream, listka);
            stream.Close();
        }
    }


    public void ExecuteFlipAll()
    {
        _isUpdatingAll = true;
        foreach (var order in Orders)
        {
            order.PrintMe = !order.PrintMe;
        }
        _isUpdatingAll = false;
        DisplaySelectedItemsCount();
    }

    public void ExecuteFlipNew()
    {
        _isUpdatingAll = true;
        foreach (var order in Orders)
        {
            if (order.Status.Equals("New Order"))
            {
                order.PrintMe = !order.PrintMe;
            }
        }
        _isUpdatingAll = false;
        DisplaySelectedItemsCount();
    }
    public async Task ExecutePrintCn22S()
    {
        var listka = Orders.Where(p => p.PrintMe).Select(p => p.orderid).ToList();
        var komplety = _currentKomplety.GetKomplety().Where(p => listka.Contains(p.Order.orderID)).ToList();
        Konsola.Napisz(denLanguageResourses.Resources.Printing + " ");
        Konsola.Napisz(listka.Count.ToString(), 1);
        Konsola.NapiszLinie(" " + denLanguageResourses.Resources.CN22Forms);
        var largePrinterResponse = SettingsService.GetSetting("4x6Labelpriner");
        if (largePrinterResponse.IsSuccess)
        {
            PrintersService.PrintCN22s(komplety, largePrinterResponse.GetValue<string>());
        }
           
    }

    public async Task ExecutePrintInvoices()
    {

        bool isConfirmed = await _dialogService.ShowYesNoMessageBox(
            denLanguageResourses.Resources.ConfirmPrinter,
            string.Format(denLanguageResourses.Resources.PrintingOn, SelectedPrinter));
        if (!isConfirmed)
        {
                
            PrintInvoicesButtonText = denLanguageResourses.Resources.PrintInvoicesButtonText; 
            PrintInvoicesBarVisibility = false;
            return;
        }

        if (SelectedPrinter.Contains("dymo", StringComparison.CurrentCultureIgnoreCase))
        {
            bool isLabelPrinterConfirmed = await _dialogService.ShowYesNoMessageBox(
                denLanguageResourses.Resources.ConfirmPrinter,
                denLanguageResourses.Resources.LabelPrinterConfirmation);
            if (!isLabelPrinterConfirmed)
            {
                PrintInvoicesButtonText = denLanguageResourses.Resources.PrintInvoicesButtonText;
                PrintInvoicesBarVisibility = false;
                return;
            }
        }

        PrintInvoicesButtonText = denLanguageResourses.Resources.Printing;
        PrintInvoicesBarVisibility = false;
        var komplety = new List<int>();
        bool sabezosy = false;
        foreach (var item in Orders.Where(p => p.PrintMe))
        {
            int Id = item.orderid;
            komplety.Add(Id);
            if (item.Buyer.Equals("Jeff Bezos") && !sabezosy)
            {
                bool isJeffBezosConfirmed = await _dialogService.ShowYesNoMessageBox(
                    denLanguageResourses.Resources.JeffBezosDetected,
                    denLanguageResourses.Resources.JeffBezosConfirmation);
                if (!isJeffBezosConfirmed)
                {
                    PrintInvoicesButtonText = denLanguageResourses.Resources.PrintInvoicesButtonText;
                    PrintInvoicesBarVisibility = false;
                    return;
                }
                sabezosy = true;
            }
        }
        if (komplety.Count == 0)
        {
            return;
        }

        var kompleciki = _currentKomplety.GetKomplety().Where(p => komplety.Contains(p.Order.orderID));

        InvoicePrintoutDataPack invoicePrintoutDataPack;
        invoicePrintoutDataPack = new InvoicePrintoutDataPack
        {
            postageTypes = await _databaseAccessLayer.PostageTypes(),

            currencies = new Dictionary<int, currency>(),
            VatRates=[]
        };
        var currencies=await _databaseAccessLayer.Currencies();
        var vatrates = await _databaseAccessLayer.VatRates();
        foreach (var ko in kompleciki)
        {
            invoicePrintoutDataPack.currencies.Add(ko.Order.orderID, currencies[ko.Order.salecurrency]);
            invoicePrintoutDataPack.VatRates.Add(ko.Order.orderID, vatrates[ko.Order.VATRateID]);
        }
        invoicePrintoutDataPack.items = _databaseAccessLayer.items;
        invoicePrintoutDataPack.kantry = await _databaseAccessLayer.kantry();
        invoicePrintoutDataPack.Brands = await _databaseAccessLayer.Brands();
        invoicePrintoutDataPack.cechyValues = await _databaseAccessLayer.cechyValues();
        invoicePrintoutDataPack.markety = (await _databaseAccessLayer.markety()).ToDictionary(p=>p.Key,q=>q.Value.name);
        invoicePrintoutDataPack.types = await _databaseAccessLayer.types();
        invoicePrintoutDataPack.kolorId = _databaseAccessLayer.colourProperty;

        MemoryStream sztrim = await _completesToXpsStream.GenerateStream(kompleciki.ToList(), invoicePrintoutDataPack);
        _xpsPrinter.SetPrinter(SelectedPrinter);
        sztrim.Seek(0, SeekOrigin.Begin);
        Konsola.NapiszLinieKolorowo(string.Format(denLanguageResourses.Resources.PrintingInvoicesCount, kompleciki.Count()));
        _xpsPrinter.Print(sztrim);
        PrintInvoicesButtonText = denLanguageResourses.Resources.PrintInvoicesButtonText;
        PrintInvoicesBarVisibility = false;
    }

    public async Task ExecutePrintSummary()
    {
        Konsola.Napisz(string.Format(denLanguageResourses.Resources.PrintingSummaryOfOrders, Orders.Where(p => p.PrintMe).Count()));
        var ordki = Orders.Where(p => p.PrintMe).Select(p => p.orderid).ToList();
        var komplety = _currentKomplety.GetKomplety().Where(p => ordki.Contains(p.Order.orderID));
        Dictionary<int, multidrawer> MultiDrawers = null;
        if (SelectedOrderSorthingMethod.Id == 0)
        {
            komplety = komplety.OrderBy(p => p.Order.paidOn);
        } else
        {
            MultiDrawers = (await _databaseAccessLayer.multidrawer()).ToDictionary(p => p.MultiDrawerID, q => q);
            var mapa = new Dictionary<int, string>();
            List<Complete> kompletes = new();
            foreach (var item in komplety)
            {
                mapa[item.Order.orderID] = GetBodyInTheBoxForId(item.OrderItems.First().itembodyID);
                   
            }
            var posortowanePoStringu = mapa.OrderBy(kvp => kvp.Value);
            foreach (var item in posortowanePoStringu)
            {
                kompletes.Add(komplety.First(p => p.Order.orderID == item.Key));
            }
            komplety = kompletes;

        }
        SummaryPrintoutDataPack spdp = await GetSummaryPrintoutDataPack(ordki);
        MemoryStream sztrim = await _ordersSummaryToXpsStream.GenerateStream(komplety.ToList(), spdp);
        _xpsPrinter.SetPrinter(SelectedPrinter);
        sztrim.Seek(0, SeekOrigin.Begin);
        _xpsPrinter.Print(sztrim);
        Konsola.NapiszLinie(denLanguageResourses.Resources.DoneMessage, 3);

        string GetBodyInTheBoxForId(int itemBodyId)
        {
            string output = "";

            var jest = _databaseAccessLayer.items[itemBodyId].bodyinthebox;
            if (jest != null)
            {
                var sza = MultiDrawers[jest.MultiDrawerID].name;
                output = sza + '[' + (char)(65 + jest.column) + ',' + (jest.row + 1) + ']';
            }
            return output;
        }
    }

    public void ExecuteRowDoubleClick(DataViewItem dataViewItem)
    {
        if (dataViewItem == null)
        {
            return;
        }
        OrderSelected?.Invoke(dataViewItem.orderid);
    }

    public void ExecuteSelectAllAmazon()
    {
        _isUpdatingAll = true;
        foreach (var order in Orders)
        {
            if (order.Market.Contains("amazon", StringComparison.CurrentCultureIgnoreCase))
            {
                order.PrintMe = true;
            }
            else
            {
                order.PrintMe = false;
            }
        }
        DisplaySelectedItemsCount();
    }

    public void ExecuteSelectAllEbay()
    {
        _isUpdatingAll = true;
        foreach (var order in Orders)
        {
            if (order.Market.Contains("ebay", StringComparison.CurrentCultureIgnoreCase))
            {
                order.PrintMe = true;
            }
            else
            {
                order.PrintMe = false;
            }
        }
        _isUpdatingAll = false;
        DisplaySelectedItemsCount();
    }

    public void ExecuteSelectAllNew()
    {
        _isUpdatingAll = true;
        foreach (var order in Orders)
        {
            if (order.Status == "New Order")
            {
                order.PrintMe = true;
            }
            else
            {
                order.PrintMe = false;
            }
        }
        _isUpdatingAll = false;
        DisplaySelectedItemsCount();
    }

    public async Task get_databaseAccessLayerTask()
    {
         
        await _databaseAccessLayer.GetPackage(locationID);
    }

    public async Task<SummaryPrintoutDataPack> GetSummaryPrintoutDataPack(List<int> listka)
    {
        var zwrotka = new SummaryPrintoutDataPack
        {
            items = _databaseAccessLayer.items,
            MultiDrawer = (await _databaseAccessLayer.multidrawer()),
            OrderIdsPlatformTXNs = (await _databaseAccessLayer.GetInvoiceTxns(listka)).GetValue<List<invoicetxn>>().ToDictionary(p => p.orderID, q => q.platformTXN),
            Markety = (await _databaseAccessLayer.markety()).ToDictionary(p=>p.Key,q=>q.Value.name),
            SoldWiths=[],
            OrderItemQuantitiesSold = new Dictionary<int, Dictionary<int, int>>()
        };

        foreach (var id in listka)
        {
            var itemQuantitiesSold = new Dictionary<int, int>();
            foreach (var item in _currentKomplety.CompletesDict[id].OrderItems)
            {
                if (_currentKomplety.CompletesDict[id].Order.market == 9)
                {
                    if (itemQuantitiesSold.ContainsKey(item.itembodyID))
                    {
                        itemQuantitiesSold[item.itembodyID] += item.quantity;
                    }
                    else
                    {
                        itemQuantitiesSold.Add(item.itembodyID, item.quantity);
                    }                  
                }
                else
                {
                    var ima = _databaseAccessLayer.items[item.itembodyID].ItmMarketAssocs.FirstOrDefault(p => p.itmmarketassID == item.itmMarketAssID);
                    if (ima != null)
                    {
                        if (itemQuantitiesSold.ContainsKey(item.itembodyID))
                        {
                            itemQuantitiesSold[item.itembodyID] += ima.quantitySold * item.quantity;
                        }
                        else
                        {
                            itemQuantitiesSold.Add(item.itembodyID, ima.quantitySold * item.quantity);
                        }                            
                        if (ima.soldWith != null)
                        {                              
                            if (!zwrotka.SoldWiths.TryGetValue(id, out Dictionary<int, int>? value))
                            {
                                value = [];
                                zwrotka.SoldWiths.Add(id, value);
                            }
                            value.Add(ima.itembodyID, (int)ima.soldWith);
                        }
                    }                    
                }
            }
            zwrotka.OrderItemQuantitiesSold.Add(id, itemQuantitiesSold);
        }
        return zwrotka;
    }

    private async Task<DataViewItem> Komplet2dataviewItem(Complete komplet, bool returningCustomer, zonki? zoneczki = null)
    {
         
        var itema = new DataViewItem
        {
            Arrived = komplet.Order.paidOn,
            Status = (await _databaseAccessLayer.statuses())[komplet.Order.status],
            PrintMe = false,
            Market = (await _databaseAccessLayer.markety())[komplet.Order.market].name,
            PostCode = komplet.BillAddr.PostalCode,
            Tracking = komplet.Order.tracking
        };
        if (komplet.BillAddr.AddressAsAString != null)
        {
            var xox = komplet.BillAddr.AddressAsAString.Split(['\r', '\n']).FirstOrDefault();
            if (!string.IsNullOrEmpty(xox))
            {
                itema.Buyer = xox;
            }
            else
            {
                itema.Buyer = komplet.Customer.GivenName + ' ' + komplet.Customer.FamilyName;
            }
        }
        else
        {
            itema.Buyer = komplet.Customer.GivenName + ' ' + komplet.Customer.FamilyName;
        }
        itema.Returning = returningCustomer;

        int j = 1;
        var nazwyprzedmiotow = new List<string>();
        var quantities = new List<string>();

        var linijki = new List<ProductLine>();
        foreach (var prod in komplet.OrderItems)
        {
            var linijka = new ProductLine();
            var bo = _databaseAccessLayer.items[prod.itembodyID].itembody;
            var kodKoloru = _databaseAccessLayer.items[bo.itembodyID].ItmCechies.FirstOrDefault(p => p.parameterID == _databaseAccessLayer.colourProperty);
            if (kodKoloru != null)
            {
                var kol = (await _databaseAccessLayer.cechyValues())[kodKoloru.parameterID].FirstOrDefault(p => p.parameterValueID == kodKoloru.parameterValueID);
                if (kol != null)
                {
                    if (kolorki.ContainsKey(kol.parameterValueID))
                    {

                        linijka.pic = kolorki[kol.parameterValueID];
                        linijka.colourId = kol.parameterValueID;
                    }
                        
                }
            }
            if (!bo.mpn.Equals("Tool"))
            {
                linijka.itembodyid = bo.itembodyID;
                linijka.name = bo.myname + ' ' + bo.mpn;
                linijka.qua = prod.quantity;
                nazwyprzedmiotow.Add(bo.myname + ' ' + bo.mpn);
                quantities.Add(prod.quantity.ToString());
            }
            else
            {
                itema.Tool = denLanguageResourses.Resources.Yes;
            }
            j++;
            if (!bo.mpn.Equals("Tool"))
            {
                linijki.Add(linijka);
            }
        }
        itema.Items = nazwyprzedmiotow.ToArray();
        itema.Items2 = linijki.ToArray();
        itema.Quantities = quantities.ToArray();


        itema.Country = (await _databaseAccessLayer.kantry())[komplet.BillAddr.CountryCode];
        
        if (komplet.BillAddr.CountryCode.Equals("GB") &&
            (
                ScottishPostCodes.Any(code => komplet.BillAddr.PostalCode.ToLower().StartsWith(code, StringComparison.OrdinalIgnoreCase))
            )
           )
        {
            itema.Flag = flagi["AB"];
            itema.Country = "Scotland";
        }
        else
        {

            itema.Flag = flagi[komplet.BillAddr.CountryCode];
        }



        var ce = (await _databaseAccessLayer.Currencies())[komplet.Order.salecurrency].symbol;
            
        itema.Postage = (double) komplet.Order.postagePrice;
           
        var meh = (await _databaseAccessLayer.krajStrefa())[komplet.BillAddr.CountryCode];
        itema.Zone = (await _databaseAccessLayer.strefyRM())[meh];
        if (zoneczki != null)
        {
            switch (meh)
            {
                case 1:
                    zoneczki.zoneUK++;
                    break;

                case 2:
                    zoneczki.zoneEU++;
                    break;

                case 5:
                    zoneczki.zone1++;
                    break;

                case 3:
                    zoneczki.zone2++;
                    break;

                case 4:
                    zoneczki.zone3++;
                    break;
            }
        }
        itema.curSymbol = ce;
        itema.Price = Math.Round(Convert.ToDecimal(komplet.Order.saletotal), 2);
        itema.orderid = komplet.Order.orderID;
        return itema;
    }

     

    public void NextTheme()
    {
        Konsola.GetNextTheme();
        BorderColor = Konsola.GetBorder();
        SettingsService.UpdateSetting("terminal_theme", Konsola.GetTheme());
    }

    public async void OneColourUpdated(int id)
    {
        var kolor = await ColoursService.GetOneColour((await _databaseAccessLayer.ColourTranslations())[id]);
        kolorki[id] = (byte[])kolor.Value;
        foreach (var ord in Orders)
        {
            foreach (var prod in ord.Items2)
            {
                if (prod.colourId == id)
                {
                    prod.pic = kolorki[id];
                }
            }
        }
    }

    public async Task PopulateInvoicesVm()
    {
        var invy = _currentKomplety.GetKomplety().Select(p => p.Order.orderID);
        if (!invy.Any())
        {
            return;
        }
        var indeksy = Orders.Where(p => !invy.Contains(p.orderid)).ToList();
        foreach (var gox in indeksy)
        {
            Orders.Remove(gox);
        }
        Konsola.Napisz(denLanguageResourses.Resources.GettingOrderDetails);

        var itemy2 = _currentKomplety.GetKomplety().Select(p => p.OrderItems).ToList();
        List<orderitem> itemy3 = new();
        foreach (var itm in itemy2)
        {
            itemy3.AddRange(itm);
        }
        List<itembody> bodki = new();
        foreach (var itm in itemy3)
        {
            bodki.Add(_databaseAccessLayer.items[itm.itembodyID].itembody);
        }
        Dictionary<int, DataViewItem> itemyMap = Orders.ToDictionary(i => i.orderid);
        var komplety = _currentKomplety.GetKomplety();
        int i = 0;
        foreach (var komplet in komplety)
        {
            bool ret = _currentKomplety.retCusts[komplet.Customer.customerID] > 1;
            if (!_currentKomplety.GetKomplety()[i].Order.status.Equals("SHIP"))
            {
                if (komplet.Order.locationID == SettingsService.LocationId)
                {
                    DataViewItem itema;
                    if (itemyMap.ContainsKey(komplet.Order.orderID))
                    {
                        var existingItem = Orders.First(x => x.orderid == komplet.Order.orderID);
                        int index = Orders.IndexOf(existingItem);

                        if (index != -1)
                        {
                            Orders[index] = itemyMap[komplet.Order.orderID];
                        }
                    }
                    else
                    {
                        itema = await Komplet2dataviewItem(_currentKomplety.CompletesDict[komplet.Order.orderID], ret);
                        Orders.Add(itema);
                    }
                }
            }
            i++;
        }
        Konsola.NapiszLinie(denLanguageResourses.Resources.DoneMessage, 3);
    }


    public async Task PrintInvoice(Complete komplet)
    {
        var kompleciki = new List<Complete> { komplet };

        InvoicePrintoutDataPack invoicePrintoutDataPack = new InvoicePrintoutDataPack
        {
            postageTypes = await _databaseAccessLayer.PostageTypes(),
        
            currencies = new Dictionary<int, currency>(),
            VatRates = new Dictionary<int, decimal>()
        };
        invoicePrintoutDataPack.currencies.Add(komplet.Order.orderID,(await _databaseAccessLayer.Currencies())[komplet.Order.salecurrency]);
        invoicePrintoutDataPack.VatRates.Add(komplet.Order.orderID,(await _databaseAccessLayer.VatRates())[komplet.Order.VATRateID]);

         
        invoicePrintoutDataPack.items = _databaseAccessLayer.items;
        invoicePrintoutDataPack.kantry = await _databaseAccessLayer.kantry();
        invoicePrintoutDataPack.Brands = await _databaseAccessLayer.Brands();
        invoicePrintoutDataPack.cechyValues = await _databaseAccessLayer.cechyValues();
        invoicePrintoutDataPack.markety =( await _databaseAccessLayer.markety()).ToDictionary(p=>p.Key,q=>q.Value.name);
        invoicePrintoutDataPack.types = await _databaseAccessLayer.types();
        invoicePrintoutDataPack.kolorId = _databaseAccessLayer.colourProperty;

        MemoryStream sztrim = await _completesToXpsStream.GenerateStream(kompleciki, invoicePrintoutDataPack);
        _xpsPrinter.SetPrinter(SelectedPrinter);
        sztrim.Seek(0, SeekOrigin.Begin);
        _xpsPrinter.Print(sztrim);
    }

    public async Task RefreshItems(List<int> orderIds)
    {
        var komplety = _currentKomplety.GetKomplety().Where(p => orderIds.Contains(p.Order.orderID)).ToList();

        foreach (var komplet in komplety)
        {
            var existingItem = Orders.First(x => x.orderid == komplet.Order.orderID);
            int index = Orders.IndexOf(existingItem);
            bool ret = _currentKomplety.retCusts[komplet.Customer.customerID] > 1;
            if (index != -1)
            {
                Orders[index] = await Komplet2dataviewItem(komplet, ret);
            }
        }
    }

    private void DisplaySelectedItemsCount()
    {
        int count = Orders.Count(x => x.PrintMe);
        switch (count)
        {
            case 0:
                Konsola.NapiszLinie(denLanguageResourses.Resources.AllOrdersUnselected);
                break;
            case 1:
                Konsola.NapiszLinie(string.Format(denLanguageResourses.Resources.OrderSelected, "1"), 1);
                break;
            default:
                Konsola.NapiszLinie(string.Format(denLanguageResourses.Resources.OrdersSelected, count.ToString()), 1);
                break;
        }
    }

 

    private async Task ExecuteFetchOrders()
    {
          
        IsBusy = true;
        FetchOrdersButtonText = denLanguageResourses.Resources.FetchingOrders; 
        FetchOrdersBarVisibility = true;
        SelectedOrder = null;
        Dictionary<int, string> stareOrdy = new();

        if (_currentKomplety.GetKomplety() != null && _currentKomplety.GetKomplety().Length > 0)
        {
            stareOrdy = _currentKomplety.GetKomplety().ToDictionary(p => p.Order.orderID, q => q.Order.status);
        }
        int number = await _currentKomplety.GetNumberOfOrders2Download();
        if (number == 0)
        {
            Konsola.NapiszLinie(denLanguageResourses.Resources.NoNewOrders);
        }
        else
        {
            Konsola.NapiszLinie(string.Format(denLanguageResourses.Resources.FetchingOnlyOrders, number));
            await _currentKomplety.FetchKomplety(stareOrdy);

            if (_currentKomplety.GetKomplety() != null && _currentKomplety.GetKomplety().Length == 0)
            {
                FetchOrdersButtonText = denLanguageResourses.Resources.FetchOrders;
                FetchOrdersBarVisibility = false;
                IsFetchOrdersProcessing = false;
                IsBusy = false;
                SomeOrdersAreIn = false;
                return;
            }
        }
        var getDailyStatsTask = GetDailyStats();
        await PopulateInvoicesVm();
              
        var ds = await getDailyStatsTask;
        SoldToday = "2Day: "+currencySymbol+" " + Math.Round(ds.kwotaDzis.TotalInMainCurrency, 2) + " (" + ds.kwotaDzis.NumberOfOrdersInMainCurrency + ") / " +
                    " * " + Math.Round(ds.kwotaDzis.TotalInOtherCurrencies, 2) + " (" + ds.kwotaDzis.NumberOfOrdersInOtherCurrencies + ") / " +
                    "Total  "+currencySymbol+"" + Math.Round(ds.kwotaDzis.FullTotalInMainCurrency, 2) + " (" + ds.kwotaDzis.calaIlosc + ")";

        SoldYday = "YDay: "+currencySymbol+" "+ Math.Round(ds.kwotaWczoaj.TotalInMainCurrency, 2) + " (" + ds.kwotaWczoaj.NumberOfOrdersInMainCurrency +
                   ") / " +
                   " * " + Math.Round(ds.kwotaWczoaj.TotalInOtherCurrencies, 2) + " (" + ds.kwotaWczoaj.NumberOfOrdersInOtherCurrencies + ") / " +
                   "Total "+currencySymbol+"" + Math.Round(ds.kwotaWczoaj.FullTotalInMainCurrency, 2) + " (" + ds.kwotaWczoaj.calaIlosc + ")";

        SoldMonth = "Month: "+ currencySymbol+" " + Math.Round(ds.tenMiesiac.TotalInMainCurrency, 2) + " (" + ds.tenMiesiac.NumberOfOrdersInMainCurrency +
                    ") / " +
                    " * " + Math.Round(ds.tenMiesiac.TotalInOtherCurrencies, 2) + " (" + ds.tenMiesiac.NumberOfOrdersInOtherCurrencies + ") / " +
                    "Total "+currencySymbol+"" + Math.Round(ds.tenMiesiac.FullTotalInMainCurrency, 2) + " (" + ds.tenMiesiac.calaIlosc + ")";
            
        IsFetchOrdersProcessing = false;
        IsBusy = false;
        FetchOrdersButtonText = denLanguageResourses.Resources.FetchOrders;
        FetchOrdersBarVisibility = false;
        SomeOrdersAreIn = true;
    }


    public async Task<DailyStats> GetDailyStats()
    {
        dnioSprzedaz kwotaDzis = new();
        dnioSprzedaz kwotaWczoraj = new();
        dnioSprzedaz tenMiesiac = new();
        DateTime today = DateTime.Today;

           

        var currentAndPreviousDayOrders = await _databaseAccessLayer.GetCurrentAndPreviousDaysKomplety();
          

         
        //wczoraj




        foreach (var komplet in currentAndPreviousDayOrders.Where(p => p.Order.paidOn.Day == today.AddDays(-1).Day))
        {
            if (!komplet.Order.salecurrency.Equals(currencyName))
            {
                foreach (var itemek in komplet.OrderItems)
                {
                    kwotaWczoraj.TotalInOtherCurrencies += itemek.price * itemek.quantity;
                    kwotaWczoraj.NumberOfOrdersInOtherCurrencies += itemek.quantity;
                    kwotaWczoraj.calaIlosc += itemek.quantity;
                    kwotaWczoraj.FullTotalInMainCurrency +=( itemek.price * itemek.quantity) * komplet.Order.xchgrate;
                }
            }
            else
            {
                foreach (var itemek in komplet.OrderItems)
                {
                    kwotaWczoraj.TotalInMainCurrency += itemek.price * itemek.quantity;
                    kwotaWczoraj.NumberOfOrdersInMainCurrency += itemek.quantity;
                    kwotaWczoraj.calaIlosc += itemek.quantity;
                    kwotaWczoraj.FullTotalInMainCurrency += itemek.price * itemek.quantity;
                }
            }
        }

        //dzis

        foreach (var komplet in currentAndPreviousDayOrders.Where(p => p.Order.paidOn.Day == today.Day))
        {
            if (!komplet.Order.salecurrency.Equals(currencyName))
            {
                   
                foreach (var itemek in komplet.OrderItems)
                {
                    kwotaDzis.TotalInOtherCurrencies += itemek.price * itemek.quantity;
                    kwotaDzis.NumberOfOrdersInOtherCurrencies += itemek.quantity;
                    kwotaDzis.calaIlosc += itemek.quantity;
                    kwotaDzis.FullTotalInMainCurrency += (itemek.price * itemek.quantity) * komplet.Order.xchgrate;
                }
            }
            else  
            {
                foreach (var itemek in komplet.OrderItems)
                {
                    kwotaDzis.TotalInMainCurrency += itemek.price * itemek.quantity;
                    kwotaDzis.NumberOfOrdersInMainCurrency += itemek.quantity;
                    kwotaDzis.calaIlosc += itemek.quantity;
                    kwotaDzis.FullTotalInMainCurrency += itemek.price * itemek.quantity;
                }
            }
        }

        //wczoraj

        foreach (var komplet in currentAndPreviousDayOrders.Where(p => p.Order.paidOn.Month == today.Month))
        {
            if (!komplet.Order.salecurrency.Equals(currencyName))
            {
                   
                foreach (var itemek in komplet.OrderItems)
                {
                    tenMiesiac.TotalInOtherCurrencies += itemek.price * itemek.quantity;
                    tenMiesiac.NumberOfOrdersInOtherCurrencies += itemek.quantity;
                    tenMiesiac.calaIlosc += itemek.quantity;
                    tenMiesiac.FullTotalInMainCurrency += (itemek.price * itemek.quantity) * komplet.Order.xchgrate;
                }
            }
            else
            {
                foreach (var itemek in komplet.OrderItems)
                {
                    tenMiesiac.TotalInMainCurrency += itemek.price * itemek.quantity;
                    tenMiesiac.NumberOfOrdersInMainCurrency += itemek.quantity;
                    tenMiesiac.calaIlosc += itemek.quantity;
                    tenMiesiac.FullTotalInMainCurrency += (itemek.price * itemek.quantity);
                }
            }
        }

        return new DailyStats
        {
            kwotaDzis = kwotaDzis,
            kwotaWczoaj = kwotaWczoraj,
            tenMiesiac = tenMiesiac
        };
    }


       

    private void Item_PrintMeChanged(object sender, EventArgs e)
    {
        if (!_isUpdatingAll)
        {
            DisplaySelectedItemsCount();
        }
    }

    [ObservableProperty] private string _aGif;

  

    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        IsFetchOrdersProcessing = true;
        FetchOrdersBarVisibility = true;
        FetchOrdersButtonText = denLanguageResourses.Resources.BootingUp; 
        Konsola.Napisz(denLanguageResourses.Resources.BootingUp, 0); Konsola.Napisz(".", 0);
        await get_databaseAccessLayerTask();
     
        kolorki = [];

        ColoursService.SetMediator(_colourOpsMediator);
        foreach (var kolek in (await _databaseAccessLayer.ColourTranslations()))
        {
            var val = await ColoursService.GetOneColour(kolek.Value);
            kolorki.Add(val.Key, (byte[])val.Value);
        }

        ColoursService.ColourChanged += OneColourUpdated;

            
        Konsola.Napisz(".", 0);
        string path2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\flags\");
        await Task.Run(() =>
        {
            DirectoryInfo d = new DirectoryInfo(path2);
            FileInfo[] Files = d.GetFiles("*.png");
            flagi = new();

            foreach (var f in Files)
            {
                byte[] imageData;
                using (var stream = new FileStream(f.FullName, FileMode.Open))
                {
                    imageData = new byte[stream.Length];
                    stream.Read(imageData, 0, (int)stream.Length);
                }
                flagi.Add(f.Name.Substring(0, 2), imageData);
            }
        });
        Konsola.Napisz(".", 0);

        var currencyNameResponse = SettingsService.GetSetting("currency");
        if (currencyNameResponse.IsSuccess)
        {
            currencyName= currencyNameResponse.GetValue<string>();
        } else 
        {
            currencyName = "GBP";
            SettingsService.UpdateSetting("currency", "GBP");
        }

        var currencies = await _databaseAccessLayer.Currencies();

        currencySymbol = currencies[currencyName].symbol;


        FetchOrdersBarVisibility = false;
        FetchOrdersButtonText = denLanguageResourses.Resources.FetchOrders;
        Konsola.NapiszLinie(denLanguageResourses.Resources.DoneMessage, 3);
        IsFetchOrdersProcessing = false;



        IsDataLoaded = true;
    }

    private void OnOrderPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DataViewItem.PrintMe))
        {
            OnPropertyChanged(nameof(PrintMeDependentButtonsOn));
            CheckIfPrintIsOk();
        }
    }

    private void OnOrdersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        for (int i = 0; i < Orders.Count; i++)
        {
            Orders[i].Index = i + 1; // Indeksowanie od 1
        }
        // Jeśli do kolekcji dodano nowe obiekty, subskrybuj ich zdarzenia PropertyChanged
        if (e.NewItems != null)
        {
            foreach (DataViewItem newItem in e.NewItems)
            {
                newItem.PropertyChanged += OnOrderPropertyChanged;
                newItem.PrintMeChanged += Item_PrintMeChanged;
            }
        }

        // Jeśli z kolekcji usunięto obiekty, anuluj subskrypcję ich zdarzeń PropertyChanged
        if (e.OldItems != null)
        {
            foreach (DataViewItem oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= OnOrderPropertyChanged;
                oldItem.PrintMeChanged -= Item_PrintMeChanged;
            }
        }

        OnPropertyChanged(nameof(PrintMeDependentButtonsOn));
    }

    private void ProductClickedExecute(ProductLine item)
    {
        ProductSelected?.Invoke(item.itembodyid);
    }

}