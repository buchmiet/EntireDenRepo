using DataServicesNET80.DatabaseAccessLayer;
using denLanguageResourses;
using denModels;
using denSharedLibrary;

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace denViewModels;

public class AddNewProductViewModel : ObservableObject, IAsyncDialogViewModel
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _shortName;
    public string ShortName
    {
        get => _shortName;
        set => SetProperty(ref _shortName, value);
    }

    private string _mpn;
    public string MPN
    {
        get => _mpn;
        set => SetProperty(ref _mpn, value);
    }

    private string _price;
    public string Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    private string _weight = "0";
    public string Weight
    {
        get => _weight;
        set => SetProperty(ref _weight, value);
    }

    private Idname _selectedSupplier;
    public Idname SelectedSupplier
    {
        get => _selectedSupplier;
        set => SetProperty(ref _selectedSupplier, value);
    }

    private Idname _selectedBrand;
    public Idname SelectedBrand
    {
        get => _selectedBrand;
        set => SetProperty(ref _selectedBrand, value);
    }

    private Idname _selectedType;
    public Idname SelectedType
    {
        get => _selectedType;
        set => SetProperty(ref _selectedType, value);
    }

    private readonly IDialogService _dialogService;


    IDatabaseAccessLayer _databaseAccessLayer;
    public AddNewProductViewModel(List<Idname> suppliers, List<Idname> brands, List<Idname> types, IDialogService dialogService, IDatabaseAccessLayer databaseAccessLayer)
    {
        _dialogService = dialogService;
        _databaseAccessLayer = databaseAccessLayer;
        Suppliers = new ObservableCollection<Idname>(suppliers);
        Brands = new ObservableCollection<Idname>(brands);
        Types = new ObservableCollection<Idname>(types);

        AddProductCommand = new AsyncRelayCommand(AddProduct);
        CancelCommand = new AsyncRelayCommand(Cancel);
    }


    public ObservableCollection<Idname> Suppliers { get; }
    public ObservableCollection<Idname> Brands { get; }
    public ObservableCollection<Idname> Types { get; }

    public ICommand AddProductCommand { get; }
    public ICommand CancelCommand { get; }

    public bool DialogResult { get; set; }
    public Dictionary<string, object> Result { get; set; }

    public event AsyncEventHandler RequestClose;


    private async Task AddProduct()
    {
        if (string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(ShortName) ||
            string.IsNullOrWhiteSpace(MPN) ||
            !double.TryParse(Price, out _) ||
            !int.TryParse(Weight, out _) ||
            SelectedSupplier == null ||
            SelectedBrand == null ||
            SelectedType == null)
        {
            // Jeśli nie, wyświetl komunikat o błędzie
            await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, denLanguageResourses.Resources.FillAllFields);
            return; // Dodane, aby zakończyć metodę w przypadku błędu
        }

        ItemBodyUpdateOperationResult rezultat = await _databaseAccessLayer.CheckIfSuchBodyCanBeAdded(Name, ShortName, MPN);
        switch (rezultat.Status)
        {
            case ItemBodyDBOperationStatus.ObjectWithNameExists:
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, string.Format(Resources.ObjectNameExists, rezultat.itembody.name));
                break;

            case ItemBodyDBOperationStatus.ObjectWithShortNameExists:
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, string.Format(denLanguageResourses.Resources.ObjectShortNameExists, rezultat.itembody.myname));
                break;

            case ItemBodyDBOperationStatus.ObjectWithMpnExists:
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, string.Format(denLanguageResourses.Resources.ObjectMPNExists, rezultat.itembody.mpn));
                break;

            case ItemBodyDBOperationStatus.UnknownError:
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, denLanguageResourses.Resources.UnknownError);
                break;

            case ItemBodyDBOperationStatus.OperationSuccessful:
                Result = GetResult();
                // Teraz możemy poprosić o zamknięcie okna dialogowego
                RequestClose?.Invoke(this, EventArgs.Empty);
                break;

            default:
                await _dialogService.ShowMessage(denLanguageResourses.Resources.ErrorTitle, denLanguageResourses.Resources.UnknownError);
                break;
        }
    }


    private async Task Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private Dictionary<string, object> GetResult()
    {
        return new Dictionary<string, object>
        {
            { "Name", Name },
            { "ShortName", ShortName },
            { "MPN", MPN },
            { "Price", Price },
            { "Weight", Weight },
            { "SelectedSupplier", SelectedSupplier },
            { "SelectedBrand", SelectedBrand },
            { "SelectedType", SelectedType }
        };
    }
}