using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace denWPFSharedLibrary;

/// <summary>
/// Interaction logic for ActiveBorderListView.xaml
/// </summary>
public partial class ActiveBorderListView : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        "ItemsSource", typeof(IEnumerable), typeof(ActiveBorderListView), new PropertyMetadata(default(IEnumerable)));

    public IEnumerable ItemsSource
    {
        get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty Column1WidthProperty = DependencyProperty.Register(
        "Column1Width", typeof(GridLength), typeof(ActiveBorderListView), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

    public GridLength Column1Width
    {
        get { return (GridLength)GetValue(Column1WidthProperty); }
        set { SetValue(Column1WidthProperty, value); }
    }

    public static readonly DependencyProperty Column2WidthProperty = DependencyProperty.Register(
        "Column2Width", typeof(GridLength), typeof(ActiveBorderListView), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

    public GridLength Column2Width
    {
        get { return (GridLength)GetValue(Column2WidthProperty); }
        set { SetValue(Column2WidthProperty, value); }
    }

    public static readonly DependencyProperty ChangedBorderColorProperty = DependencyProperty.Register(
        "ChangedBorderColor", typeof(Brush), typeof(ActiveBorderListView), new PropertyMetadata(Brushes.Green));

    public Brush ChangedBorderColor
    {
        get { return (Brush)GetValue(ChangedBorderColorProperty); }
        set { SetValue(ChangedBorderColorProperty, value); }
    }

    public ActiveBorderListView()
    {
        InitializeComponent();
    }
}