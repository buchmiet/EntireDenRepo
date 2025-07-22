using denSharedLibrary;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class ConfirmDecimalValueViewModel : ObservableObject, IAsyncDialogViewModel, IDataErrorInfo
{

    public string Error => string.Empty;

    public string this[string propertyName]
    {
        get
        {
            if (propertyName == nameof(entry))
            {
                if (!decimal.TryParse(_entry, out decimal parsedValue))
                {
                    return denLanguageResourses.Resources.ERRORDecimalNumberIncorrect;
                }
                if (parsedValue <= 0)
                {
                    return denLanguageResourses.Resources.ERRORNumberMustBeGreaterThanZero;
                }
            }
            return string.Empty;
        }
    }

    private string _entry;
    public string entry
    {
        get => _entry;
        set => SetProperty(ref _entry, value);
    }

    public decimal Result;

    public ICommand CancelCommand { get; set; }
    public ICommand OkCommand { get; set; }

 

    public ConfirmDecimalValueViewModel(decimal entry)
    {
        _entry = entry.ToString();
        OkCommand = new RelayCommand(OkExecute);
        CancelCommand = new AsyncRelayCommand(CancelExecute);
    }

    public event AsyncEventHandler RequestClose;

    public class ConfirmDecimalValueEventArgs : EventArgs
    {
        public decimal Result { get; private set; }

        public ConfirmDecimalValueEventArgs(decimal result)
        {
            Result = result;
        }
    }


    public void OkExecute()
    {
          
        RequestClose?.Invoke(this, new ConfirmDecimalValueEventArgs(Convert.ToDecimal(entry)));
    }

    public async Task CancelExecute()
    {
           
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

}