using DataServicesNET80;
using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Models;
using denModels;
using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class AddMarketViewModel : ObservableObject, IAsyncDialogViewModel
{
      
    public event AsyncEventHandler RequestClose;



    private FullItmMarketAss _result;
    private bool _dialogResult;
    private readonly IDialogService _dialogService;
    private FullItmMarketAss _itema;


    private bool _brandnew;
    public bool BrandNew
    {
        get => _brandnew;
        set => SetProperty(ref _brandnew, value);
    }

    private Idname _soldWithContent;
    public Idname SoldWithContent
    {
        get => _soldWithContent;
        set => SetProperty(ref _soldWithContent, value);
    }
    private List<Idname> _markets;
    public List<Idname> Markets
    {
        get => _markets;
        set => SetProperty(ref _markets, value);
    }

    private List<Idname> _quantitiesSold;
    public List<Idname> QuantitiesSold
    {
        get => _quantitiesSold;
        set => SetProperty(ref _quantitiesSold, value);
    }

    private Idname _selectedMarket;
    public Idname SelectedMarket
    {
        get => _selectedMarket;
        set => SetProperty(ref _selectedMarket, value);
    }

    private Idname _selectedQuantitySold;
    public Idname SelectedQuantitySold
    {
        get => _selectedQuantitySold;
        set => SetProperty(ref _selectedQuantitySold, value);
    }

    private string _SEOName;
    public string SEOName
    {
        get => _SEOName;
        set => SetProperty(ref _SEOName, value);
    }

    private string _SKU;
    public string SKU
    {
        get => _SKU;
        set => SetProperty(ref _SKU, value);
    }

    private string _itemNumber;
    public string ItemNumber
    {
        get => _itemNumber;
        set => SetProperty(ref _itemNumber, value);
    }

    public ICommand AddSoldTogetherCommand { get; }
    public ICommand RemoveSoldTogetherCommand { get; }


    private async Task AddSoldTogether()
    {
        var addsold2gether = new ProductBrowserViewModel(_databaseAccessLayer);

        addsold2gether.RequestClose += async (sender, e) =>
        {
            var result = ((ProductBrowserViewModel)sender).Result;
            if (result != -1)
            {
                SoldWithContent = new Idname { Name = _databaseAccessLayer.items[result].itembody.name, Id = result };

            }
        };
        await _dialogService.ShowDialog(addsold2gether);
    }
    private void RemoveSoldTogether()
    {
        SoldWithContent = null;
    }
    public IDatabaseAccessLayer _databaseAccessLayer;
    private int _locationid;
    public AddMarketViewModel(List<Idname> markets, IDialogService ds, IDatabaseAccessLayer databaseAccessLayer,int locationid, FullItmMarketAss itema = null)
    {
        _locationid = locationid;
        _itema = itema;
        _markets = markets;
        _databaseAccessLayer = databaseAccessLayer;
        _quantitiesSold = new List<Idname>();

        for (int i = 1; i < 10; i++)
        {
            _quantitiesSold.Add(new Idname { Id = i, Name = i.ToString() });
        }
        QuantitiesSold = _quantitiesSold;
        if (_itema.itmmarketassoc.itmmarketassID == -1)
        {
            BrandNew = true;
            SelectedQuantitySold = QuantitiesSold.First();
            SelectedMarket = Markets.First();
            SoldWithContent = null;
        }
        else
        {
            BrandNew = false;
            SelectedQuantitySold = QuantitiesSold.First(p => p.Id == itema.itmmarketassoc.quantitySold);
            SelectedMarket = markets.First(p => p.Id == itema.itmmarketassoc.marketID);
            ItemNumber = itema.itmmarketassoc.itemNumber;
            SEOName = itema.itmmarketassoc.SEName;
            if (itema.itmmarketassoc.soldWith == null)
            {
                SoldWithContent = null;
            }
            else
            {
                SoldWithContent = new Idname { Name = _databaseAccessLayer.items[(int)itema.itmmarketassoc.soldWith].itembody.name, Id = (int)itema.itmmarketassoc.soldWith };
            }
            if (itema.SKU != null)
            {
                SKU = itema.SKU.sku;
            }
        }


        OKCommand = new AsyncRelayCommand(OKExecute);
        CancelCommand = new RelayCommand(async () => await Cancel());
        AddSoldTogetherCommand = new AsyncRelayCommand(AddSoldTogether);
        RemoveSoldTogetherCommand = new RelayCommand(RemoveSoldTogether);

        _dialogService = ds;
    }



    // Analogiczne właściwości dla pozostałych pól

    public ICommand OKCommand { get; }
    public ICommand CancelCommand { get; }
    public FullItmMarketAss Result { get; set; }

    private async Task OKExecute()
    {
        if (string.IsNullOrEmpty(ItemNumber))
        {
            await _dialogService.ShowMessage("Item number missing", "Please enter item number");
            return;
        }
        if (SelectedMarket.Id != 1 && string.IsNullOrEmpty(SKU))
        {
            await _dialogService.ShowMessage("SKU missing", "Please enter SKU");
            return;
        }
        Result =await GetResult();

        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private async Task Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private async Task<FullItmMarketAss> GetResult()
    {

        if (_itema.itmmarketassoc.itmmarketassID != -1)
        {
            _itema.itmmarketassoc.SEName = SEOName;
            _itema.itmmarketassoc.quantitySold = Convert.ToInt32(SelectedQuantitySold.Id);
            _itema.itmmarketassoc.itemNumber = ItemNumber;
            _itema.itmmarketassoc.soldWith = SoldWithContent?.Id ?? null;
            if (!string.IsNullOrEmpty(SKU))
            {
                if (_itema.SKU != null)
                {
                    _itema.SKU.sku = SKU;
                    _itema.SKU.locationID = _locationid;
                } else
                {
                    var asku=new asinsku { asin= ItemNumber, locationID=1,sku=SKU};
                    _itema.SKU=await _databaseAccessLayer.AddAsinSku(asku);
                }
            }
            return _itema;

        }
        var ix = new FullItmMarketAss
        {
            itmmarketassoc = new itmmarketassoc
            {
                SEName = SEOName,
                quantitySold = Convert.ToInt32(SelectedQuantitySold.Id),
                itemNumber = ItemNumber,
                locationID = _locationid,
                marketID = Convert.ToInt32(SelectedMarket.Id),
                itembodyID = _itema.itmmarketassoc.itembodyID,
                soldWith = SoldWithContent?.Id ?? null
            },
            SKU = new asinsku
            {
                asin = ItemNumber,
                sku = SKU
            }
        };
        return ix;
    }


}