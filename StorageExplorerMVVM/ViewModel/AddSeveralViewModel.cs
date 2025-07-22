using denModels;
using denSharedLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.Models;
using Printers;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace StorageExplorerMVVM;

public class AddSeveralViewModel : ObservableObject, IAsyncDialogViewModel
{
    public event AsyncEventHandler RequestClose;
    private string _brokenLcd;
    private string _currentBody;
    private bool _isRefresherChecked = true;
   
    private short _printTimes;
    public ObservableCollection<string> Items { get; set; }= [];
    private System.Windows.Media.FontFamily _digital7;
    public System.Windows.Media.FontFamily Digital7
    {
        get => _digital7;
        set => SetProperty(ref _digital7, value);
    }

    public Dictionary<string,string> mpns;
    private readonly IDialogService _dialogService;
    public multidrawer multidrawer;
    public IDatabaseAccessLayer DatabaseAccessLayer;


    public ObservableCollection<BoolString> LabelProps { get; set; } = new();

    public AsyncRelayCommand PerformTestCommand { get; set; }

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


    public Dictionary<string, int> _quantities = new();

    IHttpClientFactory _httpClientFactory;
    public IAsyncRelayCommand CheckBrokenLcdCommand { get; }

    private async Task CheckAndSetBrokenLcdAsync(string value)
    {
        if (mpns.ContainsKey(value))
        {
            var inStock = await CasioInteractions.NLAGokanCheck(value, _httpClientFactory);
            CurrentBody = inStock
                ? $"{mpns[value]}, in stock: {_quantities[value]}"
                : $"{mpns[value]}, in stock: {_quantities[value]} DISCONTINUED!";
        }
        else
        {
            CurrentBody = "";
        }
        BrokenLcd = value; // Ustaw wartość bez wywoływania komendy.
    }

    public string BrokenLcd
    {
        get => _brokenLcd;
        set
        {
            if (SetProperty(ref _brokenLcd, value))
            {
                if (mpns.ContainsKey(_brokenLcd))
                {
                    CheckBrokenLcdCommand.Execute(_brokenLcd);
                }
            }
        }
    }

    private ISettingsService SettingsService;
    IPrintersService PrintersService;
    public AddSeveralViewModel(IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer,multidrawer _multidrawer, IHttpClientFactory httpClientFactory,
        IPrintersService printersService,ISettingsService settingsService)
    {
        SettingsService = settingsService;
        PrintersService = printersService;
        CheckBrokenLcdCommand = new AsyncRelayCommand<string>(CheckAndSetBrokenLcdAsync);
        multidrawer = _multidrawer;
        DatabaseAccessLayer = databaseAccessLayer;
        _dialogService = dialogService;
        mpns = DatabaseAccessLayer.items.Values.ToDictionary(x => x.itembody.mpn, q => q.itembody.myname);
        foreach (var item in DatabaseAccessLayer.items)
        {
            _quantities.Add(item.Value.itembody.mpn, item.Value.ItemHeaders.Where(p => p.locationID == 1).Sum(p => p.quantity));
        }
        string plik = "Digital-7";
        string patho = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\#" + plik);
        Digital7 = new System.Windows.Media.FontFamily(patho);
        PrintTimes = 1;
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

        var defaultlabelResponse = SettingsService.GetSetting("default_label");
        if (defaultlabelResponse.IsSuccess && LabelProps.Select(p => p.Name).Contains(defaultlabelResponse.GetValue<string>()))
        {
            LabelProps.First(p => p.Name.Equals(defaultlabelResponse.GetValue<string>())).Tick = true;
        }
        else
        {
            LabelProps.First().Tick = true;
        }
        PerformTestCommand.Execute(null);
        _httpClientFactory = httpClientFactory;
    }

    

    public string CurrentBody
    {
        get => _currentBody;
        set => SetProperty(ref _currentBody, value);
    }

    public bool IsRefresherChecked
    {
        get => _isRefresherChecked;
        set => SetProperty(ref _isRefresherChecked, value);
    }

      

       

    public short PrintTimes
    {
        get => _printTimes;
        set => SetProperty(ref _printTimes, value);
    }


    public ICommand PrintAllClickCommand => new RelayCommand(ButtonClick);
    public ICommand MinusClickCommand => new RelayCommand(MinusClick);
    public ICommand PlusClickCommand => new RelayCommand(PlusClick);
    

    public ICommand EnterCommand => new AsyncRelayCommand(EnterCommandExecute);

    public delegate void BodyInTheBoxAddedEventHandler(int bodyinthebox);
    public  event BodyInTheBoxAddedEventHandler BodyInTheBoxAdded;


    public Pozycja FindFreeSpot(multidrawer multidrawer)
    {
        var zwro = new Pozycja { X = 0, Y = 0 };

        var go = DatabaseAccessLayer.items.Values.Where(q => q.bodyinthebox != null).Select(p => p.bodyinthebox);
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
    public async Task EnterCommandExecute()
    {
        if (!mpns.Keys .Any(p=>p.Equals(BrokenLcd)))
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources. IncorrectSKU, denLanguageResourses.Resources.InvalidSKUEntered);
           
            return;
        }
        var tu = FindFreeSpot(multidrawer);
        if (tu.X == -1)
        {
            return;
        }

        var cialko = DatabaseAccessLayer.items.First(p => p.Value.itembody.mpn.Equals(BrokenLcd)).Value.itembody;

        var exbox = DatabaseAccessLayer.items[cialko.itembodyID].bodyinthebox;
        if (exbox != null)
        {
            var md=(await DatabaseAccessLayer.multidrawer()).First(p=>p.MultiDrawerID==exbox.MultiDrawerID).name;
            await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle,string.Format(denLanguageResourses.Resources.ProductAlreadyAssigned,md,exbox.column,exbox.row));
            return;
        }

        var bb = new bodyinthebox
        {
            column = tu.X,
            row = tu.Y,
            MultiDrawerID = multidrawer.MultiDrawerID,
            itembodyID = cialko.itembodyID
        };
        var bibi=await DatabaseAccessLayer.AddBodyInTheBox(bb);
        BodyInTheBoxAdded?.Invoke(bibi.BodyInTheBoxID);            
        Items.Add(BrokenLcd);


        var odpo = LabelProps.First(p => p.Tick);
        var lb = LabelPropertiesManager.GetProperty(odpo.Name);
        var np = await LabelPropertiesManager.GetLabelNamePack(DatabaseAccessLayer, cialko.itembodyID);    
        PrintersService.PrintBWLabel(lb, np, SettingsService.GetAllSettings().First(p => p.Key.Equals("label_printer")).Value, PrintTimes);
        BrokenLcd = "";
        CurrentBody = "";
    }

       
    private void ButtonClick()
    {
        foreach (var item in Items)
        {

        }
    }

    private void MinusClick()
    {
        if (PrintTimes > 1)
        {
            PrintTimes--;
        }
    }

    private void PlusClick()
    {
        PrintTimes++;
    }
  

}