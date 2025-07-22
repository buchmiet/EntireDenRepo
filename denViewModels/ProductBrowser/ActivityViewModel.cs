using CommunityToolkit.Mvvm.ComponentModel;

namespace denViewModels;

public class ActivityViewModel : ObservableObject
{
    private string _taskName;
    public string TaskName
    {
        get => _taskName;
        set => SetProperty(ref _taskName, value);
    }

    private string _currentImage;
    public string CurrentImage
    {
        get => _currentImage;
        set => SetProperty(ref _currentImage, value);
    }

    private string _status;
    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    private bool _tickTack = false;
    public bool TickTack
    {
        get => _tickTack;
        set => SetProperty(ref _tickTack, value);
    }
}