using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Printers;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace denViewModels.LabelManager;

public class BothLabelsContainerViewModel : ObservableObject
{

    public ICommand LoadDataCommand { get; }
    IDatabaseAccessLayer _databaseAccessLayer;
    private readonly ISettingsService _settingsService;
    public LabelControlViewModel LabelOneVM { get; }
    public Cn22SettingsViewModel Cn22VM { get; }

    public BothLabelsContainerViewModel(
        IDatabaseAccessLayer databaseAccessLayer,
        ISettingsService settingsService,
        LabelControlViewModel labelOneVM, 
        Cn22SettingsViewModel cn22VM)     
    {
        _databaseAccessLayer = databaseAccessLayer;
        _settingsService = settingsService;

        LabelOneVM = labelOneVM;
        Cn22VM = cn22VM;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }


    private bool _isDataLoaded;

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set => SetProperty(ref _isDataLoaded, value);
    }

    private bool _isBusy = true;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);

    }

    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        await _databaseAccessLayer.GetPackage(_settingsService.LocationId);

        IsDataLoaded = true;
        IsBusy = false;
    }
}