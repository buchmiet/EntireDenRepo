using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ColourBrowserMVVM.ViewModel;

public class ColourSchemeViewModel : INotifyPropertyChanged
{
    public ICommand SchemeSelectedCommand { get; set; }
    public BitmapImage Image { get; set; }
    public string Name { get; set; }
    public ColourSchemeViewModel() { }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}