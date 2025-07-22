using CommunityToolkit.Mvvm.ComponentModel;

namespace denViewModels;

public class TypePickerViewModel: ObservableObject
{
    public int Id;

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            SetProperty(ref _isSelected, value);
        }
    }
    private string _name;
    public string Name {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
        }
    }
}