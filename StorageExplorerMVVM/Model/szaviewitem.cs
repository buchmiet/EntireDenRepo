using CommunityToolkit.Mvvm.ComponentModel;

namespace StorageExplorerMVVM;

public class Szaviewitem : ObservableObject
{
    private int _id;
    private string _name;
    private string _dims;
    public int Columns { get; set; }
    public int Rows { get; set; }
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

    public string Dims
    {
        get => _dims;
        set => SetProperty(ref _dims, value);
    }
}