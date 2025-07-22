using System.Windows;

namespace denWPFSharedLibrary;

/// <summary>
/// Interaction logic for ConfirmValueWindow.xaml
/// </summary>
public partial class ConfirmDecimalValueWindow : Window
{
    public ConfirmDecimalValueWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
        SizeToContent = SizeToContent.WidthAndHeight;
    }
}