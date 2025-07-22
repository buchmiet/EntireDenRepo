using System.Windows;

namespace ProBroMVVM;

/// <summary>
/// Interaction logic for AddProduct.xaml
/// </summary>
public partial class AddNewProductWindow : Window
{
 

    public AddNewProductWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
          
    }

}