using denSharedLibrary;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace denViewModels;

public class PrametersViewModel : ObservableObject
{
    public AsyncRelayCommand LoadDataCommand { get; private set; }
    public IDatabaseAccessLayer _databaseAccessLayer;
    public int locationID;
    public IDialogService _dialogService;
    public AsyncRelayCommand ComboBoxSelectionChangedCommand { get; private set; }
    public AsyncRelayCommand AddCechaCommand { get; private set; }
    public AsyncRelayCommand RemoveCechaCommand { get; private set; }
    private IDispatcherService _dispatcherService;
    ISettingsService _settingsService;

    public PrametersViewModel(IDatabaseAccessLayer databaseAccessLayer, IDialogService dialogService, IDispatcherService dispatcherService,ISettingsService settingsService)
    {
        _databaseAccessLayer = databaseAccessLayer;
        _dialogService = dialogService;
        locationID = settingsService.LocationId;
        ComboBoxSelectionChangedCommand = new AsyncRelayCommand(SelectionInComboChanged);
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddCechaCommand = new AsyncRelayCommand(ExecuteAddCecha);
        RemoveCechaCommand = new AsyncRelayCommand(ExecuteRemoveCecha);
        AddCechaValueCommand = new AsyncRelayCommand(AddCechaValue);
        _dispatcherService = dispatcherService;
    }

    private async Task ExecuteRemoveCecha()
    {
        var cecha = SelectedCecha;
        await _databaseAccessLayer.RemoveCecha(cecha.Id);
        AllCechies.Remove(cecha);
        SelectedCecha = null;
    }

    public async Task ExecuteAddCecha()
    {
        if (string.IsNullOrEmpty(ParametersText))
        {
            return;
        }
        var cecha = await _databaseAccessLayer.AddCecha(ParametersText);
        var nowaCecha = new IdNameOO
        {
            Id = cecha.parameterID,
            Name = cecha.name,
        };
        AllCechies.Add(nowaCecha);
        SelectedCecha = nowaCecha;
        GoodToAddCecha = false;
    }

    private bool _isDataLoaded = false;

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set
        {
            SetProperty(ref _isDataLoaded, value);
        }
    }

    private ObservableCollection<IdNameOO> _allCechies;

    public ObservableCollection<IdNameOO> AllCechies
    {
        get { return _allCechies; }
        set { SetProperty(ref _allCechies, value); }
    }

    private IdNameOO _selectedCecha;

    public IdNameOO SelectedCecha
    {
        get => _selectedCecha;
        set => SetProperty(ref _selectedCecha, value);
    }

    private bool _goodToAddCecha = false;

    public bool GoodToAddCecha
    {
        get => _goodToAddCecha;
        set
        {
            SetProperty(ref _goodToAddCecha, value);
        }
    }

    private bool _goodToAddValue = false;

    public bool GoodToAddValue
    {
        get => _goodToAddValue;
        set
        {
            SetProperty(ref _goodToAddValue, value);
        }
    }

    private bool _canRemoveCecha = false;

    public bool CanRemoveCecha
    {
        get => _canRemoveCecha;
        set
        {
            SetProperty(ref _canRemoveCecha, value);
        }
    }

    private string _newValueText;

    public string NewValueText
    {
        get => _newValueText;
        set
        {
            if (SetProperty(ref _newValueText, value))
            {
                if (string.IsNullOrEmpty(value))
                {
                    GoodToAddValue = false;
                }
                else
                {
                    if (_databaseAccessLayer.cechyValues().Result.ContainsKey(SelectedCecha.Id))
                    {
                        var LowerValue = value.ToLower();
                        var dobreValues = _databaseAccessLayer.cechyValues().Result[SelectedCecha.Id].Select(p => p.name.ToLower()).ToList();
                        if (!dobreValues.Any(p => p.Equals(LowerValue)))
                        {
                            GoodToAddValue = true;
                        }
                        else
                        {
                            GoodToAddValue = false;
                        }
                    }
                    else
                    {
                        GoodToAddValue = true;
                    }
                }
            }
        }
    }

    private string _parametersText;

    public string ParametersText
    {
        get => _parametersText;
        set
        {
            if (SetProperty(ref _parametersText, value))
            {
                if (string.IsNullOrEmpty(value))
                {
                    GoodToAddCecha = false;
                }
                else
                {
                    string lowerValue = value.ToLower();

                    if (!_databaseAccessLayer.parameter().Result.Select(p => p.Value.name.ToLower()).Any(q => q.Equals(lowerValue)))

                    {
                        GoodToAddCecha = true;
                        CanRemoveCecha = false;
                    }
                    else
                    {
                        GoodToAddCecha = false;
                        SelectedCecha = AllCechies.First(p => p.Name.ToLower() == lowerValue);
                    }
                }
            }
        }
    }

    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        await _databaseAccessLayer.GetPackage(1);
        AllCechies = new ObservableCollection<IdNameOO>((await _databaseAccessLayer.parameter()).Select(type => new IdNameOO { Id = type.Key, Name = type.Value.name }));
        IsDataLoaded = true;
    }

    public ObservableCollection<ValueViewModel> ValuesItems { get; set; } = new ObservableCollection<ValueViewModel>();

    public async Task SelectionInComboChanged()
    {
        if (SelectedCecha == null)
        {
            CanRemoveCecha = false;
            GoodToAddCecha = false;
            ValuesItems.Clear();
            return;
        }
        CanRemoveCecha = true;
        List<int> dobreValues = new();
        if ((await _databaseAccessLayer.cechyValues()).ContainsKey(SelectedCecha.Id))
        {
            dobreValues = (await _databaseAccessLayer.cechyValues())[SelectedCecha.Id].Select(p => p.parameterValueID).ToList();
        }
        if (dobreValues.Count > 0)
        {
            await DoValues();
        }
    }

    public async Task DoValues()
    {
        ValuesItems.Clear();

        var count = (await _databaseAccessLayer.cechyValues())[SelectedCecha.Id].Count;
        foreach (var ws in (await _databaseAccessLayer.cechyValues())[SelectedCecha.Id].OrderBy(p => p.pos))
        {
            int parameterID = ws.parameterID;
            int valueid = ws.parameterValueID;
            ValuesItems.Add(new ValueViewModel
            {
                Id = parameterID,
                Nazwa = (await _databaseAccessLayer.cechyValues())[parameterID].First(p => p.parameterValueID == valueid).name,
                Pos = ws.pos,
                Max = count,
                RemoveCommand = new AsyncRelayCommand(() => RemoveCechaValue(valueid)),
                MoveUpCommand = new AsyncRelayCommand(() => MoveUpCechaValue(valueid)),
                MoveDownCommand = new AsyncRelayCommand(() => MoveDownCechaValue(valueid))
            });
        }
    }

    public async Task MoveUpCechaValue(int parameterValueID)
    {
        await _databaseAccessLayer.MoveUpCechaValue(parameterValueID);
        await DoValues();
    }

    public async Task MoveDownCechaValue(int parameterValueID)
    {
        await _databaseAccessLayer.MoveDownCechaValue(parameterValueID);
        await DoValues();
    }

    public async Task RemoveCechaValue(int parameterValueID)
    {
        await _databaseAccessLayer.RemoveParameterValue(parameterValueID);
        await DoValues();
    }

    public ICommand AddCechaValueCommand { get; set; }

    private async Task AddCechaValue()
    {
        if (string.IsNullOrEmpty(NewValueText))
        {
            return;
        }
        var cechaValue = await _databaseAccessLayer.AddCechaValue(SelectedCecha.Id, NewValueText);
        int parameterID = cechaValue.parameterID;
        int valueid = cechaValue.parameterValueID;
        int max = (await _databaseAccessLayer.cechyValues())[parameterID].Count;
        ValuesItems.Add(new ValueViewModel
        {
            Id = parameterID,
            Nazwa = (await _databaseAccessLayer.cechyValues())[parameterID].First(p => p.parameterValueID == valueid).name,
            Pos = cechaValue.pos,
            Max = max,
            RemoveCommand = new AsyncRelayCommand(() => RemoveCechaValue(valueid)),
        });
        await _dispatcherService.Invoke(async () =>
        {
            foreach (var item in ValuesItems)
            {
                item.Max = max;
            }
        });
        NewValueText = "";
    }
}