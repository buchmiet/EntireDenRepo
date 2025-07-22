using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class YesNoViewModel : ObservableObject, IAsyncDialogViewModel
{

    public event AsyncEventHandler RequestClose;
   

    private bool _response;
    public bool  Response
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

    public YesNoViewModel(string title, string tekst,string sss)
    {
        Tekst= tekst;
        Title= title;
    }

    public ICommand NoCommand => new RelayCommand(NoClick);

       
    public ICommand YesCommand => new RelayCommand(YesClick);



    private void NoClick()
    {
        Response = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private void YesClick()
    {
        Response = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
      
}