using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class PrintConfirmViewModel : ObservableObject, IAsyncDialogViewModel 
{
    public event AsyncEventHandler RequestClose;
    public bool Result { get; set; }
    private string _adres;
    public string Adres
    {
        get =>_adres;
        set => SetProperty(ref _adres, value);

    }

    public ICommand PrintCommand { get; }
    public ICommand CancelCommand { get; }

    public PrintConfirmViewModel(string adres)
    {
        PrintCommand = new RelayCommand(Print);
        CancelCommand = new RelayCommand(Cancel);
        Adres = adres;
        Result = false;
    }

    private void Print()
    {
        Result = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private void Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

      
}