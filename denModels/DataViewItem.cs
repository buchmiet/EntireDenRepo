
using CommunityToolkit.Mvvm.ComponentModel;

namespace denModels;

public class DataViewItem : ObservableObject
{



    string _market;
    string _buyer;
    bool _returning;

    string _tool;
    string _zone;
    string _country;
    double _postage;
    string _status;

    public string curSymbol { get; set; }

    decimal _price;

    private int _index;
    public int Index
    {
        get => _index;
        set => SetProperty(ref _index, value);
    }

    public string PriceDisplay
    {
        get => curSymbol + _price.ToString("F2");

    }

    public string PostageDisplay
    {
        get
        {
            if (_postage == 0)
            {
                return "FREE";
            }
            else
            {
                return curSymbol + _postage.ToString("F2");
            }
        }

    }

    private DateTime? _arrived;
    public DateTime? Arrived
    {
        get => _arrived;
        set
        {
            if (SetProperty(ref _arrived, value))
            {
                OnPropertyChanged(nameof(ArrivedDisplay));
            }
        }
    }

    public string ArrivedDisplay
    {
        get
        {
            if (!_arrived.HasValue) return "";

            var now = DateTime.Now;
            var arrived = _arrived.Value;

            if (arrived.Date < now.Date.AddDays(-1))
            {
                return "<Y " + arrived.ToString("HH:mm");
            }

            if (arrived.Date < now.Date)
            {
                return "Y " + arrived.ToString("HH:mm");
            }

            return arrived.ToString("HH:mm");
        }
    }

    private bool _printMe;
    public bool PrintMe
    {
        get => _printMe;
        set
        {
            if (SetProperty(ref _printMe, value))
            {
                PrintMeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public string Market
    {
        get => _market;
        set => SetProperty(ref _market, value);
    }

    public string Buyer
    {
        get => _buyer;
        set => SetProperty(ref _buyer, value);
    }

    public bool Returning
    {
        get => _returning;
        set => SetProperty(ref _returning, value);
    }

    private ProductLine[] _items2;

    public ProductLine[] Items2
    {
        get => _items2;
        set => SetProperty(ref _items2, value);
    }

    public event EventHandler PrintMeChanged;


    public string[] Items { get; set; }


    public string[] Quantities { get; set; }
    public string PostCode { get; set; }
    public string Tracking { get; set; }
    public int orderid { get; set; }
    public string Tool
    {
        get => _tool;
        set => SetProperty(ref _tool, value);
    }


    public string Zone
    {
        get => _zone;
        set => SetProperty(ref _zone, value);
    }


    public string Country
    {
        get => _country;
        set => SetProperty(ref _country, value);
    }

    public double Postage
    {
        get => _postage;
        set
        {
            SetProperty(ref _postage, value);
            OnPropertyChanged(nameof(PostageDisplay));
        }
    }



    public string tag { get; set; }
    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }


    public decimal Price
    {
        get => _price;
        set
        {
            if (SetProperty(ref _price, value))
            {
                OnPropertyChanged(nameof(PriceDisplay));
            }
        }
    }

    public byte[] Flag { get; set; }


}