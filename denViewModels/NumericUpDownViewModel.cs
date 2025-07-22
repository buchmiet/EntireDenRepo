using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class NumericUpDownViewModel : ObservableRecipient
{
    private int _value;
    public int Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public RelayCommand IncrementCommand => new RelayCommand(() => Value++);
    public RelayCommand DecrementCommand => new RelayCommand(() => Value--);
}