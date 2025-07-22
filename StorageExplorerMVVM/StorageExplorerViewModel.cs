using denSharedLibrary;
using StorageExplorerMVVM.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.Models;
using Printers;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace StorageExplorerMVVM;

public class StorageExplorerViewModel : ObservableObject
{

    public  Pozycja FindFreeSpot(multidrawer multidrawer)
    {
        var zwro = new Pozycja { X = 0, Y = 0 };

        var go = _databaseAccessLayer.items.Values.Where(q => q.bodyinthebox != null).Select(p => p.bodyinthebox);
        var bbbo = go.Where(p => p.MultiDrawerID == multidrawer.MultiDrawerID);


        while (zwro.Y <= multidrawer.rows && bbbo.Any(p => p.column == zwro.X && p.row == zwro.Y))
        {

            while (zwro.X < multidrawer.columns && bbbo.Any(p => p.column == zwro.X && p.row == zwro.Y))
            {
                zwro.X++;
            }
            if (zwro.X == multidrawer.columns)
            {
                zwro.Y++;
                zwro.X = 0;
            }
        }

        if (zwro.Y == multidrawer.rows)
        {
            zwro.X = -1;
            zwro.Y = -1;
        }




        return zwro;
    }

    private ObservableCollection<Szaviewitem> _multiDrawery;
    public ObservableCollection<Szaviewitem> MultiDrawery
    {
        get => _multiDrawery;
        set => SetProperty(ref _multiDrawery, value);
    }

    private string _informacja;
    public string Informacja
    {
        get => _informacja;
        set => SetProperty(ref _informacja, value);
    }

    private string _projSize;
    public string ProjSize
    {
        get => _projSize;
        set => SetProperty(ref _projSize, value);
    }

    private string _locateItemText;
    public string LocateItemText
    {
        get => _locateItemText;
        set => SetProperty(ref _locateItemText, value);
    }

    private bool _isCupboardSelected;
    public bool IsCupboardSelected
    {
        get => _isCupboardSelected;
        set
        {
            if (_isCupboardSelected != value)
            {
                _isCupboardSelected = value;
                OnPropertyChanged(nameof(IsCupboardSelected));
            }
        }
    }

    private Szaviewitem _selectedItem;
    public Szaviewitem SelectedItem
    {
        get => _selectedItem;
        set  {
            if (value==null)
            {
                SetProperty(ref _selectedItem, value);
                return;
            }
            _selectedItem = value;

            var md = _databaseAccessLayer.multidrawer().Result.First(p => p.MultiDrawerID == value.Id);
            var po = FindFreeSpot(md);
               

            OnPropertyChanged(nameof(SelectedItem));
            IsCupboardSelected = true;
            if (po.X == -1)
            {
                AddSeveralIsEnabled = false;
            }
            else
            {
                AddSeveralIsEnabled = true;
            }
        }
    }

      

    public ICommand MouseEnterCommand { get; set; }
    public IAsyncRelayCommand AddNewCCommand { get; }
    public AsyncRelayCommand RencupCommand { get; }
    public IAsyncRelayCommand AddSevCommand { get; }
    public IAsyncRelayCommand PrintcupCommand { get; }
      
    public IAsyncRelayCommand RemcupCommand { get; }
    public IAsyncRelayCommand AnulujCommand { get; }
    public IAsyncRelayCommand SzafeczkaMouseDownCommand { get; }
    public ICommand MultiDrawerySelectionChangedCommand { get; }
    public ICommand CancelDrawerCreationCommand { get; }
    public ICommand LoadDataCommand { get; }
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
    private readonly IDialogService _dialogService;

    public ObservableCollection<CellViewModel> Cells { get; }



    public void OnMouseEnter(int id)
    {
        var cell = Cells.FirstOrDefault(c => c.Id == id);
        if (cell == null)
        {
            return;
        }
          
        int targetRow = cell.RowIndex;
        int targetCol = cell.ColumnIndex;

        foreach (var celia in Cells)
        {
            if (celia.RowIndex <= targetRow && celia.ColumnIndex <= targetCol)
            {
                celia.BackgroundColor = System.Windows.Media.Brushes.Green;
            }
            else
            {
                celia.BackgroundColor = System.Windows.Media.Brushes.Transparent;
            }
        }

        ProjSize =  (cell.ColumnIndex + 1) + "x" + (cell.RowIndex + 1) + " compartments";
    }

    public async void StorageBoxMouseClicked(int id)
    {
    
        var cell = Cells.FirstOrDefault(c => c.Id == id);
        int targetRow = cell.RowIndex+1;
        int targetCol = cell.ColumnIndex+1;
        var forbiddenNames=(await _databaseAccessLayer.multidrawer()).Select(p=>p.name.ToLower()).ToList();
        var qvm = new ConfirmNameViewModel(_dialogService,forbiddenNames);
        qvm.RequestClose += async (sender, e) =>
        {
                
            var result = ((ConfirmNameViewModel)sender).Response;
            if (result)
            {
                   
                      
                var x = new multidrawer
                {
                    columns = targetCol,
                    rows = targetRow,
                    name = ((ConfirmNameViewModel)sender).Name
                };

                var szaf= await _databaseAccessLayer.AddMultiDrawer(x);

                MultiDrawery.Add(new Szaviewitem
                {
                    Rows=szaf.rows,
                    Columns=szaf.columns,
                    Name = szaf.name,
                    Dims = szaf.columns.ToString() + 'x' + szaf.rows,

                    Id = szaf.MultiDrawerID
                });

                  

                await _dialogService.ShowMessage("Info","New cupboard added succesfully");


                IsCupboardBeingCreated = false;
                Cells.Clear();
                return;
            }
            IsCupboardBeingCreated = false;
            Cells.Clear();
        };
        _dialogService.ShowDialog(qvm);


    }
    public IDatabaseAccessLayer _databaseAccessLayer;
    private IHttpClientFactory _httpClientFactory;
    private ISettingsService SettingsService;
    IPrintersService PrintersService;
    public StorageExplorerViewModel(IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer, IHttpClientFactory httpClientFactory,ISettingsService settingsService,IPrintersService printersService)
    {
        SettingsService = settingsService;
        PrintersService = printersService;
        _httpClientFactory = httpClientFactory;
        _dialogService = dialogService;
        _databaseAccessLayer = databaseAccessLayer;
        Cells = new ObservableCollection<CellViewModel>();
        IsCupboardBeingCreated = false;



        MultiDrawery = new ObservableCollection<Szaviewitem>();
           
        LocateItemText = "";
        IsCupboardSelected = false;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddNewCCommand = new AsyncRelayCommand(AddNewC);
        RencupCommand = new AsyncRelayCommand(Rencup);
        AddSevCommand = new AsyncRelayCommand(AddSev);
        PrintcupCommand = new AsyncRelayCommand(Printcup);
        
        RemcupCommand = new AsyncRelayCommand(Remcup);
        AnulujCommand = new AsyncRelayCommand(Anuluj);
        SzafeczkaMouseDownCommand = new AsyncRelayCommand(SzafeczkaMouseDown);
        CancelDrawerCreationCommand = new RelayCommand(CancelDrawerCreationExecute);
        MultiDrawerySelectionChangedCommand = new RelayCommand(SketchDrawerExecute);
               
        
    }

    public ObservableCollection<BodyInTheBoxViewModel> BoxViewModels { get; set; } = new ObservableCollection<BodyInTheBoxViewModel>();

    public void SketchDrawerExecute()
    {
        if (SelectedItem == null) return;
        var md = SelectedItem.Id;
        BoxViewModels.Clear();
        var bibs = _databaseAccessLayer.BodyInTheBoxes.Where(p => p.MultiDrawerID == md).ToList();
        for (int y = 0; y < SelectedItem.Rows; y++)
        {
            for (int x = 0; x < SelectedItem.Columns; x++)
            {
                bodyinthebox? bib = bibs.FirstOrDefault(p => p.row == y && p.column == x);
                if (bib != null)
                {
                    BoxViewModels.Add(new BodyInTheBoxViewModel(_dialogService,_databaseAccessLayer, y, x, SelectedItem.Id,SettingsService,PrintersService,bib));
                }
                else
                {
                    BoxViewModels.Add(new BodyInTheBoxViewModel(_dialogService, _databaseAccessLayer, y, x, SelectedItem.Id,SettingsService,PrintersService));
                }
            }
        }
    }

    public void CancelDrawerCreationExecute()
    {
        IsCupboardBeingCreated = false;
        Cells.Clear();
    }

    public class sza
    {
        public multidrawer szafka;
        public bodyinthebox[] predmy;
    }
    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;

        if (_databaseAccessLayer.DALState == DatabaseAccessLayerState.UnLoaded)
        {
            await _databaseAccessLayer.GetPackage(1);
        }

        if (_databaseAccessLayer.DALState == DatabaseAccessLayerState.Loading)
        {
            await Task.Run(() =>
            {
                while (_databaseAccessLayer.DALState!=DatabaseAccessLayerState.Loaded)
                {
                    Task.Delay(500); // Delay to avoid tight loop that hogs CPU
                }
            });
        }

        var warehouse = new List<sza>();
        MultiDrawery.Clear();
        foreach (var szaf in (await _databaseAccessLayer.multidrawer()))
        {
            var sz = new sza();
            sz.szafka = szaf;
            var pr = _databaseAccessLayer.BodyInTheBoxes.Where(p => p.MultiDrawerID == szaf.MultiDrawerID).ToArray();
            sz.predmy = pr;
            warehouse.Add(sz);
            MultiDrawery.Add(new Szaviewitem
            {
                Name = szaf.name,
                Dims = szaf.columns.ToString() + 'x' + szaf.rows,
                Rows = szaf.rows,
                Columns=szaf.columns,
                Id = szaf.MultiDrawerID
            });
        }

        IsDataLoaded = true;
    }


    // Metody odpowiadające za komendy
    private async Task MultidrawerySelectionChanged()
    {
        // ... implementacja metody
    }

    private bool _isCupboardBeingCreated;
    public bool IsCupboardBeingCreated
    {
        get => _isCupboardBeingCreated;
        set { 
            SetProperty(ref _isCupboardBeingCreated, value); 
            OnPropertyChanged(nameof(NotIsCupboardBeingCreated));
        }
            
    }

    public bool NotIsCupboardBeingCreated
    {
        get => !_isCupboardBeingCreated;
          
    }
    private bool _addSeveralIsEnabled;
    public bool AddSeveralIsEnabled
    {
        get => _addSeveralIsEnabled && IsCupboardSelected;
        set => SetProperty(ref _addSeveralIsEnabled, value);
    }



    private async Task AddNewC()
    {
        while (!IsDataLoaded)
        {
            await  Task.Delay(500);
        }
        Cells.Clear();
        SelectedItem = null;
        IsCupboardSelected = false;

        int i = 0;
        for (int y = 0; y < 30; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                var cellViewModel = new CellViewModel(i, OnMouseEnter, StorageBoxMouseClicked)
                {
                    RowIndex = y,
                    ColumnIndex = x
                };

                Cells.Add(cellViewModel);
                i++;
            }
        }
        Informacja = "Move the mouse cursor over the grid below to set the required size of the container. Click the left mouse button when the green fields match the size of the container.";
        ProjSize = "";
        IsCupboardBeingCreated = true;
    }

    private async Task Rencup()
    {
        var name = SelectedItem.Name;


        var forbiddenNames = (await _databaseAccessLayer.multidrawer()).Select(p => p.name.ToLower()).ToList();
        var qvm = new ConfirmNameViewModel(_dialogService, forbiddenNames, name);
        qvm.RequestClose += async (sender, e) =>
        {

            var result = ((ConfirmNameViewModel)sender).Response;
            if (result)
            {




                var szaf = await _databaseAccessLayer.RenameDrawer(SelectedItem.Id, ((ConfirmNameViewModel)sender).Name);

                MultiDrawery.First(p => p.Id == SelectedItem.Id).Name = szaf.name;



            }

        };
        await _dialogService.ShowDialog(qvm);

    }

    private async Task AddSev()
    {
         
        var qvm = new AddSeveralViewModel(_dialogService,_databaseAccessLayer, (await _databaseAccessLayer.multidrawer()).First(p=>p.MultiDrawerID==SelectedItem.Id),_httpClientFactory,PrintersService,SettingsService);
        qvm.BodyInTheBoxAdded += BBAdded;
        qvm.RequestClose += async (sender, e) =>
        {
            qvm.BodyInTheBoxAdded -= BBAdded;
        };

            
        _dialogService.ShowDialog(qvm);
    }

    public void BBAdded(int bbid)
    {
        var bodyinthebox=_databaseAccessLayer.BodyInTheBoxes.First(p=>p.BodyInTheBoxID==bbid);
        var szafeczka = BoxViewModels.First(p => p.Row == bodyinthebox.row && p.Column == bodyinthebox.column);
        szafeczka.Body = bodyinthebox;
    }


    private async Task Printcup()
    {
        // ... implementacja metody
    }


    private async Task Remcup()
    {
        var name=MultiDrawery.First(p=>p.Id==SelectedItem?.Id).Name;
        bool userConfirmed =await _dialogService.ShowYesNoMessageBox("Confirm removal of the cupboard", "Are you sure you want to remove " + name + '?');
         
        if (userConfirmed)
        {

            await _databaseAccessLayer. RemoveDrawer(SelectedItem.Id);
            var sza = MultiDrawery.First(p => p.Id == SelectedItem.Id);
            MultiDrawery.Remove(sza);
        }
    }

    private async Task Anuluj()
    {
        // ... implementacja metody
    }

    private async Task SzafeczkaMouseDown()
    {
        // ... implementacja metody
    }

}