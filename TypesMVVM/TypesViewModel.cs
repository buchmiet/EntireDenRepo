using denSharedLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace TypesMVVM;

public class TypesViewModel : ObservableObject, INotifyPropertyChanged
{
    private string _typName;
    private string _listOfParameters;
    private string _parametersPanel;
    private string _chooseT;

    public string TypName
    {
        get => _typName;
        set => SetProperty(ref _typName, value);
    }

    public string ListOfParameters
    {
        get => _listOfParameters;
        set => SetProperty(ref _listOfParameters, value);
    }

    public string ParametersPanel
    {
        get => _parametersPanel;
        set => SetProperty(ref _parametersPanel, value);
    }

    public string ChooseT
    {
        get => _chooseT;
        set => SetProperty(ref _chooseT, value);
    }

    private string _typesText;

    public string TypesText
    {
        get => _typesText;
        set
        {

            if (SetProperty(ref _typesText, value))
            {
                if (string.IsNullOrEmpty(value))
                {
                    GoodToAddType = false;
                } else
                {
                    string lowerValue = value.ToLower();
                     
                        
                    if (!_databaseAccessLayer.types().Result .Select(p => p.Value.ToLower()).Any(q => q.Equals(lowerValue)))

                    {
                        GoodToAddType = true;
                        CanRemoveType = false;
                    } else
                    {
                        GoodToAddType = false;
                        SelectedType=AllTypes.First(p=>p.Name.ToLower()== lowerValue);
                    }
                }
            }

        }
    }

    public AsyncRelayCommand TypNameLoadedCommand { get; private set; }
    public AsyncRelayCommand TypNameSelectionChangedCommand { get; private set; }
    public AsyncRelayCommand AddTypeCommand { get; private set; }
    public AsyncRelayCommand RemoveTypeCommand { get; private set; }
    public AsyncRelayCommand ParametersLoadedCommand { get; private set; }
    public AsyncRelayCommand ChooseTLoadedCommand { get; private set; }
    public AsyncRelayCommand AddParameterCommand { get; private set; }
    public AsyncRelayCommand LoadDataCommand { get; private set; }
    public AsyncRelayCommand ComboBoxSelectionChangedCommand { get; private set; }
    

    public IDatabaseAccessLayer _databaseAccessLayer;
    public int locationID;
    public IDialogService _dialogService;
    public ICommand TextInputCommand { get; private set; }
    public TypesViewModel(IDatabaseAccessLayer databaseAccessLayer, IDialogService dialogService,ISettingsService settingsService)
    {
        _databaseAccessLayer = databaseAccessLayer;
        _dialogService = dialogService;
        locationID = settingsService.LocationId;
        TypNameLoadedCommand = new AsyncRelayCommand(ExecuteTypNameLoaded);
        TypNameSelectionChangedCommand = new AsyncRelayCommand(ExecuteTypNameSelectionChanged);
        AddTypeCommand = new AsyncRelayCommand(ExecuteAddType);
        RemoveTypeCommand = new AsyncRelayCommand(ExecuteRemoveType);
        ParametersLoadedCommand = new AsyncRelayCommand(ExecuteParametersLoaded);
        ChooseTLoadedCommand = new AsyncRelayCommand(ExecuteChooseTLoaded);
        AddParameterCommand = new AsyncRelayCommand(ExecuteAddParameter);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        ComboBoxSelectionChangedCommand = new AsyncRelayCommand(SelectionInComboChanged);
         

    }

    public async Task DoParameters()
    {
        ParametryItems.Clear();
            
        var count =  (await _databaseAccessLayer.TypePars())[SelectedType.Id].Count;
        foreach (var ws in (await _databaseAccessLayer.TypePars())[SelectedType.Id])
        {
            int parameterID = ws.parameterID;
            int typeid=SelectedType.Id;
            ParametryItems.Add(new ParameterViewModel
            {
                Id = ws.parameterID,
                Nazwa = (await _databaseAccessLayer.parameter())[ws.parameterID].name,
                Pos = ws.pos,
                Max = count,
                RemoveCommand= new AsyncRelayCommand(() => RemoveTypePar(parameterID,typeid)),
                MoveUpCommand=new AsyncRelayCommand(() => MoveUpParameter(parameterID, typeid)),
                MoveDownCommand= new AsyncRelayCommand(() => MoveDownParameter(parameterID, typeid))
            });
        }
            
    }

    public async Task MoveDownParameter(int parameterID, int typeid)
    {
        await _databaseAccessLayer.MoveDownTypePar(parameterID, typeid);

        await DoParameters();
    }


    public async Task MoveUpParameter(int parameterID, int typeid)
    {
        await _databaseAccessLayer.MoveUpTypePar(parameterID, typeid);

        await DoParameters();
    }

    public async Task RemoveTypePar(int parameterID, int typeid)
    {
        await _databaseAccessLayer. RemoveTypeParameterRelation(parameterID,typeid);
        AllParameters.Add(new IdNameOO
        {
            Id = parameterID,
            Name = (await _databaseAccessLayer.parameter())[parameterID].name
        });
        await DoParameters();
    }

    public ObservableCollection<ParameterViewModel> ParametryItems { get; set; } = new ObservableCollection<ParameterViewModel>();

    public async Task SelectionInComboChanged()
    {
        if (SelectedType == null)
        {
            CanRemoveType = false;
            GoodToAddType = false;
            ParametryItems.Clear() ;
            return;
        }
        CanRemoveType = true;
        var wszystkiecechy= (await _databaseAccessLayer.parameter()).Select(p=>p.Value).ToList();
        List<int> dobrecechy = new();
        if ((await _databaseAccessLayer.TypePars()).ContainsKey(SelectedType.Id))
        {
            dobrecechy= (await _databaseAccessLayer.TypePars())[SelectedType.Id].Select(p => p.parameterID).ToList();
        }
        AllParameters = new ObservableCollection<IdNameOO>();
        foreach(var ws in wszystkiecechy)
        {
            if (!dobrecechy.Contains(ws.parameterID))
            {
                AllParameters.Add(new IdNameOO
                {
                    Id = ws.parameterID,
                    Name = (await _databaseAccessLayer.parameter())[ws.parameterID].name
                });
            }
        }
        if (dobrecechy.Count > 0)
        {
            DoParameters();
        }

    }

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set
        {
            SetProperty(ref _isDataLoaded, value);
        }
    }

    private bool _canRemoveType = false;

    public bool CanRemoveType
    {
        get => _canRemoveType;
        set
        {
            SetProperty(ref _canRemoveType, value);
        }
    }

    private bool _goodToAddType=false;
    public bool GoodToAddType
    {
        get => _goodToAddType;
        set
        {
            SetProperty(ref _goodToAddType, value);
        }
    }
    private bool _goodToAddParameter = false;
    public bool GoodToAddParameter
    {
        get => _goodToAddParameter;
        set
        {
            SetProperty(ref _goodToAddParameter, value);
        }
    }

    private bool _isDataLoaded = false;
    private ObservableCollection<IdNameOO> _allTypes;
    public ObservableCollection<IdNameOO> AllTypes
    {
        get { return _allTypes; }
        set { SetProperty(ref _allTypes, value); }
    }

    private ObservableCollection<IdNameOO> _allParameters;
    public ObservableCollection<IdNameOO> AllParameters
    {
        get { return _allParameters; }
        set { SetProperty(ref _allParameters, value); }
    }

    private IdNameOO _selectedParameter;
    public IdNameOO SelectedParameter
    {
        get { return _selectedParameter; }
        set { SetProperty(ref _selectedParameter, value);
            if (value != null)
            {
                GoodToAddParameter = true;
            }
        }
    }
    private IdNameOO _selectedType;

    public IdNameOO SelectedType
    {
        get => _selectedType;
        set => SetProperty(ref _selectedType, value);
    }

    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        await _databaseAccessLayer.GetPackage(1);
        AllTypes = new ObservableCollection<IdNameOO>((await _databaseAccessLayer.types()).Where(p=>!p.Value.ToLower().Equals("unassigned")).Select(type => new IdNameOO { Id = type.Key, Name = type.Value }));
        IsDataLoaded = true;
    }


    private async Task ExecuteTypNameLoaded()
    {
        // Logika dla TypName_Loaded
    }

    private async Task ExecuteTypNameSelectionChanged()
    {
        // Logika dla TypName_SelectionChanged
    }

    private async Task ExecuteAddType()
    {
        if (string.IsNullOrEmpty(TypesText))
        {
            return;
        }
        var typek=await _databaseAccessLayer.AddType(TypesText);
        var nowyTyp = new IdNameOO
        {
            Id = typek.typeID,
            Name = typek.name,
        };
        AllTypes.Add(nowyTyp);
        SelectedType = nowyTyp;
        GoodToAddType = false;
    }

    private async Task ExecuteRemoveType()
    {
        var typek = SelectedType;
           
        
        await _databaseAccessLayer.RemoveType(typek.Id);
        AllTypes.Remove(typek);
        SelectedType = null;
    }

    private async Task ExecuteParametersLoaded()
    {
        // Logika dla Parameters_Loaded
    }

    private async Task ExecuteChooseTLoaded()
    {
        // Logika dla ChooseT_Loaded
    }

    private async Task ExecuteAddParameter()
    {
        var typeid = SelectedType.Id;
        var parameterID=SelectedParameter.Id;
        await _databaseAccessLayer.AddTypeParameterRelation(typeid, parameterID);
        var doUsu = AllParameters.First(p => p.Id == parameterID);
        AllParameters.Remove(doUsu);
        await DoParameters();
    }
}