using CasioUKDeliveriesMVVM.Model;
using DataServicesNET80;
using denSharedLibrary;
using denViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.Models;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace CasioUKDeliveriesMVVM;

public class CasioUKDeliveriesViewModel :ObservableObject, INotifyPropertyChanged
{
    public static List<casioinvoice> doPokazania;
    public static int invoiceindexer;
    public static decimal totalO;
    private readonly IDialogService _dialogService;
    private string _addMpnTXT;

    private string _boHigh;
    private string _curOrHigh;
    private string _invoiceText;

    private bool _isDataLoaded = false;
    private bool _isInvoiceProcessing;
    private List<casioukbackorder> _originalBakOrdy = new ();
    private List<casioukcurrentorder> _originalCurOrdy = new ();
    private Visibility _pokazujInvoiceProgressBar;
    private Visibility _pokazujProgressBar;
    private string _searchMpnText;
    private int LocationID;
        
    Dictionary<string, decimal> Prices = new Dictionary<string, decimal>();

    public IDatabaseAccessLayer _databaseAccessLayer;
    private IDispatcherService _dispatcherService;
    private IMarketActions _marketActions;
    public CasioUKDeliveriesViewModel(IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer,IDispatcherService dispatcherService,IMarketActions marketActions,ISettingsService settingsService)
    {
        _marketActions= marketActions;
        _dispatcherService = dispatcherService;
        _databaseAccessLayer = databaseAccessLayer;
        LocationID = settingsService.LocationId;
        AddMpnCommand = new RelayCommand(ExecuteAddMpnCommand);
        ProductBrowserCommand = new RelayCommand(ExecuteProductBrowserCommand);
        ChangeQuantityCommand = new RelayCommand<casioukcurrentorder>(ExecuteChangeQuantityCommand);
        GenerateOrderCommand = new RelayCommand(ExecuteGenerateOrderCommand);
        SearchCommand = new AsyncRelayCommand(SearchExecute);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddInvoiceCommand = new AsyncRelayCommand(AddInvoiceExecute);
        AddPLCommand = new RelayCommand<SuggestedItem>(AddPLExecute);
        AddUKCommand = new RelayCommand<SuggestedItem>(AddUKExecute);
        PokazujProgressBar = Visibility.Visible;
        _dialogService = dialogService;
        IsInvoiceProcessing = true;
        PokazujInvoiceProgressBar = Visibility.Collapsed;
        CanAddMpn = false;
        
           
    }

    public ICommand AddInvoiceCommand { get; set; }
    public ICommand AddMpnCommand { get; set; }

    private decimal _totalAmount;
    public decimal TotalAmount 
    { 
        get=>_totalAmount;
        set =>SetProperty(ref _totalAmount, value);
    }

    private bool _canAddMpn;
    public bool CanAddMpn
    {
        get => _canAddMpn;
        set
        {
            _canAddMpn = value; 
            OnPropertyChanged(nameof(CanAddMpn));
        }
    }

    public string AddMpnTXT
    {
        get => _addMpnTXT;
        set
        {
            _addMpnTXT = value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                CanAddMpn = true;
            } else
            {
                CanAddMpn = false;
            }
            OnPropertyChanged(nameof(AddMpnTXT));
        }
    }

    public ICommand AddPLCommand { get; set; }
    public ICommand AddUKCommand { get; set; }
    public ObservableCollection<casioukbackorder> BakOrdy { get; set; } = new();
    public string BoHigh
    {
        get => _boHigh;
        set
        {
            _boHigh = value;

            OnPropertyChanged(nameof(BoHigh));

            FilterBakOrdy();
        }
    }

    public ICommand ChangeQuantityCommand { get; set; }
    public ObservableCollection<casioukcurrentorder> CurOrdy { get; set; } = new();
    public string CurOrHigh
    {
        get => _curOrHigh;
        set
        {
            _curOrHigh = value;

            OnPropertyChanged(nameof(CurOrHigh));
            FilterCurOrdy();
        }
    }

    public ICommand GenerateOrderCommand { get; set; }
    public string InvoiceText
    {
        get { return _invoiceText; }
        set
        {
            if (value != _invoiceText)
            {
                _invoiceText = value;
                OnPropertyChanged(nameof(InvoiceText));
            }
        }
    }

    public bool IsDataLoaded
    {
        get { return _isDataLoaded; }
        set
        {
            if (value != _isDataLoaded)
            {
                _isDataLoaded = value;
                OnPropertyChanged(nameof(IsDataLoaded));
            }
        }
    }

    public bool IsInvoiceProcessing
    {
        get { return _isInvoiceProcessing; }
        set
        {
            _isInvoiceProcessing = value;
            OnPropertyChanged(nameof(IsInvoiceProcessing));
        }
    }

    public ICommand LoadDataCommand { get; }
    public Visibility PokazujInvoiceProgressBar
    {
        get { return _pokazujInvoiceProgressBar; }
        set
        {
            _pokazujInvoiceProgressBar = value;
            OnPropertyChanged(nameof(PokazujInvoiceProgressBar));
        }
    }

    public Visibility PokazujProgressBar
    {
        get { return _pokazujProgressBar; }
        set
        {
            _pokazujProgressBar = value;
            OnPropertyChanged(nameof(PokazujProgressBar));
        }
    }

    public ICommand ProductBrowserCommand { get; set; }
    public ICommand SearchCommand { get; set; }
    public ObservableCollection<Wysw> SearchedItems { get; set; } = new ObservableCollection<Wysw>();
    public string SearchMpnText
    {
        get { return _searchMpnText; }
        set
        {
            if (value != _searchMpnText)
            {
                _searchMpnText = value;
                OnPropertyChanged(nameof(SearchMpnText));
            }
        }
    }

    public ObservableCollection<SuggestedItem> SugPLListItems { get; set; } = new ObservableCollection<SuggestedItem>();

    public ObservableCollection<SuggestedItem> SugUKListItems { get; set; } = new ObservableCollection<SuggestedItem>();

    public static void robInvoice(string text)
    {
        StringReader strReader = new StringReader(text);
        string linia = strReader.ReadLine();
        while (linia != null)
        {
            if (linia.EndsWith("GB 300 3060 82"))
            {
                string[] czesci = linia.Split(' ');
                DateTime data = Convert.ToDateTime(czesci[0]);

                linia = strReader.ReadLine();
                if (linia.StartsWith("PP1"))
                {
                    linia = strReader.ReadLine();
                }

                while ((linia != null))
                {
                    if (linia.StartsWith("Y "))
                    {
                        linia = null;
                    }
                    else
                    {
                        if (linia.EndsWith(" Y")&&!linia.EndsWith("ASS'Y")&& !linia.EndsWith("ASS Y"))
                        {
                            string[] czesci1 = linia.Split(' ');
                            var iloscczesci = czesci1.Length;
                            if (!decimal.TryParse(czesci1[iloscczesci - 3], out var cena))
                            {
                                throw new("");
                            }
                              
                            var zakup = new casioinvoice
                            {
                                date = data,
                                mpn = czesci1[0],
                                price = cena,
                                quantity = Convert.ToInt32(czesci1[iloscczesci - 5])
                            };
                            totalO += Convert.ToDecimal(zakup.quantity * zakup.price);
                            doPokazania.Add(zakup);
                            invoiceindexer++;
                        }
                        linia = strReader.ReadLine();
                    }
                }
            }
            linia = strReader.ReadLine();
        }
    }

    public void PrepareAndDisplaySuggestions()
    {
        var listaSugestii = new List<SuggestedItem>();

        foreach (var body in _databaseAccessLayer.items.Where(p => !p.Value.itembody.name.ToLower().Equals("discontinued")))
        {
            var bo = body.Value.itembody;
            if (!CurOrdy.Any(predicate => predicate.mpn.Equals(bo.mpn)) && !BakOrdy.Any(predicate => predicate.mpn.Equals(bo.mpn)))
            {
                var gege = body.Value.ItemHeaders.Sum(p => p.quantity);
                if (gege < 1)
                {
                    if (body.Value.ItemHeaders.Count == 1)
                    {
                        var to = body.Value.ItemHeaders.First();
                        if (to.supplierID == 1)
                        {
                            var sug = new SuggestedItem
                            {
                                mpn = bo.mpn,
                                pricePL = 0,
                                priceUK = to.pricePaid,
                                morExpBy = "0",
                                name = bo.myname
                            };
                            listaSugestii.Add(sug);
                        }
                        else
                        {
                            var sug = new SuggestedItem
                            {
                                mpn = bo.mpn,
                                priceUK = 0,
                                pricePL = to.pricePaid,
                                morExpBy = "0"
                                ,
                                name = bo.myname
                            };
                            listaSugestii.Add(sug);
                        }
                    }
                    else
                    {
                        var heduk = body.Value.ItemHeaders.FirstOrDefault(predicate => predicate.supplierID == 1);

                        var hedpl = body.Value.ItemHeaders.FirstOrDefault(predicate => predicate.supplierID == 2);
                        var hedsw = body.Value.ItemHeaders.FirstOrDefault(predicate => predicate.supplierID == 3);
                        var sug = new SuggestedItem
                        {
                            mpn = bo.mpn,
                            priceUK = 0,
                            pricePL = 0,
                            morExpBy = "0",
                            name = bo.myname
                        };
                        if (heduk != null)
                        {
                            sug.priceUK = heduk.pricePaid;
                        }

                        if (hedpl != null)
                        {
                            sug.pricePL = hedpl.pricePaid;
                        }
                        else
                        {
                            if (hedsw != null)
                            {
                                sug.pricePL = hedsw.pricePaid;
                            }
                        }
                        if (sug.priceUK > 0 && sug.pricePL > 0)
                        {
                            var roz = Convert.ToInt32(Math.Round(sug.pricePL / sug.priceUK * 100));
                            sug.morExpBy = roz.ToString();
                        }
                        listaSugestii.Add(sug);
                    }
                }
            }
        }

        foreach (var su in listaSugestii)
        {
            if (su.pricePL == 0 || Convert.ToInt32(su.morExpBy) > 95)
            {
                SugUKListItems.Add(su);
            }
            else
            {
                SugPLListItems.Add(su);
            }
        }
    }

    private async Task AddInvoiceExecute()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            PokazujInvoiceProgressBar = Visibility.Visible;
            IsInvoiceProcessing = false;
        });

        string text = InvoiceText;
        string[] stringSeparators = ["INVOICE"];
        var invoices = text.Split(stringSeparators, StringSplitOptions.None);

        doPokazania = [];
        foreach (var inv in invoices)
        {
            robInvoice(inv);
        }

        var hoho = "";
        var BodiesToAdd = new List<Body2Add>();

        var Quantitiez2Change = new List<Body2Add>();
        var cialka = _databaseAccessLayer.items.Values.Select(p => p.itembody);
        foreach (var doz in doPokazania)
        {
   

            var ax = cialka.FirstOrDefault(p => p.mpn.Equals(doz.mpn));

            if (ax == null)
            {
                hoho = hoho + doz.mpn + " not found the the db" + Environment.NewLine;

                var s = new itembody
                {
                    brandID = 1,
                    description = "",
                    fullsearchterm = "",
                    mpn = doz.mpn,
                    myname = doz.mpn,
                    name = doz.mpn,
                    typeId = 1,
                    weight=0,
                    readyTotrack = true,
                    visible = true
                };
                var b2a = new Body2Add
                {
                    body = s,
                    price = Convert.ToDecimal(doz.price) ,
                    quantity = doz.quantity,
                    when = doz.date,
                    xchgrate = 1,
                    vatrate = 1,
                    acquiredcurrency = "GBP",
                    purchasecurrency = "GBP"
                };
                BodiesToAdd.Add(b2a);
            }
            else
            {
                var df = _databaseAccessLayer.items[ax.itembodyID].ItemHeaders.FirstOrDefault(p => p.locationID == LocationID && p.supplierID == 1);
                if (df == null)
                {
                    await _databaseAccessLayer.AddNewItemHeader(1, Convert.ToDecimal(doz.price) , ax.itembodyID, 0,LocationID,1,1,"GBP","GBP");
                }
                Quantitiez2Change.Add(new Body2Add
                {
                    body = ax,
                    price = Convert.ToDecimal(doz.price) ,
                    quantity = doz.quantity,
                    when = doz.date,
                    xchgrate = 1,
                    vatrate = 1,
                    acquiredcurrency = "GBP",
                    purchasecurrency = "GBP"
                });
            }
        }
        var haha = await _databaseAccessLayer.AddNewBodz(BodiesToAdd, LocationID);

        foreach (var q2c in Quantitiez2Change)
        {
            var hedzik = _databaseAccessLayer.items.FirstOrDefault(p => p.Key == q2c.body.itembodyID).Value.ItemHeaders.FirstOrDefault(p => p.supplierID == 1);
            var total = await _databaseAccessLayer.IncreaseQuantityInHeader(hedzik.itemheaderID, q2c.quantity, " while doing invoice");
            await _marketActions.AssignQuantities2markets(hedzik.itembodyID, LocationID, pisz);

            var mybeki = _originalBakOrdy.Where(p => p.mpn.Equals(q2c.body.mpn)).ToArray();
            if (mybeki.Count() == 0)
            {
                hoho += q2c.body.mpn + " not found in backorders\r\n";
            }
            var rou = q2c.quantity;
            // w hue mam ilosc danego mpn we wszystkicg back order
            var hue = mybeki.Sum(p => p.quantity);
            if (hue < q2c.quantity)
            {
                hoho += rou + " of " + q2c.body.mpn + " arrived, but only " + hue + " was in backordrs";
                rou = hue;
            }
            // rou - ilosc do wyeliminowania z backorders, max wartosc to hue
            // rou bedzie zawsze najwyzej rowne ilosci mpn z bakorders

            int j = 0;
              
            while (rou > 0)
            {
                if (mybeki[j].quantity > rou)
                {
                    var idi = mybeki[j].casioUKbackorderId;
                    var dousu = _originalBakOrdy.First(p => p.casioUKbackorderId == idi);
                    mybeki[j].quantity -= rou;
                    dousu.quantity = mybeki[j].quantity;
                    rou = 0;

                    await  _databaseAccessLayer.UpdateQuantityInCasioBackorder(idi, dousu.quantity);
                }
                else
                {
                    rou -= mybeki[j].quantity;

                    var idi = mybeki[j].casioUKbackorderId;

                    await _databaseAccessLayer.RemoveCasioBackorder(idi);
                }

                j++;
            }
        }
        await _databaseAccessLayer.AddCasioInvoices(doPokazania);

        var qvm = new XLSXViewModel(hoho, true);
        qvm.RequestClose += async (sender, e) =>
        {
        };
        _ = _dialogService.ShowDialog(qvm);

        await LoadCurAndBackOrds();
        Application.Current.Dispatcher.Invoke(() =>
        {
            PokazujInvoiceProgressBar = Visibility.Collapsed;
            IsInvoiceProcessing = true;
        });

        void pisz(string s)
        {}
    }

    private void AddPLExecute(SuggestedItem si)
    {
        var qvm = new QuantityViewModel(1);
        qvm.RequestClose += async (sender, e) =>
        {
            var result = ((QuantityViewModel)sender).Response;

            if (result != -1)
            {
                var cuo = await _databaseAccessLayer.AddCasioUKCurOrd(new casioukcurrentorder
                {
                    mpn = si.mpn,
                    quantity = result
                });
                if (cuo != null)
                {
                    CurOrdy.Add(cuo);
                    _originalCurOrdy.Add(cuo);
                    SugPLListItems.Remove(si);
                }
            }
        };
        _dialogService.ShowDialog(qvm);
    }

    private void AddUKExecute(SuggestedItem si)
    {
        var qvm = new QuantityViewModel(1);
        qvm.RequestClose += async (sender, e) =>
        {
            var result = ((QuantityViewModel)sender).Response;

            if (result != -1)
            {
                var cuo = await _databaseAccessLayer.AddCasioUKCurOrd(new casioukcurrentorder
                {
                    mpn = si.mpn,
                    quantity = result
                });
                if (cuo != null)
                {
                    CurOrdy.Add(cuo);
                    _originalCurOrdy.Add(cuo);
                    SugUKListItems.Remove(si);
                }
            }
        };
        _dialogService.ShowDialog(qvm);
    }

    public void CountTotal()
    {
        if (!IsDataLoaded)
        {
            return;
        }
        decimal ta = 0;
        foreach (var co in _originalCurOrdy)
        {
            if (Prices.ContainsKey(co.mpn))
            {
                ta += (Prices[co.mpn] * co.quantity);
            }
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            TotalAmount = ta; 
        });
          
    }



    private void ExecuteAddMpnCommand()
    {
        Regex regex = new Regex(@"^\d+$");
        if (AddMpnTXT.Length != 8 || regex.IsMatch(AddMpnTXT))
        {
            var qvm = new QuantityViewModel(1);
            qvm.RequestClose += async (sender, e) =>
            {
                var result = ((QuantityViewModel)sender).Response;

                if (result != -1)
                {
                    var cuo = await _databaseAccessLayer.AddCasioUKCurOrd(new casioukcurrentorder
                    {
                        mpn = AddMpnTXT,
                        quantity = result
                    });
                    if (cuo != null)
                    {
                        CurOrdy.Add(cuo);
                        _originalCurOrdy.Add(cuo);
                    }
                    CountTotal();
                }
            };
            _dialogService.ShowDialog(qvm);
        }
        else
        {
            _dialogService.ShowMessage("Incorrect MPN", "Enter correct MPN value");
        }
    }

    private void ExecuteChangeQuantityCommand(casioukcurrentorder cuo)
    {
        var qvm = new QuantityViewModel(cuo.quantity);
        qvm.RequestClose += async (sender, e) =>
        {
            var result = ((QuantityViewModel)sender).Response;
            if (result != -1)
            {
                int index1 = _originalCurOrdy.IndexOf(cuo);
                int index2 = CurOrdy.IndexOf(cuo);
                cuo.quantity = result;
                var cuo2 = await _databaseAccessLayer.RefreshUKCurOrd(cuo);
                _originalCurOrdy[index1] = cuo2;
                if (index2 != -1)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CurOrdy[index2] = cuo2;
                    });
                }
                CountTotal();
            }
        };
        _dialogService.ShowDialog(qvm);
    }

    private void ExecuteGenerateOrderCommand()
    {
        if (_originalCurOrdy.Count == 0)
        {
            return;
        }
        var dzis = DateTime.Now;
        var nobe = new List<casioukbackorder>();
        var cube = new List<casioukcurrentorder>();
        string wy = "";
        foreach (var ne in _originalCurOrdy)
        {
            var nob = new casioukbackorder
            {
                mpn = ne.mpn,
                orderedon = dzis,
                quantity = ne.quantity
            };
            cube.Add(ne);
            nobe.Add(nob);
            wy += nob.mpn + "\t" + nob.quantity + "\r\n";
        }

        var qvm = new XLSXViewModel(wy, true);
        qvm.RequestClose += async (sender, e) =>
        {
            var result = ((XLSXViewModel)sender).Response;
            if (result)
            {
                await _databaseAccessLayer.CurOrds2BackOrds(nobe);
                await LoadCurAndBackOrds();
            }
        };
        _dialogService.ShowDialog(qvm);
    }

    private void ExecuteProductBrowserCommand()
    {
        var addsomempn = new ProductBrowserViewModel(_databaseAccessLayer);

        addsomempn.RequestClose += async (sender, e) =>
        {
            var result = ((ProductBrowserViewModel)sender).Result;
            if (result != -1)
            {
                var empen = _databaseAccessLayer.items[result].itembody.mpn;
                var qvm = new QuantityViewModel(1);
                qvm.RequestClose += async (sender, e) =>
                {
                    var result = ((QuantityViewModel)sender).Response;

                    if (result != -1)
                    {
                        var cuo = await _databaseAccessLayer.AddCasioUKCurOrd(new casioukcurrentorder
                        {
                            mpn = empen,
                            quantity = result
                        });
                        if (cuo != null)
                        {
                            CurOrdy.Add(cuo);
                            _originalCurOrdy.Add(cuo);
                        }
                    }
                };
                _dialogService.ShowDialog(qvm);
            }
        };
        _dialogService.ShowDialog(addsomempn);
    }

    private void FilterBakOrdy()
    {
        BakOrdy.Clear();
        var filteredItems = string.IsNullOrEmpty(BoHigh)
            ? _originalBakOrdy
            : _originalBakOrdy.Where(item => item.mpn.Contains(BoHigh)).ToList();

        foreach (var item in filteredItems)
        {
            BakOrdy.Add(item);
        }
    }

    private void FilterCurOrdy()
    {
        CurOrdy.Clear();
        foreach (var item in _originalCurOrdy)
        {
            if (string.IsNullOrWhiteSpace(CurOrHigh) || item.mpn.Contains(CurOrHigh))
            {
                CurOrdy.Add(item);
            }
         
        }
    }

    private async Task LoadCurAndBackOrds()
    {
        var newItems = (await _databaseAccessLayer.GetCasioUKBackorders()).OrderBy(p => p.orderedon);
        var newItems2 = (await _databaseAccessLayer.GetCasioUKCurrentorders()).OrderBy(p => p.casioUKcurrentOrderId);
           
        _dispatcherService.Invoke(() =>
        {
            PokazujProgressBar = Visibility.Visible;

            foreach (var item in newItems)
            {
                BakOrdy.Add(item);
                _originalBakOrdy.Add(item);
            }
            TotalAmount = 0;
            foreach (var item in newItems2)
            {
               
                CurOrdy.Add(item);
                _originalCurOrdy.Add(item);
            }
            CountTotal();
            PrepareAndDisplaySuggestions();
            PokazujProgressBar = Visibility.Collapsed;
        });
    }

    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        await _databaseAccessLayer.GetPackage(1);
        await LoadCurAndBackOrds();
        IsDataLoaded = true;
        foreach (var item in _databaseAccessLayer.items)
        {
            var produ = item.Value.ItemHeaders.FirstOrDefault(p => p.supplierID == 1 && p.locationID == 1);
            if (produ != null)
            {
                Prices.Add(item.Value.itembody.mpn, Convert.ToDecimal(produ.pricePaid));
            }
        }
        CountTotal();
    }
    private async Task SearchExecute()
    {
        var doPopulacji = new List<Wysw>();

        var znalezione = await _databaseAccessLayer.FindAmongCasioUKInvoices(SearchMpnText);

        foreach (var empn in znalezione)
        {
            doPopulacji.Add(

                new Wysw
                {
                    mpn = empn.mpn,
                    quantity = empn.quantity,
                    deliveredon = empn.date.ToString("dd-MM-yyyy"),
                    price = empn.price.ToString()
                });
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            SearchedItems.Clear();
            foreach (var item in doPopulacji)
            {
                SearchedItems.Add(item);
            }
        });
    }
}