using System.Windows;

namespace ProBroMVVM;

/// <summary>
/// Interaction logic for ItemEventWindow.xaml
/// </summary>
public partial class ItemEventWindow : Window
{
    public ItemEventWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}