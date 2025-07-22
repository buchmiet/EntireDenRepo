using DataServicesNET80.DatabaseAccessLayer;
using System.Windows.Controls;

namespace OrdBroMVVM;

public partial class OrdBroUserControl : UserControl
{

    public OrdBroUserControl(IDatabaseAccessLayer databaseAccessLayer)
    {


        InitializeComponent();

    }


}