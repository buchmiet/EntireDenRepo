using System.Windows;

namespace StorageExplorerMVVM;

/// <summary>
/// Interaction logic for AddSeveralWindow.xaml
/// </summary>
public partial class AddSeveralWindow : Window
{
    public AddSeveralWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}