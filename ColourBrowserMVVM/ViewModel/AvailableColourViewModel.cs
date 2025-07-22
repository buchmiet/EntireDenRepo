using System.ComponentModel;
using System.Windows.Input;
using static denSharedLibrary.Colours;

namespace ColourBrowserMVVM.ViewModel;

public class AvailableColourViewModel : INotifyPropertyChanged
{
    private RGB _color;

    public event PropertyChangedEventHandler? PropertyChanged;
    public RGB OldColor { get; set; }

    public RGB Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
    }

    public int ColourId { get; set; }

    public ICommand ColourSelectedCommand { get; set; }

    public AvailableColourViewModel()
    {
           
    }

   

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}