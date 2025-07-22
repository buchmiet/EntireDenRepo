using denSharedLibrary;
using MayAlsoFitMVVM.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.Models;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace MayAlsoFitMVVM;

public class MayAlsoFitViewModel : ObservableObject, INotifyCollectionChanged
{
    private ObservableCollection<Wyswietlacz> _listaBodies;
    public ObservableCollection<Wyswietlacz> ListaBodies
    {
        get { return _listaBodies; }
        set { SetProperty(ref _listaBodies, value); }
    }


    private bool _listaBodiesItemSelectedAndMore = false;
    public bool ListaBodiesItemSelectedAndMore
    {
        get => _listaBodiesItemSelected && GrupaczesciItemSelected;
    }


    private bool _listaZegowItemSelectedAndMore = false;
    public bool ListaZegowItemSelectedAndMore
    {
        get => _listaZegowItemSelected && DostepneGrupyZegowItemSelected;
    }

    private bool _listaBodiesItemSelected = false;
    public bool ListaBodiesItemSelected
    {
        get => _listaBodiesItemSelected;
        set
        {
            if (SetProperty(ref _listaBodiesItemSelected, value))
            {
                // Podnieś zdarzenie dla właściwości zależnej
                OnPropertyChanged(nameof(ListaBodiesItemSelectedAndMore));
            }

        }
    }


    private bool _grupaczesciItemSelected = false;
    public bool GrupaczesciItemSelected
    {
        get => _grupaczesciItemSelected;
        set
        {
            if (SetProperty(ref _grupaczesciItemSelected, value))
            {
                // Podnieś zdarzenie dla właściwości zależnej
                OnPropertyChanged(nameof(ListaBodiesItemSelectedAndMore));
            }
        }
    }

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FilterGroups();
        }
    }

    private ObservableCollection<IdNameOO> _allGroups;
    private void FilterGroups()
    {
        if (_allGroups == null)
        {
            _allGroups = new ObservableCollection<IdNameOO>(DostepneGrupy);
        }

        DostepneGrupy.Clear();

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var group in _allGroups)
            {
                DostepneGrupy.Add(group);
            }
        }
        else
        {
            var filteredGroups = _allGroups.Where(g => g.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var group in filteredGroups)
            {
                DostepneGrupy.Add(group);
            }
        }
    }

    private ObservableCollection<IdNameOO> _dostepneGrupy;
    public ObservableCollection<IdNameOO> DostepneGrupy
    {
        get { return _dostepneGrupy; }
        set { SetProperty(ref _dostepneGrupy, value); }
    }

    private ObservableCollection<Wyswietlacz> _bodiesInGroup;
    public ObservableCollection<Wyswietlacz> BodiesInGroup
    {
        get { return _bodiesInGroup; }
        set { SetProperty(ref _bodiesInGroup, value); }
    }

    private bool _bodiesInTheGroupItemSelected = false;
    public bool BodiesInTheGroupItemSelected
    {
        get => _bodiesInTheGroupItemSelected;
        set { SetProperty(ref _bodiesInTheGroupItemSelected, value); }
    }


    private Wyswietlacz _bodiesInTheGroupSelectedItem;
    public Wyswietlacz BodiesInTheGroupSelectedItem
    {
        get => _bodiesInTheGroupSelectedItem;
        set
        {
            SetProperty(ref _bodiesInTheGroupSelectedItem, value);
        }
    }

    private ObservableCollection<Zwiazkitemplate> _dostepneGrupyLast;
    public ObservableCollection<Zwiazkitemplate> DostepneGrupyLast
    {
        get { return _dostepneGrupyLast; }
        set { SetProperty(ref _dostepneGrupyLast, value); }
    }

    private Zwiazkitemplate _dostepneGrupyLastSelectedItem;
    public Zwiazkitemplate DostepneGrupyLastSelectedItem
    {
        get { return _dostepneGrupyLastSelectedItem; }
        set { SetProperty(ref _dostepneGrupyLastSelectedItem, value); }
    }

    private ObservableCollection<IdNameOO> _listaZegow;
    public ObservableCollection<IdNameOO> ListaZegow
    {
        get { return _listaZegow; }
        set
        {
            _listaZegow = value;
            NotifyPropertyChanged(nameof(ListaZegow));
        }
    }

    private bool _listaZegowItemSelected;
    public bool ListaZegowItemSelected
    {
        get => _listaZegowItemSelected;
        set
        {
            if (SetProperty(ref _listaZegowItemSelected, value))
            {
                OnPropertyChanged(nameof(ListaZegowItemSelectedAndMore));
            }
        }
    }

    private IdNameOO _listaZegowSelectedItem;
    public IdNameOO ListaZegowSelectedItem
    {
        get { return _listaZegowSelectedItem; }
        set
        {
            SetProperty(ref _listaZegowSelectedItem, value);
            if (value != null)
            {
                var zeczek = _databaseAccessLayer.Watches().Result  .FirstOrDefault(p => p.watchID == value.Id);
                if (zeczek != null)
                {
                    string fileName;
                    if (zeczek.haspic)
                    {
                        fileName = @"c:\mucpacs\watches\" + zeczek.name.ToLower() + @".jpg";
                        if (!File.Exists(fileName))
                        {
                            fileName = Environment.CurrentDirectory + @"\Data\np.png";
                        }
                    }
                    else
                    {
                        fileName = Environment.CurrentDirectory + @"\Data\np.png";
                    }
                    Zegareczek = new BitmapImage(new Uri(fileName));
                }



                ListaZegowItemSelected = true;
            }
            else
            {
                ListaZegowItemSelected = false;
            }
        }
    }

    private BitmapImage _zegareczek;
    public BitmapImage Zegareczek
    {
        get => _zegareczek;
        set => SetProperty(ref _zegareczek, value);
    }

    private ObservableCollection<IdNameOO> _dostepneGrupyZegow;
    public ObservableCollection<IdNameOO> DostepneGrupyZegow
    {
        get { return _dostepneGrupyZegow; }
        set
        {
            _dostepneGrupyZegow = value;
            NotifyPropertyChanged(nameof(DostepneGrupyZegow));
        }
    }

    private bool _dostepneGrupyZegowItemSelected = false;
    public bool DostepneGrupyZegowItemSelected
    {
        get => _dostepneGrupyZegowItemSelected;
        set
        {
            if (SetProperty(ref _dostepneGrupyZegowItemSelected, value))
            {
                OnPropertyChanged(nameof(ListaZegowItemSelectedAndMore));
            }
        }
    }


    public async Task PopulateWatchesInTheGroup()
    {
        WatchesInTheGroup.Clear();
        foreach (var ox in (await _databaseAccessLayer.watchesgrouped()).Where(p => p.group4watchesID == DostepneGrupyZegowSelectedItem.Id))
        {
            var xid = ox.watchID;
            var xname = (await _databaseAccessLayer.Watches()).First(p => p.watchID == ox.watchID).name;
            var exx =
                new IdNameOO
                {
                    Id = xid,
                    Name = xname,

                };
            WatchesInTheGroup.Add(exx);

        }
    }


    private IdNameOO _dostepneGrupyZegowSelectedItem;
    public IdNameOO DostepneGrupyZegowSelectedItem
    {
        get => _dostepneGrupyZegowSelectedItem;
        set
        {
            SetProperty(ref _dostepneGrupyZegowSelectedItem, value);

            if (value != null)
            {
                DostepneGrupyZegowItemSelected = true;
                WatchesGroupName = value.Name;
                PopulateWatchesInTheGroup().Wait();
            }
            else
            {
                DostepneGrupyZegowItemSelected = false;
                WatchesGroupName = "";
                WatchesInTheGroup.Clear();
            }

        }
    }


    private ObservableCollection<IdNameOO> _watchesinthegroup;
    public ObservableCollection<IdNameOO> WatchesInTheGroup
    {
        get { return _watchesinthegroup; }
        set
        {
            _watchesinthegroup = value;
            NotifyPropertyChanged(nameof(WatchesInTheGroup));
        }
    }

    private ObservableCollection<IdNameOO> _dostepneGrupyZegowLast;
    public ObservableCollection<IdNameOO> DostepneGrupyZegowLast
    {
        get { return _dostepneGrupyZegowLast; }
        set
        {
            _dostepneGrupyZegowLast = value;
            NotifyPropertyChanged(nameof(DostepneGrupyZegowLast));
        }
    }

    private IdNameOO _dostepneGrupyZegowLastSelectedItem;
    public IdNameOO DostepneGrupyZegowLastSelectedItem
    {
        get { return _dostepneGrupyZegowLastSelectedItem; }
        set
        {
            SetProperty(ref _dostepneGrupyZegowLastSelectedItem, value);

        }
    }


    private string _wsterm;
    public string WSterm
    {
        get => _wsterm;
        set
        {
            SetProperty(ref _wsterm, value);
            FilterWatches();
        }
    }

    private string _watchesgroupname;
    public string WatchesGroupName
    {
        get => _watchesgroupname;
        set
        {
            SetProperty(ref _watchesgroupname, value);

        }
    }


    private ObservableCollection<IdNameOO> _allWatches;
    private void FilterWatches()
    {
        if (_allWatches == null)
        {
            _allWatches = new ObservableCollection<IdNameOO>(ListaZegow);
        }

        ListaZegow.Clear();

        if (string.IsNullOrWhiteSpace(WSterm))
        {
            foreach (var zeg in _allWatches)
            {
                ListaZegow.Add(zeg);
            }
        }
        else
        {
            var filteredWatches = _allWatches.Where(g => g.Name.Contains(WSterm, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var group in filteredWatches)
            {
                ListaZegow.Add(group);
            }
        }
    }
    private string _mpnSearch;
    public string MpnSearch
    {
        get { return _mpnSearch; }
        set
        {
            _mpnSearch = value;
            NotifyPropertyChanged(nameof(MpnSearch));
        }
    }

    private string _nameSearch;
    public string NameSearch
    {
        get { return _nameSearch; }
        set
        {
            _nameSearch = value;
            NotifyPropertyChanged(nameof(NameSearch));
        }
    }



    public event NotifyCollectionChangedEventHandler? CollectionChanged;


    public RelayCommand<IList> SelectionChangedCommand => new RelayCommand<IList>(OnSelectionChanged);

    private ObservableCollection<Wyswietlacz> _selectedBodies = new ObservableCollection<Wyswietlacz>();

    public ObservableCollection<Wyswietlacz> SelectedBodies
    {
        get => _selectedBodies;
        set
        {
            _selectedBodies = value;
            NotifyPropertyChanged(nameof(SelectedBodies));
        }
    }




    private void OnSelectionChanged(IList items)
    {
        if (items == null)
        {
            ListaBodiesItemSelected = true;

            return;
        }
        ListaBodiesItemSelected = true;
        SelectedBodies.Clear();

        foreach (var item in items)
        {
            SelectedBodies.Add(item as Wyswietlacz);
        }
    }


    public AsyncRelayCommand GroupaZegowSelectedCommand { get; set; }
    public AsyncRelayCommand Add2GroupOfWatchesCommand { get; private set; }
    public AsyncRelayCommand CreateGroupOfWatchesCommand { get; private set; }
    public AsyncRelayCommand SaveWatchesNameCommand { get; private set; }
    public AsyncRelayCommand DeleteWatchesGroupCommand { get; private set; }
    public AsyncRelayCommand<IdNameOO> RemoveFromGroupCommand { get; private set; }
    public RelayCommand SearchBodyByMPNCommand { get; private set; }
    public RelayCommand SearchBodyByNameCommand { get; private set; }
    public RelayCommand ClearBodiesCommand { get; private set; }
    public RelayCommand ListaBodiesSelectionChangedCommand { get; private set; }
    public AsyncRelayCommand Add2BgroupCommand { get; private set; }
    public AsyncRelayCommand CreateNewBodyGroupCommand { get; private set; }
    public RelayCommand ExtractWatchesCommand { get; private set; }
    public RelayCommand DostepneGrupySelectionChangedCommand { get; private set; }
    public AsyncRelayCommand SavebodiesnameCommand { get; private set; }
    public AsyncRelayCommand DeletebodiesgroupCommand { get; private set; }
    public RelayCommand ExtractWatches4GroupCommand { get; private set; }
    public RelayCommand ButtonBase2Command { get; private set; }
    public RelayCommand ButtonBaseOnClick4Command { get; private set; }
    public RelayCommand ButtonBaseOnClick5Command { get; private set; }
    public ICommand LoadDataCommand { get; }
    public AsyncRelayCommand<Wyswietlacz> RemoveBodyCommand { get; private set; }




    public IDatabaseAccessLayer _databaseAccessLayer;
    public int locationID;
    public IDialogService _dialogService;
    public MayAlsoFitViewModel(IDatabaseAccessLayer databaseAccessLayer, IDialogService dialogService,ISettingsService settingsService)
    {
        _databaseAccessLayer = databaseAccessLayer;
        _dialogService = dialogService;
        BodiesInGroup = new ObservableCollection<Wyswietlacz>();
        WatchesInTheGroup = new ObservableCollection<IdNameOO>();
        DostepneGrupyZegow = new ObservableCollection<IdNameOO>();
        DostepneGrupyZegowLast = new ObservableCollection<IdNameOO>();
        ListaBodies = new();
        ListaZegow = new();
        locationID = settingsService.LocationId;

        SearchBodyByMPNCommand = new RelayCommand(ExecuteSearchBodyByMPN);
        SearchBodyByNameCommand = new RelayCommand(ExecuteSearchBodyByName);
        ClearBodiesCommand = new RelayCommand(ExecuteClearBodies);
        ListaBodiesSelectionChangedCommand = new RelayCommand(ExecuteListaBodiesSelectionChanged);
        Add2BgroupCommand = new AsyncRelayCommand(ExecuteAdd2Bgroup);
        CreateNewBodyGroupCommand = new AsyncRelayCommand(ExecuteCreateNewBodyGroup);
        SavebodiesnameCommand = new AsyncRelayCommand(ExecuteSavebodiesname);
        DeletebodiesgroupCommand = new AsyncRelayCommand(ExecuteDeletebodiesgroup);
        Add2GroupOfWatchesCommand = new AsyncRelayCommand(ExecuteAdd2GroupOfWatches);
        CreateGroupOfWatchesCommand = new AsyncRelayCommand(ExecuteCreateGroupOfWatches);
        SaveWatchesNameCommand = new AsyncRelayCommand(ExecuteSaveWatchesName);
        DeleteWatchesGroupCommand = new AsyncRelayCommand(ExecuteDeleteGroup4Watches);
        RemoveFromGroupCommand = new AsyncRelayCommand<IdNameOO>(ExecuteRemoveFromGroup);
        RemoveBodyCommand = new AsyncRelayCommand<Wyswietlacz>(RemoveItem);
        GroupaZegowSelectedCommand = new AsyncRelayCommand(PopulateLastBodies);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    public async Task RemoveItem(Wyswietlacz item)
    {
        int id = SelectedGrupaCzesci.Id;
        await _databaseAccessLayer.RemoveBodyFromGroup(id, item.id);
        BodiesInGroup.Remove(item);
    }

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set
        {
            _isDataLoaded = value;
            NotifyPropertyChanged(nameof(IsDataLoaded));
        }

    }






    private bool _isDataLoaded = false;
    private async Task LoadDataAsync()
    {

        if (IsDataLoaded) return;
        await _databaseAccessLayer.GetPackage(1);
            
        GetALlBodiesToList();
        await GetALlWatchesToList();

        await PopulateGroupsOfBodies(-1);
        await PopulateGroupsOfWatches(-1);

        IsDataLoaded = true;



    }




    private async Task PopulateGroupsOfWatches(int id)
    {


        var stringa = new List<IdNameOO>();
        List<group4watch> grupyCialek;
        List<watchesgrouped> cialkaWgrupach;


        foreach (var x1 in (await _databaseAccessLayer.Group4Watches()))
        {
            var trt = (await _databaseAccessLayer.watchesgrouped()).Where(p => p.group4watchesID == x1.group4watchesID).ToList();

            var sztr = "";
            if (!String.IsNullOrEmpty(x1.name))
            {
                sztr = x1.name;
            }
            else
            {
                sztr = "no name";
            }

            stringa.Add(new IdNameOO
            {
                Id = x1.group4watchesID,
                Name = sztr
            });

        }
        DostepneGrupyZegow.Clear();
        DostepneGrupyZegowLast.Clear();
        foreach (var item in stringa.OrderBy(p => p.Name))
        {
            DostepneGrupyZegow.Add(item);
            DostepneGrupyZegowLast.Add(item);
        }






        if (stringa.Count > 0)
        {

            if (id != -1)
            {
                DostepneGrupyZegowSelectedItem = DostepneGrupyZegow.First(p => p.Id == id);
            }


        }


        await RefreshWatchesinGroup();
    }

    private async Task RefreshWatchesinGroup()
    {
        if (DostepneGrupyZegowSelectedItem == null)
        {
            WatchesInTheGroup.Clear();
            return;
        }

        var pr = DostepneGrupyZegowSelectedItem;
        var stringa = new List<IdNameOO>();
        //  group4watch o = groupsOfwatches.Keys.Single(p => p.group4watchesID == pr.Id);
        foreach (var ox in (await _databaseAccessLayer.watchesgrouped()).Where(p => p.group4watchesID == DostepneGrupyZegowSelectedItem.Id))//    groupsOfwatches[o])
        {
            var xid = ox.watchID;
            var xbody = (await _databaseAccessLayer.Watches()).First(p => p.watchID == ox.watchID);
            var xname = xbody.name;
            var exx =
                new IdNameOO
                {
                    Id = xid,
                    Name = xname,

                };
            stringa.Add(exx);
        }
        WatchesInTheGroup.Clear();
        foreach (var item in stringa.OrderBy(p => p.Name))
        {
            WatchesInTheGroup.Add(item);
        }

        // WatchesInTheGroup = new ObservableCollection<Watchwyswietlacz>(stringa);


    }

    private string _bodiesgroupname;
    public string Bodiesgroupname
    {
        get => _bodiesgroupname;

        set { SetProperty(ref _bodiesgroupname, value); }

    }



    private IdNameOO _selectedGrupaCzesci;
    public IdNameOO SelectedGrupaCzesci
    {
        get => _selectedGrupaCzesci;
        set
        {
            _selectedGrupaCzesci = value;

            OnPropertyChanged(nameof(SelectedGrupaCzesci));
            if (value == null)
            {
                BodiesInGroup.Clear();
                GrupaczesciItemSelected = false;
                return;
            }
            BodiesInGroup.Clear();
            var stringa = new List<Wyswietlacz>();
            //   group4body o = groupsOfBodies.Keys.Single(p => p.group4bodiesID == SelectedGrupaCzesci.Id);
            foreach (var ox in _databaseAccessLayer.bodiesgrouped().Result .Where(p => p.group4bodiesID == SelectedGrupaCzesci.Id))//  groupsOfBodies[o])
            {
                var xid = ox.itemBodyID;
                var xbody = _databaseAccessLayer.items.Values.Select(p => p.itembody).First(p => p.itembodyID == ox.itemBodyID);
                var xname = xbody.myname;
                var exx =
                    new Wyswietlacz
                    {
                        id = xid,
                        name = xname,
                        mpn = xbody.mpn
                    };
                //   stringa.Add(exx);
                BodiesInGroup.Add(exx);

            }

            Bodiesgroupname = SelectedGrupaCzesci.Name;

            GrupaczesciItemSelected = true;
        }
    }


    // public Dictionary<group4body, List<bodiesGrouped>> groupsOfBodies;



    private async Task PopulateGroupsOfBodies(int id)
    {
        int id2 = -1;
        if (SelectedGrupaCzesci != null)
        {
            id2 = SelectedGrupaCzesci.Id;
        }


        //  groupsOfBodies = new Dictionary<group4body, List<bodiesGrouped>>();
        var stringa = new List<IdNameOO>();

        foreach (var x1 in (await _databaseAccessLayer.Group4Bodies()))
        {
            var trt = (await _databaseAccessLayer.bodiesgrouped()).Where(p => p.group4bodiesID == x1.group4bodiesID).ToList();
            //  groupsOfBodies.Add(x1,trt);
            var sztr = "";
            if (!String.IsNullOrEmpty(x1.name))
            {
                sztr = x1.name;
            }
            else
            {
                sztr = "no name";
            }
            stringa.Add(new IdNameOO
            {
                Id = x1.group4bodiesID,
                Name = sztr
            });

        }



        DostepneGrupy = new ObservableCollection<IdNameOO>(stringa.OrderBy(p => p.Name));

        if (stringa.Count > 0)
        {
            //savebodiesname.IsEnabled = true;
            //extractWatches4Group.IsEnabled = true;
            //deletebodiesgroup.IsEnabled = true;
            int ind = 0;
            if (id != -1)
            {
                ind = id;
            }
            else
            {
                if (id2 != -1)
                {
                    ind = id2;
                }
            }

            if (ind == 0 && id == -1)
            {
                BodiesInGroup.Clear();
            }
            else
            {
                SelectedGrupaCzesci = DostepneGrupy.First(p => p.Id == ind);
            }
        }

        await RefreshBodiesinGroup();
        // await PopulateLastBodies();
    }

    //public async Task PopulateLastBodies()
    //{
    //    mayalsofit[] zwiazki;// = mayalsofit[];
    //    if (DostepneGrupyZegowLastSelectedItem == null)
    //    {
    //        return;
    //    }

    //    group4body[] ej;

    //    ej = _databaseAccessLayer.Group4Bodies.ToArray();
    //    var ux = new List<Zwiazkitemplate>();

    //    for (int i = 0; i < ej.Length; i++)
    //    {
    //        var zwiazkiDobre = _databaseAccessLayer.mayalsofits.Where(p => p.group4watchesID == DostepneGrupyZegowLastSelectedItem.Id);
    //        var oj = new Zwiazkitemplate
    //        {
    //            id = ej[i].group4bodiesID,

    //        };
    //        var hej = ej[i];
    //        if (string.IsNullOrEmpty(hej.name))
    //        {
    //            oj.Name = "no name";
    //        }
    //        else
    //        {
    //            oj.Name = hej.name;
    //        }

    //        if (zwiazkiDobre.All(p => p.group4bodiesID != ej[i].group4bodiesID))
    //        {
    //            oj.AssignButton = true;
    //            oj.RemoveButton = false;
    //        }
    //        else
    //        {
    //            oj.AssignButton = false;
    //            oj.RemoveButton = true;
    //        }


    //        ux.Add(oj);
    //    }

    //  DostepneGrupyLast =new ObservableCollection<Zwiazkitemplate>( ux.OrderBy(p => p.Name));
    //}

    private async Task RefreshBodiesinGroup()
    {
        if (SelectedGrupaCzesci == null)
        {
            BodiesInGroup.Clear();
            return;
        }

        var pr = SelectedGrupaCzesci;
        var stringa = new List<Wyswietlacz>();
        // group4body o = groupsOfBodies.Keys.Single(p => p.group4bodiesID == pr.Id);
        foreach (var ox in (await _databaseAccessLayer.bodiesgrouped()).Where(p => p.group4bodiesID == SelectedGrupaCzesci.Id))//  groupsOfBodies[o])
        {
            var xid = ox.itemBodyID;
            var xbody = _databaseAccessLayer.items.Values.First(p => p.itembody.itembodyID == ox.itemBodyID).itembody;
            var xname = xbody.myname;
            var exx =
                new Wyswietlacz
                {
                    id = xid,
                    name = xname,
                    mpn = xbody.mpn
                };
            stringa.Add(exx);
        }
        BodiesInGroup = new ObservableCollection<Wyswietlacz>(stringa.OrderBy(p => p.name));
    }


    public void GetALlBodiesToList()
    {

        foreach (var body in _databaseAccessLayer.items.Values.Select(p => p.itembody))
        {

            ListaBodies.Add(new Wyswietlacz
            {
                id = body.itembodyID,
                name = body.name,
                mpn = body.mpn,
            });

        }
    }

    public async Task GetALlWatchesToList()
    {
        foreach (var wa in (await _databaseAccessLayer.Watches()))
        {
            ListaZegow.Add(new IdNameOO
            {
                Id = wa.watchID,
                Name = wa.name
            });
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    private async Task ExecuteAdd2GroupOfWatches()
    {
        var zeg2Add = ListaZegowSelectedItem.Id;
        if (!(await _databaseAccessLayer.watchesgrouped()).Any(p => p.watchID == zeg2Add && p.group4watchesID == DostepneGrupyZegowSelectedItem.Id))
        {

            await _databaseAccessLayer.AddWatch2Group(zeg2Add, DostepneGrupyZegowSelectedItem.Id);



            await PopulateWatchesInTheGroup();





        }
        else
        {
            await _dialogService.ShowMessage("Information", "Watch " + ListaZegowSelectedItem.Name + " is already in group " + DostepneGrupyZegowSelectedItem.Name);
        }


    }
    public ObservableCollection<WatchesAndBodiesGroupsViewModel> AvailableGroups { get; set; } = new ObservableCollection<WatchesAndBodiesGroupsViewModel>();


    private WatchesAndBodiesGroupsViewModel _selectedGroup4BodiesLast;

    public WatchesAndBodiesGroupsViewModel SelectedGroup4BodiesLast
    {
        get => _selectedGroup4BodiesLast;

        set { SetProperty(ref _selectedGroup4BodiesLast, value); }

    }


      


    public async Task PopulateLastBodies()
    {
        mayalsofit[] zwiazki;
        if (DostepneGrupyZegowLastSelectedItem == null)
        {
            return;
        }



        var ux = new ObservableCollection<WatchesAndBodiesGroupsViewModel>();


        var zwiazkiDobre = (await _databaseAccessLayer.mayalsofits()).Where(p => p.group4watchesID == DostepneGrupyZegowLastSelectedItem.Id);
        HashSet<int> zwiazkiDobreIds = new HashSet<int>(zwiazkiDobre.Select(p => p.group4bodiesID));
        foreach (var gru in (await _databaseAccessLayer.Group4Bodies()))
        {
            var isPresent = zwiazkiDobreIds.Contains(gru.group4bodiesID);
            var viewModel = new WatchesAndBodiesGroupsViewModel
            {
                Id = gru.group4bodiesID,
                Name = string.IsNullOrEmpty(gru.name) ? "no name" : gru.name,
                //AssignButton = zwiazkiDobre.All(p => p.group4bodiesID != gru.group4bodiesID),
                //RemoveButton = !zwiazkiDobre.All(p => p.group4bodiesID != gru.group4bodiesID)
                AssignButton = !isPresent,
                RemoveButton = isPresent
            };
            viewModel.AssignCommand = new AsyncRelayCommand(async () =>
            {
                var maf = await _databaseAccessLayer.AddMayAlsoFit(viewModel.Id, DostepneGrupyZegowLastSelectedItem.Id);
                //await PopulateLastBodies();
                //SelectedGroup4BodiesLast = AvailableGroups.First(p=>p.Id==maf.group4bodiesID);
                viewModel.AssignButton = !viewModel.AssignButton;
                viewModel.RemoveButton = !viewModel.RemoveButton;

            });
            viewModel.RemoveCommand = new AsyncRelayCommand(async () =>
            {
                await _databaseAccessLayer.RemoveMayAlsoFit(viewModel.Id, DostepneGrupyZegowLastSelectedItem.Id);
                //   await PopulateLastBodies();
                //  SelectedGroup4BodiesLast = AvailableGroups.First(p => p.Id == viewModel.Id);
                viewModel.AssignButton = !viewModel.AssignButton;
                viewModel.RemoveButton = !viewModel.RemoveButton;
            });


            ux.Add(viewModel);

        }


        AvailableGroups.Clear();
        foreach (var item in ux.OrderBy(p => p.Name))
        {
            AvailableGroups.Add(item);
        }
    }


    private async Task ExecuteCreateGroupOfWatches()
    {
        var zeg2Add = ListaZegowSelectedItem.Id;
        List<string> forbiddenNames = (await _databaseAccessLayer.Group4Watches()).Select(p => p.name.ToLower()).ToList();
        var qvm = new ConfirmNameViewModel(_dialogService, forbiddenNames);
        qvm.RequestClose += async (sender, e) =>
        {

            var result = ((ConfirmNameViewModel)sender).Response;
            if (result)
            {
                var name = ((ConfirmNameViewModel)sender).Name;
                var g4w = await _databaseAccessLayer.AddGroup4watch(name);
                await _databaseAccessLayer.AddWatch2Group(zeg2Add, g4w.group4watchesID);


                var task = PopulateGroupsOfWatches(g4w.group4watchesID);



                await _dialogService.ShowMessage("Info", "New group added succesfully");


                await task;

            }

        };
        await _dialogService.ShowDialog(qvm);
    }

    private async Task ExecuteSaveWatchesName()
    {
        if (string.IsNullOrEmpty(WatchesGroupName))
        {
            return;
        }
        if ((await _databaseAccessLayer.Group4Watches()).Select(p => p.name.ToLower()).Contains(WatchesGroupName.ToLower().Trim()))
        {
            await _dialogService.ShowMessage("Name is already in use", "Name \"" + WatchesGroupName + "\" is already in use, please use different name");
            return;
        }
        await _databaseAccessLayer.RenameGroup4Watches(DostepneGrupyZegowSelectedItem.Id, WatchesGroupName);
        var item = DostepneGrupyZegow.FirstOrDefault(x => x.Id == DostepneGrupyZegowSelectedItem.Id);
        if (item != null)
        {
            item.Name = WatchesGroupName;
            int index = DostepneGrupyZegow.IndexOf(item);
            DostepneGrupyZegow[index] = item;
        }
    }

    private async Task ExecuteDeleteGroup4Watches()
    {
        var wg = (await _databaseAccessLayer.Group4Watches()).First(p => p.name.Equals(DostepneGrupyZegowSelectedItem.Name));
        var confirmed =await _dialogService.ShowYesNoMessageBox("Confirm deletion", "Do you want to proceed with deleting group " + wg.name);
        if (confirmed)
        {
            await _databaseAccessLayer.RemoveWatchesGrouped(DostepneGrupyZegowSelectedItem.Id);
        }
        var item = DostepneGrupyZegow.FirstOrDefault(x => x.Id == wg.group4watchesID);
        if (item != null)
        {
            int index = DostepneGrupyZegow.IndexOf(item);
            DostepneGrupyZegow.RemoveAt(index);

        }
        var item2 = DostepneGrupyZegowLast.FirstOrDefault(x => x.Id == wg.group4watchesID);
        if (item2 != null)
        {
            int index = DostepneGrupyZegowLast.IndexOf(item2);
            DostepneGrupyZegowLast.RemoveAt(index);

        }
        DostepneGrupyZegowSelectedItem = null;
    }

    private async Task ExecuteRemoveFromGroup(IdNameOO item)
    {
        int id = DostepneGrupyZegowSelectedItem.Id;
        await _databaseAccessLayer.RemoveWatchFromGroup(id, item.Id);
        WatchesInTheGroup.Remove(item);
        await PopulateWatchesInTheGroup();
    }



    private void ExecuteSearchBodyByMPN()
    {
        ListaBodies.Clear();
        foreach (var bod in _databaseAccessLayer.items.Values.Select(p => p.itembody))
        {
            if (bod.mpn.Contains(MpnSearch))
            {
                ListaBodies.Add(new Wyswietlacz
                {
                    id = bod.itembodyID,
                    name = bod.name,
                    mpn = bod.mpn,
                });
            }
        }
    }

    private void ExecuteSearchBodyByName()
    {
        ListaBodies.Clear();
        foreach (var bod in _databaseAccessLayer.items.Values.Select(p => p.itembody))
        {
            if (bod.name.ToLower().Contains(NameSearch.ToLower()) || bod.myname.ToLower().Contains(NameSearch.ToLower()))
            {
                ListaBodies.Add(new Wyswietlacz
                {
                    id = bod.itembodyID,
                    name = bod.name,
                    mpn = bod.mpn,
                });
            }
        }
    }

    private void ExecuteClearBodies()
    {
        ListaBodies.Clear();
        foreach (var body in _databaseAccessLayer.items.Values.Select(p => p.itembody))
        {

            ListaBodies.Add(new Wyswietlacz
            {
                id = body.itembodyID,
                name = body.name,
                mpn = body.mpn,
            });

        }
    }

    private void ExecuteListaBodiesSelectionChanged()
    {
        // Logika dla ListaBodiesSelectionChanged
    }

    private async Task ExecuteAdd2Bgroup()
    {
        var bodiesToAdd = SelectedBodies.Select(p => p.id).ToList();
        var istniejaceCialkaIDs = (await _databaseAccessLayer.bodiesgrouped()).Where(p => p.group4bodiesID == SelectedGrupaCzesci.Id).Select(p => p.itemBodyID).ToList();

        // Kopiowanie zawartości bodiesToAdd do removedBodies
        var removedBodies = new List<int>(bodiesToAdd);

        // Usuwanie identyfikatorów, które nie są obecne w istniejaceCialkaIDs
        bodiesToAdd.RemoveAll(id => istniejaceCialkaIDs.Contains(id));

        // Usuwanie identyfikatorów, które zostały w bodiesToAdd z removedBodies, aby uzyskać tylko te, które zostały usunięte
        removedBodies.RemoveAll(id => !istniejaceCialkaIDs.Contains(id));

        // Tworzenie listy nazw z SelectedBodies dla identyfikatorów w removedBodies
        var removedBodiesNames = SelectedBodies.Where(p => removedBodies.Contains(p.id)).Select(p => p.name).ToList();
        string removedNamesString = string.Join(", ", removedBodiesNames);
        if (bodiesToAdd.Count == 0)
        {
            await _dialogService.ShowMessage("Information", "All selected products are already in the group " + SelectedGrupaCzesci.Name);
        }
        else
        {
            if (!string.IsNullOrEmpty(removedNamesString))
            {
                await _dialogService.ShowMessage("Information", "Products " + removedNamesString + " are already in the group " + SelectedGrupaCzesci.Name + ", adding only remaining " + bodiesToAdd.Count + " product(s)");
            }
            await _databaseAccessLayer.AddBodies2Group(bodiesToAdd, SelectedGrupaCzesci.Id);

            await PopulateGroupsOfBodies(-1);
        }
        await PopulateLastBodies();
    }

    private async Task ExecuteCreateNewBodyGroup()
    {
        var bodiesToAdd = SelectedBodies.Select(p => p.id).ToList();
        List<string> forbiddenNames = (await _databaseAccessLayer.Group4Bodies()).Select(p => p.name.ToLower()).ToList();
        var qvm = new ConfirmNameViewModel(_dialogService, forbiddenNames);
        qvm.RequestClose += async (sender, e) =>
        {

            var result = ((ConfirmNameViewModel)sender).Response;
            if (result)
            {
                var name = ((ConfirmNameViewModel)sender).Name;
                var g4b = await _databaseAccessLayer.AddGroup4Body(name);
                await _databaseAccessLayer.AddBodies2Group(bodiesToAdd, g4b.group4bodiesID);

                var task = PopulateGroupsOfBodies(-1);



                _dialogService.ShowMessage("Info", "New group added succesfully");


                await task;
                return;
            }

        };
        _dialogService.ShowDialog(qvm);


    }

  

    private async Task ExecuteSavebodiesname()
    {
        if (string.IsNullOrEmpty(Bodiesgroupname))
        {
            return;
        }
        if ((await _databaseAccessLayer.Group4Bodies()).Select(p => p.name.ToLower()).Contains(Bodiesgroupname.ToLower().Trim()))
        {
            _dialogService.ShowMessage("Name is already in use", "Name \"" + Bodiesgroupname + "\" is already in use, please use different name");
            return;
        }
        await _databaseAccessLayer.RenameGroup4Bodies(SelectedGrupaCzesci.Id, Bodiesgroupname);
        var item = DostepneGrupy.FirstOrDefault(x => x.Id == SelectedGrupaCzesci.Id);
        if (item != null)
        {
            item.Name = Bodiesgroupname;
            int index = DostepneGrupy.IndexOf(item);
            DostepneGrupy[index] = item;
        }
    }

    private async Task ExecuteDeletebodiesgroup()
    {
        var bg =(await _databaseAccessLayer.Group4Bodies()).First(p => p.name.Equals(SelectedGrupaCzesci.Name));
        var confirmed =await _dialogService.ShowYesNoMessageBox("Confirm deletion", "Do you want to proceed with deleting group " + bg.name);
        if (confirmed)
        {
            await _databaseAccessLayer.RemoveBodiesGrouped(SelectedGrupaCzesci.Id);
        }
        var item = DostepneGrupy.FirstOrDefault(x => x.Id == bg.group4bodiesID);
        if (item != null)
        {

            int index = DostepneGrupy.IndexOf(item);
            DostepneGrupy.RemoveAt(index);

        }
        var item2 = AvailableGroups.FirstOrDefault(x => x.Id == bg.group4bodiesID);
        if (item2 != null)
        {
            AvailableGroups.Remove(item2);
        }

        SelectedGrupaCzesci = null;
    }

}