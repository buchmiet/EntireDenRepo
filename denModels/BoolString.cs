using CommunityToolkit.Mvvm.ComponentModel;

namespace denModels;

public class BoolString : ObservableObject
{
    private bool _tick;
    public bool Tick
    {
        get => _tick;
        set => SetProperty(ref _tick, value);
    }
    private string _name;
    public string Name
    {
        get => _name; set => SetProperty(ref _name, value);
    }
}