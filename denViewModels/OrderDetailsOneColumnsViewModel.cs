using denModels;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace denViewModels;

public class OrderDetailsOneColumnsViewModel : ObservableObject
{
    public ObservableCollection<IOrderWidgetViewModel> OneColumnItems { get; set; } = new ObservableCollection<IOrderWidgetViewModel>();
      


    private string _tekst;
    public string Tekst
    {
        get => _tekst;
        set
        {
            SetProperty(ref _tekst, value);
        }
    }

    private int _rows;
    public int Rows
    {
        get => _rows;
        set
        {
            SetProperty(ref _rows, value);
        }
    }

    public OrderDetailsOneColumnsViewModel(List<IOrderWidgetViewModel> _collection, string _tekst)
    {
        Tekst = _tekst;
        var its = _collection.FirstOrDefault(p => p is OrderWidgetViewModelGrid4x4);
        foreach (var item in _collection)
        {
            OneColumnItems.Add(item);
        }
            
    }

}