using System.Windows;

namespace CasioUKDeliveriesMVVM;

/// <summary>
/// Interaction logic for QuantityWindow.xaml
/// </summary>
public partial class QuantityWindow :System.Windows. Window
{
    public QuantityWindow()
    {
            

        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}