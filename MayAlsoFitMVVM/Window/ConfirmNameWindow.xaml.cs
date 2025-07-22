using System.Windows;

namespace MayAlsoFitMVVM;

/// <summary>
/// Interaction logic for ConfirmNameWindow.xaml
/// </summary>
/// 
public partial class ConfirmNameWindow : Window
{
    public ConfirmNameWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}