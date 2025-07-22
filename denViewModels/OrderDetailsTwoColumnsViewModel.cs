using denModels;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace denViewModels;

public class OrderDetailsTwoColumnsViewModel:ObservableObject
{
    public ObservableCollection<IOrderWidgetViewModel> LeftColumnItems { get; set; } = new ObservableCollection<IOrderWidgetViewModel>();
    public ObservableCollection<IOrderWidgetViewModel> RightColumnItems { get; set; } = new ObservableCollection<IOrderWidgetViewModel>();
      

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

    public OrderDetailsTwoColumnsViewModel(List<IOrderWidgetViewModel> _collection,string _tekst)
    {
        Tekst = _tekst;
        var its=_collection.FirstOrDefault(p=>p is OrderWidgetViewModelGrid4x4);
        if ( its != null )
        {
            var x4x= its as OrderWidgetViewModelGrid4x4;
            int extra = x4x.LeftColumn.Count;
            Rows=-( extra/2);
        }

          
        Rows+= (_collection.Count / 2) + (_collection.Count % 2);
        foreach (var item in _collection.Take(Rows))
        {
            LeftColumnItems.Add(item);
        }        
        foreach (var item in _collection.Skip(Rows))
        {
            RightColumnItems.Add(item);
        }
    }

}