using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StorageExplorerMVVM;

public class CellViewModel : ObservableRecipient
{
    private System.Windows.Media.Brush _backgroundColor;

    public System.Windows.Media.Brush BackgroundColor
    {
        get { return _backgroundColor; }
        set { SetProperty(ref _backgroundColor, value); }
    }

    public int Id { get; }  // numer identyfikacyjny

    private int _rowIndex;
    public int RowIndex
    {
        get => _rowIndex;
        set => SetProperty(ref _rowIndex, value);
    }

    private int _columnIndex;
    public int ColumnIndex
    {
        get => _columnIndex;
        set => SetProperty(ref _columnIndex, value);
    }

    public ICommand MouseEnterCommand { get; }
    public ICommand MouseLeftButtonDownCommand { get; }

    public CellViewModel(int id, Action<int> mouseEnterAction, Action<int> mouseDownAction)
    {
        Id = id;
        MouseEnterCommand = new RelayCommand(() => mouseEnterAction(Id));
        MouseLeftButtonDownCommand = new RelayCommand(() => mouseDownAction(Id));
    }
}