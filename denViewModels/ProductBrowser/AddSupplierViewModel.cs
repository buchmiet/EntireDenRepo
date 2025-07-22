using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class AddSupplierViewModel : ObservableObject, IAsyncDialogViewModel
{
    public ICommand OkCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public event AsyncEventHandler RequestClose;
    private readonly IDialogService _dialogService;

    private string _newSupplier;
    public string NewSupplier
    {
        get => _newSupplier;
        set => SetProperty(ref _newSupplier, value);
    }

    //public bool DialogResult { get; set; }
    public string Result { get; set; }

    public AddSupplierViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        OkCommand = new AsyncRelayCommand(AddSupplier);
        CancelCommand = new AsyncRelayCommand(Cancel);
    }

    private async Task AddSupplier()
    {
        if (string.IsNullOrWhiteSpace(NewSupplier))
        {
            await _dialogService.ShowMessage("Error", "You need to provide a name of the supplier");
            return;
        }

        Result = NewSupplier;

        // Teraz możemy poprosić o zamknięcie okna dialogowego
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private async Task Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }


}