using denMethods;
using denSharedLibrary;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using denLanguageResourses;
using LabelType = denSharedLibrary.LabelType;
using Printers;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;


namespace denViewModels;

public class LabelControlViewModel : ObservableObject
{




    public ObservableCollection<ILabelTypeViewModel> LabelTypes { get; set; } = new();

    private ObservableCollection<string> _printers;

    public ObservableCollection<string> Printers
    {
        get => _printers;
        set => SetProperty(ref _printers, value);
    }
    private string _selectedPrinter;

    public string SelectedPrinter
    {
        get => _selectedPrinter;
        set
        {
            if (SetProperty(ref _selectedPrinter, value))
            {
                _settingsService.UpdateSettings(new Dictionary<string, string> { { "label_printer", SelectedPrinter } });
                foreach (var labelTypeViewModel in LabelTypes)
                {
                    labelTypeViewModel.SetPrinterName(SelectedPrinter);
                }

            }
        }
    }




    private bool _isComboBoxEnabled;
    public bool IsComboBoxEnabled
    {
        get => _isComboBoxEnabled;
        set => SetProperty(ref _isComboBoxEnabled, value);
    }

    public ICommand AddNewProductLabelCommand { get; }
    public ICommand AddNewAddressLabelCommand { get; }
    public ICommand AddNewReturnLabelCommand { get; }
    public AsyncRelayCommand RefreshData { get; }


    private readonly ISettingsService _settingsService;
    private readonly IPrintersService _printersService;
    private readonly IDatabaseAccessLayer _databaseAccessLayer;
    private readonly IDialogService _dialogService; 

    public LabelControlViewModel(
        ISettingsService settingsService,
        IPrintersService printersService,
        IDatabaseAccessLayer databaseAccessLayer,
        IDialogService dialogService
        )
    {
        _settingsService = settingsService;
        _printersService = printersService;
        _databaseAccessLayer = databaseAccessLayer;
        _dialogService = dialogService;
        AddNewReturnLabelCommand = new AsyncRelayCommand(AddNewReturnLabelExecute);
        AddNewProductLabelCommand = new AsyncRelayCommand(AddNewProductLabelExecute);
        AddNewAddressLabelCommand = new AsyncRelayCommand(AddNewAddressLabelExecute);
        var druczki = _printersService.GetPrinters("label_printer");
        Printers = new ObservableCollection<string>(druczki.Value);
        RefreshData = new AsyncRelayCommand(GetDataExecute);
        RefreshData.Execute(null);
        if (Printers.Count > 0)
        {
            SelectedPrinter = druczki.Key;
            IsComboBoxEnabled = true;
        }
        else
        {
            IsComboBoxEnabled = false;
        }
    }


    private bool _isDataLoaded;
    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set => SetProperty(ref _isDataLoaded, value);
    }

      

    public async Task AddNewReturnLabelExecute()
    {

        var lab = new LabelProperties
        {
            LabelName = "Return Label",
            LabelType = LabelType.ReturnLabel,
            Width = 57,
            Height = 32,
            Landscape = true
        };
        LabelPropertiesManager.SaveLabelProperties(lab);
        string BusinessAddress;
        //tring.IsNullOrEmpty(SettingsService.GetSetting("businessname")) ? SettingsService.GetSetting("businessaddress") : SettingsService.GetSetting("businessaddress")+Environment.NewLine+SettingsService.GetSetting("businessaddress");
        var adr = _settingsService.GetSetting("businessaddress").GetValue<string>();

        // change into ?? expression

        BusinessAddress = adr != null ? Base64Converter.DecodeBase64ToString(adr.Trim()) : AddressGenerator.GenerateRandomUKAddress();
        if (!string.IsNullOrEmpty(_settingsService.GetSetting("businessname").GetValue<string>()))
        {
            adr += string.IsNullOrEmpty(_settingsService.GetSetting("businessname").GetValue<string>());
        }
            
        var retlab = new AddressLabelViewModel(lab, SelectedPrinter, BusinessAddress, _dialogService,_settingsService,_printersService, typeOfAddressLabel:AddressLabelViewModel.TypeOfAddressLabel.returnlabel);
        retlab.DeleteCommand = new AsyncRelayCommand(async () => { await RemoveLabelTemplate(lab.LabelName); });
        LabelTypes.Add(retlab);
        Need2AddReturnLabel = false;
           
    }

     


    public async Task AddNewAddressLabelExecute()
    {

        var lab = new LabelProperties
        {
            LabelName = "Address Label",
            LabelType = LabelType.AddressLabel,
            Width = 70,
            Height = 54,
            Landscape = true
        };
        LabelPropertiesManager.SaveLabelProperties(lab);
        string BusinessAddress = AddressGenerator.GenerateRandomUKAddress();

        var adrlab = new AddressLabelViewModel(lab, SelectedPrinter, BusinessAddress, _dialogService, _settingsService, _printersService);
        adrlab.DeleteCommand = new AsyncRelayCommand(async () => { await RemoveLabelTemplate(lab.LabelName); });
        LabelTypes.Add(adrlab);
        Need2AddAddressLabel = false;
    }


    public async Task AddNewProductLabelExecute()
    {
        var addlabelproperty = new AddLabelPropertyViewModel(_dialogService);
        addlabelproperty.RequestClose += async (sender, e) =>
        {
            var result = ((AddLabelPropertyViewModel)sender).Result;
            if (result != null)
            {
                LabelPropertiesManager.SaveLabelProperties(result);
                LabelTypes.Clear();
                RefreshData.Execute(null);
            }
        };
        await _dialogService.ShowDialog(addlabelproperty);
    }




    private bool _need2AddReturnLabel;
    public bool Need2AddReturnLabel
    {
        get => _need2AddReturnLabel;
        set => SetProperty(ref _need2AddReturnLabel, value);
    }

    private bool _need2AddAddressLabel;
    public bool Need2AddAddressLabel
    {
        get => _need2AddAddressLabel;
        set => SetProperty(ref _need2AddAddressLabel, value);
    }

    public async Task GetDataExecute()
    {
        await _databaseAccessLayer.GetPackage(1);
        var przykladowe2 = _databaseAccessLayer.items.Where(p => p.Value.bodyinthebox != null);

        var przykladowe = przykladowe2.Select(p => p.Key).ToList();
        var LabelProperties = LabelPropertiesManager.GetLabelProperties().ToArray();
        var retlab = LabelProperties.FirstOrDefault(p => p.LabelType == LabelType.ReturnLabel);
        if (retlab == null)
        {
            Need2AddReturnLabel = true;
        }
        else
        {
            Need2AddReturnLabel = false;
        }
        retlab = LabelProperties.FirstOrDefault(p => p.LabelType == LabelType.AddressLabel);
        if (retlab == null)
        {
            Need2AddAddressLabel = true;
        }
        else
        {
            Need2AddAddressLabel = false;
        }
        var random = new Random();
        var indices = new HashSet<int>();  // zbiór do przechowywania unikatowych indeksów
        while (indices.Count < LabelProperties.Length && indices.Count < przykladowe.Count)
        {
            indices.Add(random.Next(przykladowe.Count));  // dodaj losowy indeks do zbioru
        }
        var losoweElementy = indices.Select(index => przykladowe[index]).ToArray();
        for (int i = 0; i < LabelProperties.Count(); i++)
        {
            if (LabelProperties[i].LabelType == LabelType.ProductLabel)
            {
                var namepack = await LabelPropertiesManager.GetLabelNamePack(_databaseAccessLayer, losoweElementy[i]);
                var name = LabelProperties[i].LabelName;
                var labelTypeViewModel = new ProductLabelViewModel
                (
                    namepack,
                    LabelProperties[i],
                    SelectedPrinter,
                    _printersService

                );
                labelTypeViewModel.DeleteCommand = new AsyncRelayCommand(async () => { await RemoveLabelTemplate(name); });
                LabelTypes.Add(labelTypeViewModel);
            }
            if (LabelProperties[i].LabelType == LabelType.ReturnLabel)
            {

                string BusinessAddress = _settingsService.GetSetting("businessname").GetValue<string>() == null ? "" : _settingsService.GetSetting("businessname").GetValue<string>() + Environment.NewLine;
                var adr = _settingsService.GetSetting("businessaddress").GetValue<string>();
                if (adr != null)
                {
                    BusinessAddress += Base64Converter.DecodeBase64ToString(adr.Trim());
                }
                else
                {
                    BusinessAddress = AddressGenerator.GenerateRandomUKAddress();
                }
                var retlab2 = new AddressLabelViewModel(LabelProperties[i], SelectedPrinter, BusinessAddress, _dialogService, _settingsService, _printersService);
                var name = LabelProperties[i].LabelName;
                retlab2.DeleteCommand = new AsyncRelayCommand(async () => { await RemoveLabelTemplate(name); });
                LabelTypes.Add(retlab2);
            }
            if (LabelProperties[i].LabelType == LabelType.AddressLabel)
            {
                var adr = AddressGenerator.GenerateRandomUKAddress();
                var retlab2 = new AddressLabelViewModel(LabelProperties[i], SelectedPrinter, adr, _dialogService, _settingsService, _printersService);
                var name = LabelProperties[i].LabelName;
                retlab2.DeleteCommand = new AsyncRelayCommand(async () => { await RemoveLabelTemplate(name); });
                LabelTypes.Add(retlab2);
            }

        }
        IsDataLoaded = true;
    }

    public async Task RemoveLabelTemplate(string labelName)
    {
        var resp = await _dialogService.ShowYesNoMessageBox(
            Resources.ConfirmYourChoice,
            string.Format(Resources.RemoveLabelConfirmationMessage, labelName));
        if (resp)
        {
            if (LabelPropertiesManager.RemoveLabelProperties(labelName))
            {
                await _dialogService.ShowMessage(Resources.Info, Resources.LabelTemplateRemovedMessage);
                LabelTypes.Clear();
                await GetDataExecute();
            }
            else
            {
                await _dialogService.ShowMessage(Resources.Info, Resources.ErrorRemovingLabelTemplateMessage);
            }
        }
    }


}