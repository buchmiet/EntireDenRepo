using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class ValueViewModel:ObservableObject
{
    private string _nazwa;
    public string Nazwa
    {
        get => _nazwa;
        set => SetProperty(ref _nazwa, value);
    }

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private int _pos;
    public int Pos
    {
        get => _pos;
        set => SetProperty(ref _pos, value);
    }

    //private int _ilo;
    //public int Ilo
    //{
    //    get => _ilo;
    //    set => SetProperty(ref _ilo, value);
    //}

    public AsyncRelayCommand RemoveCommand { get; set; }
    public AsyncRelayCommand MoveUpCommand { get; set; }
    public AsyncRelayCommand MoveDownCommand { get; set; }
    private int _max;
    public int Max
    {
        get => _max;
        set
        {
            SetProperty(ref _max, value);
            OnPropertyChanged(nameof(IsMoveUpEnabled));
            OnPropertyChanged(nameof(IsMoveDownEnabled));
        }
    }

    public bool IsMoveUpEnabled => Pos > 0;

    public bool IsMoveDownEnabled => Pos < Max - 1;

    public ValueViewModel()
    {

        MoveUpCommand = new AsyncRelayCommand(MoveUp, () => Pos > 0);
        MoveDownCommand = new AsyncRelayCommand(MoveDown, () => Pos < Max - 1);
    }

    private void Remove()
    {
        // Logika usuwania
    }

    private async Task MoveUp()
    {
        // Logika przesuwania w górę
    }

    private async Task MoveDown()
    {
        // Logika przesuwania w dół
    }


}