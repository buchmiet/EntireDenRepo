using denSharedLibrary;
using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace CasioUKDeliveriesMVVM;

public class QuantityViewModel : INotifyPropertyChanged, IAsyncDialogViewModel
{
    private int _response;
    private string _title;
    private string _quantity;

    public int Response
    {
        get { return _response; }
        set
        {
            _response = value;
            OnPropertyChanged(nameof(Response));
        }
    }

    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public string Quantity
    {
        get { return _quantity; }
        set
        {
            _quantity = value;
            OnPropertyChanged(nameof(Quantity));
            CheckValidNumber();
        }
    }

    public ICommand CancelCommand { get; set; }
    public ICommand ConfirmCommand { get; set; }

    public QuantityViewModel(int quan)
    {
        CancelCommand = new RelayCommand(CancelOperation);
        ConfirmCommand = new RelayCommand(ConfirmOperation);
        Response = -1;
        Quantity = quan.ToString();
    }

    private void CancelOperation()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private void ConfirmOperation()
    {
        if (int.TryParse(Quantity, out _response))
        {
            if (_response < 1)
            {
                MessageBox.Show("Enter value larger than 0");
            }
            else
            {
                Response = Convert.ToInt32(Quantity);
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            MessageBox.Show("Enter correct numeric value");
        }
    }


    private void CheckValidNumber()
    {
        if (!int.TryParse(Quantity, out int number))
        {
            MessageBox.Show("Enter correct numeric value");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public event AsyncEventHandler RequestClose;
}