using denSharedLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static MucPartsNET80.MyDatabase;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using WatchExplorerMVVM.Model;
using DataServicesNET80.DatabaseAccessLayer;
using shookayNET;

namespace WatchExplorerMVVM;

public class WatchExplorerViewModel : INotifyPropertyChanged
{

    private string _zagareczekTBText;
    public string ZagareczekTBText
    {
        get { return _zagareczekTBText; }
        set
        {
            _zagareczekTBText = value;
            OnPropertyChanged(nameof(ZagareczekTBText));
            OnSearchChanged();
        }
    }


    private string _watchesFoundText;
    public string WatchesFoundText
    {
        get { return _watchesFoundText; }
        set
        {
            _watchesFoundText = value;
            OnPropertyChanged(nameof(WatchesFoundText));
        }
    }

    private ObservableCollection<object> _zegarkiItems;
    public ObservableCollection<object> ZegarkiItems
    {
        get { return _zegarkiItems; }
        set
        {
            _zegarkiItems = value;
            OnPropertyChanged(nameof(ZegarkiItems));
        }
    }

    private bool _bButtonEnabled;
    public bool BButtonEnabled
    {
        get { return _bButtonEnabled; }
        set
        {
            _bButtonEnabled = value;
            OnPropertyChanged(nameof(BButtonEnabled));
        }
    }

    private bool _nButtonEnabled;
    public bool NButtonEnabled
    {
        get { return _nButtonEnabled; }
        set
        {
            _nButtonEnabled = value;
            OnPropertyChanged(nameof(NButtonEnabled));
        }
    }

    private string _watchNameLabelText;
    public string WatchNameLabelText
    {
        get { return _watchNameLabelText; }
        set
        {
            _watchNameLabelText = value;
            OnPropertyChanged(nameof(WatchNameLabelText));
        }
    }

    private string _mpnentryText;
    public string MpnentryText
    {
        get { return _mpnentryText; }
        set
        {
            _mpnentryText = value;
            OnPropertyChanged(nameof(MpnentryText));
        }
    }

    private ObservableCollection<object> _czesciItems;
    public ObservableCollection<object> CzesciItems
    {
        get { return _czesciItems; }
        set
        {
            _czesciItems = value;
            OnPropertyChanged(nameof(CzesciItems));
        }
    }

    private bool _isDataLoaded = false;
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

    public ICommand ZagareczekTBTextChanged { get; set; }
    public ICommand ZegarkiSelectionChanged { get; set; }
    public ICommand BButtonClicked { get; set; }
    public ICommand NButtonClicked { get; set; }
    public ICommand LookupMpnClicked { get; set; }
    public ICommand CzesciSelectionChanged { get; set; }
    public ICommand AddToCasioBackorderClicked { get; set; }
    public ICommand LoadDataCommand { get; }

    public ObservableCollection<watchviewitem> zegaruchny { get; set; } = new ObservableCollection<watchviewitem>();
    public ObservableCollection<watchviewitem> zegaruchnyView { get; set; } = new ObservableCollection<watchviewitem>();

    public ICommand RowClickCommand { get; }
    public ICommand PartClickCommand { get; }
    Dictionary<string, int> mpns;
    private bool _isStackPanelVisible;
    public bool IsStackPanelVisible
    {
        get { return _isStackPanelVisible; }
        set
        {
            _isStackPanelVisible = value;
            OnPropertyChanged(nameof(IsStackPanelVisible));
        }
    }



    private string _coSkopio;
    public string CoSkopio
    {
        get { return _coSkopio; }
        set
        {
            _coSkopio = value;
            OnPropertyChanged(nameof(CoSkopio));
        }
    }

    public IDatabaseAccessLayer _databaseAccessLayer;
    private IHttpClientFactory _httpClientFactory;
    public WatchExplorerViewModel(IDatabaseAccessLayer databaseAccessLayer, IHttpClientFactory httpClientFactory)
    {
        IsDataLoaded = false;
        IsStackPanelVisible = false;
        _databaseAccessLayer = databaseAccessLayer;
        _httpClientFactory = httpClientFactory;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        RowClickCommand = new AsyncRelayCommand<watchviewitem>(ExecuteWatchClick);
        PartClickCommand = new AsyncRelayCommand<partviewitem>(ExecutePartClick);

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public event Action<int> ProductSelected;
    public async Task ExecutePartClick(partviewitem czesc)
    {
        if (czesc == null) return;
        if (czesc.available.StartsWith("in s"))
        {
            var itembodyid = _databaseAccessLayer.items.First(p => p.Value.itembody.mpn.Equals(czesc.mpn)).Value.itembody.itembodyID;
            ProductSelected?.Invoke(itembodyid);
            return;
        }
        if (czesc.available.ToLower().Equals("available"))
        {
            Clipboard.SetDataObject(czesc.mpn);
            IsStackPanelVisible = true;
            CoSkopio = czesc.mpn;
            await Task.Delay(2000);
            IsStackPanelVisible = false;
            CoSkopio = "";
        }
    }


    MucPartsNET80.watch[] rawWatches;
    bool firstrun = true;
    ShookayWrapper<StringInt> se;
    private Visibility _pokazujProgressBar;
    public Visibility PokazujProgressBar
    {
        get { return _pokazujProgressBar; }
        set
        {
            _pokazujProgressBar = value;
            OnPropertyChanged(nameof(PokazujProgressBar));
        }
    }


    private BitmapImage _fotkaZegarka;
    public BitmapImage FotkaZegarka
    {
        get { return _fotkaZegarka; }
        set
        {
            _fotkaZegarka = value;
            OnPropertyChanged(nameof(FotkaZegarka));
        }
    }


    private bool _doesWatchHavePhoto;
    public bool DoesWatchHavePhoto
    {
        get { return _doesWatchHavePhoto; }
        set
        {
            if (value != _doesWatchHavePhoto)
            {
                _doesWatchHavePhoto = value;
                OnPropertyChanged(nameof(DoesWatchHavePhoto));
            }
        }
    }


    public async Task ExecuteWatchClick(watchviewitem zeg)
    {
        if (zeg == null)
        {
            return;
        }
        WatchNameLabelText = zeg.name;
        var watch = rawWatches.Single(p => p.watch_id == zeg.id);
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        Czesciuchny.Clear();
        var czastki = await Task.Run(() => MucPartsNET80.MyDatabase.getAllPartsForWatch(zeg.id), token);
        foreach (var cz in czastki)
        {


            Czesciuchny.Add(new partviewitem
            {
                id = cz.part_id,
                mpn = cz.mpn,
                type = partTypes.Single(p => p.part_type_id == cz.part_type_id).name,
                available = "checking...",


            });



        }

        Parallel.For(0, czastki.Count(), async i =>
        {

            bool wartosc = await CasioInteractions.NLAGokanCheck(czastki[i].mpn, _httpClientFactory, token);
            string coprzypisac = "";
            if (wartosc)
            {
                if (mpns.ContainsKey(czastki[i].mpn))

                {
                    coprzypisac = "in stock (" + mpns[czastki[i].mpn] + ')';
                }
                else
                {
                    coprzypisac = "available";
                }

            }
            else
            {
                coprzypisac = "discontinued";
            }


            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!token.IsCancellationRequested)
                {
                    Czesciuchny[i].available = coprzypisac;
                }
            });

        });








        if (watch.picname != "")
        {

            DoesWatchHavePhoto = true;


            var image = new BitmapImage();
            image.BeginInit();
            string fileName = @"c:\mucpacs\watches\" + watch.nicename.ToLower() + @".jpg";
            image.UriSource = new Uri(fileName);
            image.EndInit();
            FotkaZegarka = image;
        }
        else
        {
            DoesWatchHavePhoto = false;
        }
    }

    part_type[] partTypes;


    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        PokazujProgressBar = Visibility.Visible;
        var mydir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string cs = @"URI=file:" + System.IO.Path.Combine(mydir, @"data\zegareczkidb.db");
        MucPartsNET80.MyDatabase.setConnection(cs);
        var families = await MucPartsNET80.MyDatabase.getAllFamilies();
        partTypes = await MucPartsNET80.MyDatabase.getAllPartTypes();
        rawWatches = await MucPartsNET80.MyDatabase.getAllWatches();
        var rawParts = await MucPartsNET80.MyDatabase.getAllParts();
        var rawParts_relation = await MucPartsNET80.MyDatabase.getAllWatchPartRelations();
        var watchLinks = new Dictionary<int, int>();
        for (int i = 0; i < rawWatches.Length; i++)
        {
            watchLinks.Add(rawWatches[i].watch_id, i);
        }
        var zegareczki = new List<StringInt>();
        foreach (MucPartsNET80.watch wat in rawWatches)
        {
            if (string.IsNullOrEmpty(wat.watch_searchfield))
            {
                var tju = 1;
            }
            zegareczki.Add(new StringInt(  wat.watch_searchfield + ' ' + wat.nicename,wat.watch_id));
        }
        
        foreach (var zeg in rawWatches)
        {
            var o =
                new watchviewitem
                {
                    id = zeg.watch_id,
                    name = zeg.nicename,

                };
            var txa = families.Single(p => p.family_id == zeg.family_id);
            if (txa.family_id != 1)
            {
                o.family = txa.family_name;
            }

            zegaruchny.Add(o);
        }
        if (_databaseAccessLayer.DALState == DatabaseAccessLayerState.UnLoaded)
        {
            await _databaseAccessLayer.GetPackage(1);
        }

        if (_databaseAccessLayer.DALState == DatabaseAccessLayerState.Loading)
        {
            await Task.Run(() =>
            {
                while (_databaseAccessLayer.DALState != DatabaseAccessLayerState.Loaded)
                {
                    Task.Delay(500); // Delay to avoid tight loop that hogs CPU
                }
            });
        }
        mpns = _databaseAccessLayer.items.Select(p => p.Value).ToDictionary(p => p.itembody.mpn, q => q.ItemHeaders.Sum(p => p.quantity));
        se =     new ShookayWrapper<StringInt>(zegareczki);
        await se.PrepareEntries();
        IsDataLoaded = true;

        OnSearchChanged();

    }

    private CancellationTokenSource _cancellationTokenSource;
    private async void OnSearchChanged()
    {

        _cancellationTokenSource.Cancel(); // Cancel any ongoing search
        _cancellationTokenSource = new CancellationTokenSource(); // Create new cancellation token source

        var token = _cancellationTokenSource.Token;
        var searchByName = ZagareczekTBText?.ToLower();  // use ToLower for string comparison


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
            List<watchviewitem> results;

            if (string.IsNullOrEmpty(searchByName))// && string.IsNullOrEmpty(searchByMpn))
            {
                // If both search fields are empty, return all products
                results = await Task.Run(() => zegaruchny.ToList(), token);
            }
            else
            {

                // Otherwise, perform the search
                results = await Task.Run(async () =>
                {
                    var gux =await se.FindWithin(searchByName);
                    return zegaruchny.Where(item => gux.Contains(item.id)).ToList();
                }, token);
            }

            if (!token.IsCancellationRequested) // Proceed only if there were no new changes during the search
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    zegaruchnyView.Clear();
                    foreach (var result in results)
                    {
                        zegaruchnyView.Add(result);
                    }
                });
            }
        }
        if (firstrun)
        {
            PokazujProgressBar = Visibility.Collapsed;
            firstrun = false;
        }
    }
    public ObservableCollection<partviewitem> Czesciuchny { get; set; } = new ObservableCollection<partviewitem>();

    public async Task PopulateParts(int id, CancellationToken token)
    {




        Parallel.For(0, Czesciuchny.Count, async i =>
        {

            await Task.Run(() => checkIfPartSells(i, Czesciuchny[i].mpn, token), token);

        });
    }
    public async Task checkIfPartSells(int id, string mpn, CancellationToken token)
    {
        bool tut;


        tut = await Task.Run(() => CasioInteractions.NLAGokanCheck(mpn,  _httpClientFactory,token), token);
           


        //if (czesciuchny == null || czesciuchny.Count < id)
        //{
        //    return;
        //}

        if (tut)
        {
            if (mpns.ContainsKey(mpn))

            {
                Czesciuchny[id].available = "in stock (" + mpns[mpn] + ')';
            }
            else
            {
                Czesciuchny[id].available = "available";
            }

        }
        else
        {
            Czesciuchny[id].available = "discontinued";
        }


    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }

}