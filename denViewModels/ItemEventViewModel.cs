using CommunityToolkit.Mvvm.ComponentModel;
using denSharedLibrary;

namespace denViewModels;

public class ItemEventViewModel : ObservableObject, IAsyncDialogViewModel
{

    private readonly IDialogService _dialogService;

    private List<ItemEvent> _events;
    public List<ItemEvent> Events
    {
        get => _events;
        set => SetProperty(ref _events, value);
    }



    public bool DialogResult { get; set; }
    public Tuple<int, string> Result { get; set; }

    public ItemEventViewModel(List<ItemEvent> _eventy, IDialogService dialogService)
    {
        _dialogService = dialogService;
        _events = _eventy;

    }



    private async Task Cancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);

    }

    public Tuple<int, string> GetResult()
    {
        return null;
    }

     
    public event AsyncEventHandler RequestClose;

     
}