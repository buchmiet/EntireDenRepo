using System.Windows;

namespace denWPFSharedLibrary;

public partial class ProductBrowserWindow : Window
{
       

    public ProductBrowserWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
            
    }

       
}