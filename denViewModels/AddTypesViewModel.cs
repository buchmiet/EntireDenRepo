using denSharedLibrary;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace denViewModels;

public class AddTypesViewModel : ObservableObject, IAsyncDialogViewModel
{
    public event AsyncEventHandler RequestClose;

    public ObservableCollection<TypePickerViewModel> TypesViewModels { get; set; } = new ObservableCollection<TypePickerViewModel>();
    public RelayCommand CancelCommand { get; private set; }
    public RelayCommand OkCommand { get; private set; }

    public List<int> Response;

    public AddTypesViewModel(Dictionary<int,string> _types)
    {
        foreach (var type in _types)
        {
            TypesViewModels.Add(new TypePickerViewModel
            {
                Id = type.Key,
                IsSelected = false,
                Name = type.Value,
            });
        }
        OkCommand= new RelayCommand(ExecuteOkCommand);
        CancelCommand= new RelayCommand(ExecuteCancelCommand);
    }

    private void ExecuteCancelCommand()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private void ExecuteOkCommand()
    {
        Response = TypesViewModels.Where(p=>p.IsSelected).Select(p=>p.Id).ToList();         
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

}