using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MayAlsoFitMVVM.ViewModel;

public class WatchesAndBodiesGroupsViewModel : ObservableObject
{
    private int _id;
    private string _name;
    private bool _assignButton;
    private bool _removeButton;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public bool AssignButton
    {
        get => _assignButton;
        set => SetProperty(ref _assignButton, value);
    }

    public bool RemoveButton
    {
        get => _removeButton;
        set => SetProperty(ref _removeButton, value);
    }

    
    public AsyncRelayCommand AssignCommand { get; set; }
    public AsyncRelayCommand RemoveCommand { get; set; }
}