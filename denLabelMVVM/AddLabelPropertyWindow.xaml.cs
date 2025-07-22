using System.Windows;

namespace denLabelMVVM;

/// <summary>
/// Interaction logic for AddLabelProperty.xaml
/// </summary>
public partial class AddLabelPropertyWindow : Window
{
    public AddLabelPropertyWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
    }
}