using System.Windows;

namespace ProBroMVVM;

/// <summary>
/// Interaction logic for LaunchTracking.xaml
/// </summary>
public partial class LaunchTrackingWindow : Window
{

     



    public LaunchTrackingWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
        SizeToContent = SizeToContent.WidthAndHeight;

    }

      
}