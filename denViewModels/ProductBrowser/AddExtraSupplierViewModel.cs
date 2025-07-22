using denModels;
using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class AddExtraSupplierViewModel : ObservableObject, IAsyncDialogViewModel
{
    public ICommand AssignSupplierCommand { get; private set; }
    public event AsyncEventHandler RequestClose;
    public ICommand CancelCommand { get; private set; }
    private readonly IDialogService _dialogService;

    private Idname _selectedValue;
    public Idname SelectedValue
    {
        get => _selectedValue;
        set => SetProperty(ref _selectedValue, value);
    }

    private List<Idname> _values;
    public List<Idname> Values
    {
        get => _values;
        set => SetProperty(ref _values, value);
    }

    private string _price;
    public string Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    public Tuple<int, string> Result { get; set; }

    public AddExtraSupplierViewModel(List<Idname> _values, IDialogService dialogService)
    {
        _dialogService = dialogService;
        Values = _values;
        SelectedValue = Values.First();
        AssignSupplierCommand = new AsyncRelayCommand(AssignSupplier);
        CancelCommand = new AsyncRelayCommand(Cancel);

    }

    private async Task AssignSupplier()
    {
        if (!double.TryParse(Price, out _))
        {
            await _dialogService.ShowMessage("Error", "Invalid price format");
            return;
        }

        Result = GetResult();

        // Teraz możemy poprosić o zamknięcie okna dialogowego
        RequestClose?.Invoke(this, EventArgs.Empty);

    }

    private async Task Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);

    }

    public Tuple<int, string> GetResult()
    {
        return new Tuple<int, string>(SelectedValue.Id, Price);
    }


}