using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class CountryViewModel : ObservableObject
{   
    private bool _isChecked;
    private string _countryName;

    public CountryViewModel(string countryName)
    {
        _countryName = countryName;          
    }

    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }

    public string CountryName
    {
        get => _countryName;
        set => SetProperty(ref _countryName, value);
    }

    public RelayCommand RemoveCommand { get; set; }
}