using System.Windows;

namespace ProBroMVVM;

/// <summary>
/// Interaction logic for AddSupplierWindow.xaml
/// </summary>
public partial class AddSupplierWindow : Window
{
    public AddSupplierWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }

      
}