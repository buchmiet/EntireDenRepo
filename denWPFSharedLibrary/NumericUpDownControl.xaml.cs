using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace denWPFSharedLibrary;

/// <summary>
/// Interaction logic for NumericUpDownControl.xaml
/// </summary>
public partial class NumericUpDownControl : UserControl
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDownControl),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public int Value
    {
        get { return (int)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty UpButtonCommandProperty =
        DependencyProperty.Register("UpButtonCommand", typeof(ICommand), typeof(NumericUpDownControl), new PropertyMetadata(null));

    public ICommand UpButtonCommand
    {
        get { return (ICommand)GetValue(UpButtonCommandProperty); }
        set { SetValue(UpButtonCommandProperty, value); }
    }

    public static readonly DependencyProperty DownButtonCommandProperty =
        DependencyProperty.Register("DownButtonCommand", typeof(ICommand), typeof(NumericUpDownControl), new PropertyMetadata(null));

    public ICommand DownButtonCommand
    {
        get { return (ICommand)GetValue(DownButtonCommandProperty); }
        set { SetValue(DownButtonCommandProperty, value); }
    }


    public NumericUpDownControl()
    {
        InitializeComponent();
    }
}