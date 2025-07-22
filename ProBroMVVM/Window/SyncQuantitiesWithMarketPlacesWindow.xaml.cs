using System.Windows;

namespace ProBroMVVM;

/// <summary>
/// Interaction logic for SyncQuantitiesWithMarketPlacesWindow.xaml
/// </summary>
public partial class SyncQuantitiesWithMarketPlacesWindow : Window
{
    public SyncQuantitiesWithMarketPlacesWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
        SizeToContent = SizeToContent.WidthAndHeight;
    }
}