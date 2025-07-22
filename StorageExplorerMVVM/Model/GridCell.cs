using System.ComponentModel;

namespace StorageExplorerMVVM;

public class GridCell : INotifyPropertyChanged
{
    private bool _isActive;
    public int X { get; set; } // Właściwość X (kolumna)
    public int Y { get; set; } // Właściwość Y (rzędy)



    private string _boxedName;
    public string BoxedName
    {
        get => _boxedName;
        set
        {
            _boxedName = value;
            OnPropertyChanged(nameof(BoxedName));
        }
    }

    public bool IsActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            OnPropertyChanged(nameof(IsActive));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}