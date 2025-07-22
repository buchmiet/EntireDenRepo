using denSharedLibrary;
using System;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace CasioUKDeliveriesMVVM;

public class XLSXViewModel : INotifyPropertyChanged, IAsyncDialogViewModel
{
    
    private string _xelx;
    public bool Response = false;
    public event AsyncEventHandler RequestClose;

    private bool _cancelButtonActive;
    public int NumberOfButtons;

    public bool CancelButtonActive
    {
        get { return _cancelButtonActive; }
        set
        {
            _cancelButtonActive = value;
            if (value)
            {
                NumberOfButtons = 2;
            }
            else
            {
                NumberOfButtons = 1;
            }

            OnPropertyChanged(nameof(NumberOfButtons));
            OnPropertyChanged(nameof(CancelButtonActive));
        }
    }

    public string Xelx
    {
        get { return _xelx; }
        set
        {
            _xelx = value;
            OnPropertyChanged(nameof(Xelx));
        }
    }

    
    public ICommand ConfirmCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    public XLSXViewModel(string text,bool _cancelButtonActive2)
    {
        Xelx = text;
        CancelButtonActive = _cancelButtonActive2;
        ConfirmCommand = new RelayCommand(ConfirmOperation);
        CancelCommand = new RelayCommand(CancelOperation);
    }

       

    private void ConfirmOperation()
    {
        Response = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
    private void CancelOperation()
    {
        Response = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
       
}