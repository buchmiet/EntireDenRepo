using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class MessageBoxViewModel:ObservableObject, IAsyncDialogViewModel
{
    public event AsyncEventHandler RequestClose;


    private bool _response;
    public bool Response
    {
        get => _response;
        set => SetProperty(ref _response, value);
    }

    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            SetProperty(ref _title, value);
        }
    }

    private string _tekst;
    public string Tekst
    {
        get => _tekst;
        set
        {
            SetProperty(ref _tekst, value);
        }
    }

    public MessageBoxViewModel(string title, string tekst)
    {
        Tekst = tekst;
        Title = title;
    }

    public ICommand OKCommand => new RelayCommand(OKClick);


       



    private void OKClick()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

       

}