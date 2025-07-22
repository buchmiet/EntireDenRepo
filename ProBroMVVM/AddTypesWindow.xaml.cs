using System.Windows;

namespace ProBroMVVM;

/// <summary>
/// Interaction logic for AddTypesWindow.xaml
/// </summary>
public partial class AddTypesWindow : Window
{
    public AddTypesWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}