using CommunityToolkit.Mvvm.ComponentModel;

namespace denModels;

public class ItemQuantity : ObservableObject
{
      


    private Idname _idname;
    public Idname Idname
    {
        get => _idname;
        set => SetProperty(ref _idname, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

}