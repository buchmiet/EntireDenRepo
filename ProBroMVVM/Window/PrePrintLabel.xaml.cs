using System.Windows;

namespace ProBroMVVM;

/// <summary>
/// Interaction logic for PrePrintLabel.xaml
/// </summary>
public partial class PrePrintLabelWindow : Window
{
    public PrePrintLabelWindow()
    {
        InitializeComponent();
          
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}