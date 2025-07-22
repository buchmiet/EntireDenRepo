using System.ComponentModel;

namespace WatchExplorerMVVM.Model;

public class partviewitem : INotifyPropertyChanged
{
    public int id { get; set; }
    public string mpn { get; set; }

  

    private string _available;


    public string available
    {

        get { return _available; }

        set
        {
            if (_available != value)
            {
                _available = value;
                NotifyPropertyChanged("available");
            }
        }

    }
    public string type { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propName)
    {

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

}