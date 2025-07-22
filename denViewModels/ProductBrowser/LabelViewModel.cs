using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace denViewModels;

public class LabelViewModel : ObservableObject
{
    private string _tekst;
    public string Tekst
    {
        get => _tekst;
        set => SetProperty(ref _tekst, value);
    }

    public int Id;


    public ICommand RemoveCommand { get; set; }

    public LabelViewModel()
    {

        //   RemoveCommand = new RelayCommand(UsunEtykiete);
    }

    private void UsunEtykiete()
    {
        // Logika usuwania
    }
}