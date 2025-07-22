using CommunityToolkit.Mvvm.ComponentModel;

namespace denModels;

public class ProductLine : ObservableObject
{
    private byte[] _pic;

    public byte[] pic
    {
        get => _pic;
        set => SetProperty(ref _pic, value);

    }

    public int colourId { get; set; }
    public string name { get; set; }
    public int qua { get; set; }

    public int itembodyid { get; set; }
}