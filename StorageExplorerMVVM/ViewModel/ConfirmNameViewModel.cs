using denSharedLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace StorageExplorerMVVM.ViewModel;

public class ConfirmNameViewModel : INotifyPropertyChanged, IAsyncDialogViewModel
{
    private string _name;
    public bool Response = false;
    public event AsyncEventHandler RequestClose;
    private readonly IDialogService _dialogService;

    public int NumberOfButtons;



    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }


    public ICommand ConfirmCommand { get; set; }
    public ICommand CancelCommand { get; set; }
    List<string> ForbiddenNames;


    public ConfirmNameViewModel(IDialogService dialogService, List<string>? _forbiddenNames = null, string? name = null)
    {


        ConfirmCommand = new RelayCommand(ConfirmOperation);
        CancelCommand = new RelayCommand(CancelOperation);
        _dialogService = dialogService;
        if (!string.IsNullOrEmpty(name))
        {
            Name = name;
        }
        if (_forbiddenNames != null)
        {
            ForbiddenNames = _forbiddenNames;
        }
    }



    private void ConfirmOperation()
    {
        if (string.IsNullOrEmpty(Name))
        {
            _dialogService.ShowMessage("Error", "Please enter a valid name");
        }
        else
        {
            if (ForbiddenNames.Any(p => p.Equals(Name.ToLower())))
            {
                _dialogService.ShowMessage("Error", "This name is already in use, choose different name");
            }
            else
            {
                Response = true;
                RequestClose?.Invoke(this, EventArgs.Empty);
            }


        }
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