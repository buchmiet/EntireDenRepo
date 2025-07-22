using System.Windows;

namespace CasioUKDeliveriesMVVM;

/// <summary>
/// Interaction logic for XLXSWindow.xaml
/// </summary>
public partial class XLSXWindow : System.Windows.Window
{
    public XLSXWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}