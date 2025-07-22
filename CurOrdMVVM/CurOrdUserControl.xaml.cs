

using denViewModels;

using System.Windows;
using System.Windows.Controls;



namespace CurOrdMVVM;

public partial class CurOrdUserControl : UserControl
{    

     
             
    public CurOrdViewModel MyDataContext;

  

    public CurOrdUserControl()
    {
        InitializeComponent();                    
        this.DataContextChanged += CurOrdUserControl_DataContextChanged;
        MyDataContext = (CurOrdViewModel)this.DataContext;
    }



    private void CurOrdUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is CurOrdViewModel)
        {
            MyDataContext = (CurOrdViewModel)e.NewValue;
            
        }
    }

     
}