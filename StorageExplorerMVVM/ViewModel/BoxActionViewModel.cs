using denModels;
using denSharedLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace StorageExplorerMVVM;

public class BoxActionViewModel : ObservableObject,IAsyncDialogViewModel
{
    private string _changeOrAssign;
  
    private KeyValuePair<BodyInTheBoxActons, StringInt> _response;

    private int _copies = 1;

    private bool _emptyBox = true; 
    public event AsyncEventHandler RequestClose;
    public ObservableCollection<BoolString> LabelProps { get; set; } = new ObservableCollection<BoolString>();
    public async Task PerformTest()
    {
        if (LabelProps.Count == 0)
        {
            await _dialogService.ShowMessage(
                denLanguageResourses.Resources.NoLabelsDesignedMessage,
                denLanguageResourses.Resources.LabelManagerDesignRequiredMessage);

            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
    public ICommand RadioButtonCommand => new RelayCommand<string>(LabelTypeClicked);
    public void LabelTypeClicked(string name)
    {
        foreach (var prop in LabelProps)
        {
            if (!prop.Name.Equals(name))
            {
                prop.Tick = false;
            }
            else
            {
                prop.Tick = true;
            }
        }
    }

    private readonly IDialogService _dialogService;
    public IDatabaseAccessLayer _databaseAccessLayer;
    public AsyncRelayCommand PerformTestCommand { get; set; }
    private ISettingsService _settingsService;
    public BoxActionViewModel(BodyInTheBoxActons CoA, IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer,ISettingsService settingsService) 
    {
        _settingsService = settingsService;
        _dialogService = dialogService;
        _databaseAccessLayer = databaseAccessLayer;
        if (CoA == BodyInTheBoxActons.Change)
        {
            ChangeOrAssign = denLanguageResourses.Resources.ChangeMPN;
            EmptyBox = true;
        }
        if (CoA == BodyInTheBoxActons.Assign)
        {
            ChangeOrAssign = denLanguageResourses.Resources.AssignMPN;
        }
        var l = LabelPropertiesManager.GetLabelProperties().Where(p => p.LabelType == LabelType.ProductLabel).Select(p => p.LabelName);
        PerformTestCommand = new AsyncRelayCommand(PerformTest);


        foreach (var prop in l)
        {
            LabelProps.Add(new BoolString
            {
                Tick = false,
                Name = prop
            });
        }

        // Default label selection
        var defaultLabelSetting = settingsService.GetSetting("default_label");
        var availableLabels = LabelProps.Select(p => p.Name).ToList();

        if (!defaultLabelSetting.IsSuccess)
        {
            // Brak ustawienia - zainicjuj pierwszym dostępnym labelem
            var firstLabel = LabelProps.First();
            firstLabel.Tick = true;
            settingsService.UpdateSetting("default_label", firstLabel.Name);
        }
        else
        {
            var labelValue = defaultLabelSetting.GetValue<string>();

            if (availableLabels.Contains(labelValue))
            {
                // Poprawna wartość - zaznacz odpowiedni label
                LabelProps.First(p => p.Name == labelValue).Tick = true;
            }
            else
            {
                // Nieprawidłowa wartość - zaznacz pierwszy i zaktualizuj ustawienie
                var firstLabel = LabelProps.First();
                firstLabel.Tick = true;
                settingsService.UpdateSetting("default_label", firstLabel.Name);
            }
        }
        PerformTestCommand.Execute(null);
    }

    public string ChangeOrAssign
    {
        get => _changeOrAssign;
        private set => SetProperty(ref _changeOrAssign, value);
    }

    public enum BodyInTheBoxActons
    {
        Change,
        Assign,
        Remove,
        Print,
        Cancel
    }

    
    public KeyValuePair<BodyInTheBoxActons, StringInt> Response
    {
        get => _response;
        set => SetProperty(ref _response, value);
    }

    public int Copies
    {
        get => _copies;
        set
        {
            SetProperty(ref _copies, value);
            OnPropertyChanged(nameof(CopiesDisplay));
        }
    }

    public string CopiesDisplay => Copies.ToString();

    public bool EmptyBox
    {
        get => _emptyBox;
        set => SetProperty(ref _emptyBox, value);
    }
       

    public RelayCommand ChangeMPNCommand => new RelayCommand(() =>
    {
        Response = new KeyValuePair<BodyInTheBoxActons, StringInt>(BodyInTheBoxActons.Change,null );
        RequestClose?.Invoke(this, EventArgs.Empty);
    });

    public AsyncRelayCommand PrintBoxCommand => new(async() =>
    {

        //if (VertOr) Response = new KeyValuePair<DymoLabelSizeOrientation, int>(DymoLabelSizeOrientation.Label32x57Portrait, Copies); 
        //if (HorOr) Response = new KeyValuePair<DymoLabelSizeOrientation, int>(DymoLabelSizeOrientation.Label32x57Landscape, Copies);
        //if (LongOr) Response = new KeyValuePair<DymoLabelSizeOrientation, int>(DymoLabelSizeOrientation.Label28x89, Copies);
        //var odpo = LabelProps.First(p => p.Tick);
        //var lb = LabelPropertiesManager.GetProperty(odpo.Name);
        //var np = await LabelPropertiesManager.GetLabelNamePack(_databaseAccessLayer, cialko.itembodyID);
        //PrintersService.PrintBWLabel(lb, np, SettingsService.GetAllSettings().First(p => p.Key.Equals("label_printer")).Value, PrintTimes);
        Response = new KeyValuePair<BodyInTheBoxActons, StringInt>(BodyInTheBoxActons.Print, new StringInt(LabelProps.First(p => p.Tick).Name, Copies));

        RequestClose?.Invoke(this, EventArgs.Empty);
    });

    public RelayCommand CancelCommand => new RelayCommand(() =>
    {
        //   Response = new KeyValuePair<DymoLabelSizeOrientation, int>(DymoLabelSizeOrientation.Cancel, Copies);
        Response = new KeyValuePair<BodyInTheBoxActons, StringInt>(BodyInTheBoxActons.Cancel, null);
        RequestClose?.Invoke(this, EventArgs.Empty);
    });

    public RelayCommand DecrementCopiesCommand => new RelayCommand(() =>
    {
        if (Copies > 1)
        {
            Copies--;
        }
    });

    public RelayCommand IncrementCopiesCommand => new RelayCommand(() =>
    {
        Copies++;
    });

    public RelayCommand RemoveMPNCommand => new(() =>
    {
        // Response = new KeyValuePair<DymoLabelSizeOrientation, int>(DymoLabelSizeOrientation.RemoveMpn, 0);
        Response = new KeyValuePair<BodyInTheBoxActons, StringInt>(BodyInTheBoxActons.Remove, null);
        RequestClose?.Invoke(this, EventArgs.Empty);
    });

    //public RelayCommand SetVertOrCommand => new RelayCommand(() => SetOrientation(true, false, false));
    //public RelayCommand SetHorOrCommand => new RelayCommand(() => SetOrientation(false, true, false));
    //public RelayCommand SetLongOrCommand => new RelayCommand(() => SetOrientation(false, false, true));

    //private void SetOrientation(bool vert, bool hor, bool longOr)
    //{
    //    VertOr = vert;
    //    HorOr = hor;
    //    LongOr = longOr;
    //}
}