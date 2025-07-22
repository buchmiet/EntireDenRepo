using CommunityToolkit.Mvvm.ComponentModel;

namespace denModels;

public class ProductViewItem : ObservableObject
{
    private int _id;
    private int _number;
    private int _quantity;
    private string _fullName;
    private string _mpn;
    private bool _readyToTrack;
    private bool _assigned;
    private string _myName;
    private string _notes;
    private string _brand;
    private string _type;
    private string _locatedAt;
    private int _weight;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public int Number
    {
        get => _number;
        set => SetProperty(ref _number, value);
    }

    public int Weight
    {
        get => _weight;
        set => SetProperty(ref _weight, value);
    }

    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Mpn
    {
        get => _mpn;
        set => SetProperty(ref _mpn, value);
    }

    public bool ReadyToTrack
    {
        get => _readyToTrack;
        set => SetProperty(ref _readyToTrack, value);
    }

    public bool Assigned
    {
        get => _assigned;
        set => SetProperty(ref _assigned, value);
    }

    public string MyName
    {
        get => _myName;
        set => SetProperty(ref _myName, value);
    }

    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public string Brand
    {
        get => _brand;
        set => SetProperty(ref _brand, value);
    }

    public string Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    public string LocatedAt
    {
        get => _locatedAt;
        set => SetProperty(ref _locatedAt, value);
    }

    
}