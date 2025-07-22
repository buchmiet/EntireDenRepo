using denModels;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace denViewModels;

public class OrderWidgetViewModelGrid4x4 : ObservableObject, IOrderWidgetViewModel
{


    public class RowItem
    {
        public string Left { get; set; }
        public string Right { get; set; }
    }

    public ObservableCollection<string> LeftColumn { get; set; }=new ObservableCollection<string>();
    public ObservableCollection<string> RightColumn { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<RowItem> Rows { get; set; } = new ObservableCollection<RowItem>();

    public OrderWidgetViewModelGrid4x4(List<string> leftColumn, List<string> rightColumn)
    {
        LeftColumn = new ObservableCollection<string>(leftColumn);
        RightColumn = new ObservableCollection<string>(rightColumn);
        UpdateRows();
    }
    private void UpdateRows()
    {
        Rows.Clear();
        int count = Math.Min(LeftColumn.Count, RightColumn.Count);
        for (int i = 0; i < count; i++)
        {
            Rows.Add(new RowItem { Left = LeftColumn[i], Right = RightColumn[i] });
        }
    }
}