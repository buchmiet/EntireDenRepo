using System.Windows;

namespace OrdBroMVVM;

/// <summary>
/// Interaction logic for printConfirm.xaml
/// </summary>
public partial class PrintConfirmWindow : Window
{

    //string zwrotka;


    //public string Response
    //{
    //    get { return zwrotka; }

    //}

    public PrintConfirmWindow()
    {
        InitializeComponent();
        //adres.Text = adr;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;

    }

    //private void Button_Click(object sender, RoutedEventArgs e)
    //{
    //    DialogResult = false;
    //}

    //private async void Button_Click_1(object sender, RoutedEventArgs e)
    //{
    //    zwrotka = adres.Text.TrimStart();
    //    DialogResult = true;
    //}




}