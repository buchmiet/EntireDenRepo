using denSharedLibrary;
using denViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.Models;
using Printers;
using SettingsKeptInFile;
using DataServicesNET80.DatabaseAccessLayer;

namespace StorageExplorerMVVM;

public class BodyInTheBoxViewModel : ObservableObject
{
    private string _buttonName;

    private int _row;
    private int _column;
    private int _multiDrawerID;
    private readonly IDialogService _dialogService;

    public ICommand ButtonCommand { get; }

    public int Row => _row;

    public int Column => _column;

    public string ButtonName
    {
        get => _buttonName;
        private set => SetProperty(ref _buttonName, value);
    }


    private bodyinthebox _body;
    public bodyinthebox Body
        
    {
        get => _body;
        set {
            ButtonName = _databaseAccessLayer.items[value.itembodyID].itembody.mpn;
            _body = value;
            OnPropertyChanged(nameof(Body));
        }
    }

    public IDatabaseAccessLayer _databaseAccessLayer;
    public ISettingsService _settingsService;
    public IPrintersService PrintersService;
    public BodyInTheBoxViewModel(IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer, int row, int column, int multiDrawerID, ISettingsService settingsService,IPrintersService printersService,
        bodyinthebox? body = null)
    {
        PrintersService = printersService;
        _settingsService = settingsService;
        _dialogService = dialogService;
        _databaseAccessLayer = databaseAccessLayer;
        _row = row;
        _column = column;
        _multiDrawerID = multiDrawerID;
        _body = body;
        if (body != null)
        {
            ButtonName = _databaseAccessLayer.items[body.itembodyID].itembody.mpn;
        } else {
            ButtonName= "empty box";
        }
         
       
        ButtonCommand = new AsyncRelayCommand(ExecuteButtonCommand);
    }

    private async Task ExecuteButtonCommand()
    {
        BoxActionViewModel qvm;
        if (ButtonName.Equals("empty box"))
        {
            qvm = new BoxActionViewModel(BoxActionViewModel.BodyInTheBoxActons.Assign,_dialogService,_databaseAccessLayer,_settingsService);
        } else
        {
            qvm = new BoxActionViewModel(BoxActionViewModel.BodyInTheBoxActons.Change, _dialogService, _databaseAccessLayer, _settingsService);
        }
            
        qvm.RequestClose += async (sender, e) =>
        {

            var result = ((BoxActionViewModel)sender).Response;
            if (result.Key!= BoxActionViewModel.BodyInTheBoxActons.Cancel)
            {

                 
                if (result.Key == BoxActionViewModel.BodyInTheBoxActons.Change)
                {

                    var pbVM = new ProductBrowserViewModel(_databaseAccessLayer);

                    pbVM.RequestClose += async (sender, e) =>
                    {
                        var result = ((ProductBrowserViewModel)sender).Result;

                        if (result != -1)
                        {
                            if (_databaseAccessLayer.items[result].bodyinthebox != null) 
                            {

                                var box = (await _databaseAccessLayer.multidrawer()).First(p => p.MultiDrawerID == _databaseAccessLayer.items[result].bodyinthebox.MultiDrawerID).name;
                                var column = _databaseAccessLayer.items[result].bodyinthebox.column;
                                var row = _databaseAccessLayer.items[result].bodyinthebox.row;
                                var message = string.Format(denLanguageResourses.Resources.ProductAlreadyAssigned, box, column, row);

                                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, message);


                                //await _dialogService.ShowMessage("Error", "This product is already assigned to box "+
                                //     (await _databaseAccessLayer.multidrawer()).First(p=>p.MultiDrawerID==_databaseAccessLayer.items[result].bodyinthebox.MultiDrawerID).name+
                                //     " to location ["+ _databaseAccessLayer.items[result].bodyinthebox.column+','+ _databaseAccessLayer.items[result].bodyinthebox.row
                                //     +']'
                                //     );

                            }
                            else
                            {
                                if (_body == null)
                                {
                                    var go = new bodyinthebox
                                    {
                                        column = _column,
                                        row = _row,
                                        itembodyID = result,
                                        MultiDrawerID = _multiDrawerID
                                    };
                                    var cialko = await _databaseAccessLayer.AddBodyInTheBox(go);

                                    ButtonName = _databaseAccessLayer.items[cialko.itembodyID].itembody.mpn;
                                    _body = cialko;

                                } else
                                {

                                    var go = new bodyinthebox
                                    {
                                        column = _column,
                                        row = _row,
                                        itembodyID = result,
                                        MultiDrawerID = _multiDrawerID
                                    };
                                    await _databaseAccessLayer.RemoveBodyInTheBox(_body);
                                    var cialko = await _databaseAccessLayer.AddBodyInTheBox(go);
                                    ButtonName = _databaseAccessLayer.items[cialko.itembodyID].itembody.mpn;
                                    _body = cialko;
                                }
                            }

                        }
                    };
                    await  _dialogService.ShowDialog(pbVM);


                      

                }
                if (result.Key== BoxActionViewModel.BodyInTheBoxActons.Print) // (result.Key == DymoLabelSizeOrientation.Label32x57Landscape || result.Key == DymoLabelSizeOrientation.Label32x57Portrait || result.Key == DymoLabelSizeOrientation.Label28x89)
                {


                    var cialko = _databaseAccessLayer.items[_body.itembodyID].itembody;
                    var   Cname = (await _databaseAccessLayer.multidrawer()).First(p => p.MultiDrawerID == _databaseAccessLayer.items[_body.itembodyID].bodyinthebox.MultiDrawerID).name;
                    var lb = LabelPropertiesManager.GetProperty(result.Value.Name);

                    var np = await LabelPropertiesManager.GetLabelNamePack(_databaseAccessLayer, _body.itembodyID);

                    PrintersService.PrintBWLabel(lb, np, _settingsService.GetAllSettings().First(p => p.Key.Equals("label_printer")).Value,(short) result.Value.Id);


                    //      Dymo.PrintBoxLabel(cialko.itembodyID,result, _databaseAccessLayer);                   
                }
                if (result.Key== BoxActionViewModel.BodyInTheBoxActons.Remove) 
                {
                    await _databaseAccessLayer.RemoveBodyInTheBox(_body);
                    _body = null;
                    ButtonName = "empty box";
                }






            }

        };
        await _dialogService.ShowDialog(qvm);
    }
}