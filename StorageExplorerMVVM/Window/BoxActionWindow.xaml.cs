using System.Windows;

namespace StorageExplorerMVVM;

/// <summary>
/// Interaction logic for BoxActionWindow.xaml
/// </summary>
public partial class BoxActionWindow : Window
{
    public BoxActionWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}