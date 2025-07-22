using DataServicesNET80.DatabaseAccessLayer;
using denModels.MarketplaceServices;
using denSharedLibrary;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static denSharedLibrary.Colours;

namespace denViewModels.ProductBrowser;

public class SyncQuantitiesWithMarketPlacesViewModel : ObservableObject, IAsyncDialogViewModel
{
    public class OneMarketPlace : ObservableObject
    {
        public int marketId;
        private bool _isProcessing=false;
        public bool IsProcessing
        {
            get=>_isProcessing;
            set=>SetProperty(ref _isProcessing, value);
        }
        private string _marketPlaceName;
        public string MarketPlaceName
        {
            get => _marketPlaceName;
            set=>SetProperty(ref _marketPlaceName, value);
        }
        private string _result;
        public string Result
        {
            get=> _result;
            set=>SetProperty(ref _result, value);
        }
        private RGB _colour;
        public RGB Colour
        {
            get => _colour;
            set => SetProperty(ref _colour, value);
        }
    }

    private bool _canLeave=false;
    public bool CanLeave
    {
        get => _canLeave;
        set 
        {
            SetProperty(ref _canLeave, value); 
            OnPropertyChanged(nameof(CanCancel));
        }
    }
    private bool _sizeToContent;
    public bool SizeToContent
    {
        get => _sizeToContent;
        set => SetProperty(ref _sizeToContent, value);

    }
    public bool CanCancel
    {
        get => !_canLeave;
       
    }

    public ObservableCollection<OneMarketPlace> AllMarketPlaces { get; set; }=new ObservableCollection<OneMarketPlace>();

    public ICommand CancelCommand { get; set; }
    public ICommand OkCommand { get; set; }
    public IAsyncRelayCommand LoadDataCommand {  get; set; }

    //private SizeToContent _sizeToContent = SizeToContent.Manual;
    //public SizeToContent SizeToContent
    //{
    //    get => _sizeToContent;
    //    set => SetProperty(ref _sizeToContent, value);
            
    //}


    IDatabaseAccessLayer _databaseAccessLayer;
    IDialogService _dialogService;
    IDispatcherService _dispatcherService;
    private IMarketActions _marketActions;
    int Itembodyid;
    int Quantity = 0;
    int locationID;
    public SyncQuantitiesWithMarketPlacesViewModel(int itembodyid, int quantity,int locationid, IDatabaseAccessLayer databaseAccessLayer,IDialogService dialogService,IDispatcherService dispatcherService,IMarketActions marketActions)
    {
        _marketActions=marketActions;
        locationID=locationid;
        Itembodyid=itembodyid;
        _dispatcherService = dispatcherService;
        _databaseAccessLayer= databaseAccessLayer;
        _dialogService= dialogService;
        Quantity = quantity;
        LoadDataCommand = new AsyncRelayCommand(LoadDataExecute);
        OkCommand = new RelayCommand(OkExecute);
        CancelCommand = new AsyncRelayCommand(CancelExecute);
    }


    public async Task CancelExecute()
    {
        var question =await _dialogService.ShowYesNoMessageBox(denLanguageResourses.Resources.Info, denLanguageResourses.Resources.ConfirmCancelOperation);
        if (!question) { return; }            
        _dispatcherService.Invoke(() => 
        {
            var ilosc= AllMarketPlaces.Where(p => p.IsProcessing).Count();
            if (ilosc == 0)
            {
                _dialogService.ShowMessage(denLanguageResourses.Resources.Info, denLanguageResourses.Resources.OperationCompletedBeforeCancel);
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
        });
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public void OkExecute()

    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public async Task UpdateOnOneMarket(string itm, OneMarketPlace omp, int MarketQuantity)
    {
        var ret =await _marketActions.UpdateQuantityOnMarketplace(itm, MarketQuantity,  locationID,omp.marketId,pisz);
        var color = new RGB();
        if (ret.Status == UpdateQuantityStatus.OK) 
        {
            color.R = 0;
            color.G = 255;
            color.B = 0;
        }
        if (ret.Status == UpdateQuantityStatus.Error)
        {
            color.R = 255;
            color.G = 0;
            color.B = 0;
        }
        if (ret.Status == UpdateQuantityStatus.NotAttempted)
        {
            color.R = 128;
            color.G = 128;
            color.B = 128;
        }
        _dispatcherService.Invoke(() =>
        {
            omp.Colour = color;
            omp.IsProcessing = false;
            omp.Result= ret.Response.Code+" "+ret.Response.Message;
        });

        void pisz(string s)
        { }
    }

      
    public Dictionary<KeyValuePair<string,int>, OneMarketPlace> item2statusDict = new();
        
        
       

    public async Task LoadDataExecute()
    {
         
        var asinSkus = ((await _databaseAccessLayer.ASINSKUS()).Where(p => p.locationID == locationID).GroupBy(p => p.asin).ToDictionary(p => p.Key, q => q.Select(p => p.sku).ToList()));
        var produkty = _databaseAccessLayer.items[Itembodyid].ItmMarketAssocs.Where(p => p.locationID == locationID);
        var markety = await _databaseAccessLayer.markety();
        
        var color = new RGB()
        {
            R = 255,
            G = 255,
            B = 255,
        };
        foreach (var prod in produkty)
        {
            int quantity = Quantity / prod.quantitySold;
            if (prod.soldWith != null)
            {
                int qs = _databaseAccessLayer.items[(int)prod.soldWith].ItemHeaders.Select(p=>p.quantity).Sum();
                quantity=Math.Min(quantity, qs/prod.quantitySold);
            }
            var tst2 = await _databaseAccessLayer.MarketPlatformAssociations();
            var marketplace2platform = (await _databaseAccessLayer.MarketPlatformAssociations()).FirstOrDefault(p => p.marketID == prod.marketID);
            var platform = (await _databaseAccessLayer.Platformy())[marketplace2platform.platformID];
            if (platform.name.ToLower().StartsWith("amazon"))
            {
                foreach (var sku in asinSkus[prod.itemNumber])
                {
                    var amomp = new OneMarketPlace
                    {
                        IsProcessing = true,
                        MarketPlaceName = markety[prod.marketID].name,
                        Colour = color,
                        marketId = prod.marketID,
                    };
                    item2statusDict.Add(new KeyValuePair<string, int>(sku,quantity), amomp);
                    AllMarketPlaces.Add(amomp);
                }
            }
            else
            {
                var omp = new OneMarketPlace
                {
                    IsProcessing = true,
                    MarketPlaceName = markety[prod.marketID].name,
                    Colour = color,
                    marketId = prod.marketID,
                };
                item2statusDict.Add(new KeyValuePair<string, int>(prod.itemNumber,quantity), omp);
                AllMarketPlaces.Add(omp);
            }
        }

        SizeToContent = true;
        List<Task> taski = new List<Task>();
        foreach(var aomp in item2statusDict)
        {
            taski.Add(UpdateOnOneMarket(aomp.Key.Key, aomp.Value,aomp.Key.Value));
        }
        await Task.WhenAll(taski);
        CanLeave = true;
    }

    public event AsyncEventHandler RequestClose;
        
}