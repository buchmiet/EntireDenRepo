using System.Windows;

namespace ProBroMVVM;

/// <summary>
/// Interaction logic for AddMarket.xaml
/// </summary>
public partial class AddMarketWindow : Window
{

    public AddMarketWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;

    }
           
}