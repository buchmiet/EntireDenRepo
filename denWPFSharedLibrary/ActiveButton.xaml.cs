using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace denWPFSharedLibrary;

/// <summary>
/// Interaction logic for ActiveButton.xaml
/// </summary>
/// 
public partial class ActiveButton : UserControl
{
    public static readonly DependencyProperty ButtonContentProperty =
        DependencyProperty.Register("ButtonContent", typeof(string), typeof(ActiveButton));

    public string ButtonContent
    {
        get { return (string)GetValue(ButtonContentProperty); }
        set { SetValue(ButtonContentProperty, value); }
    }

    public static readonly DependencyProperty IsButtonEnabledProperty =
        DependencyProperty.Register("IsButtonEnabled", typeof(bool), typeof(ActiveButton));

    public bool IsButtonEnabled
    {
        get { return (bool)GetValue(IsButtonEnabledProperty); }
        set { SetValue(IsButtonEnabledProperty, value); }
    }

    public static readonly DependencyProperty ButtonCommandProperty =
        DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(ActiveButton));

    public ICommand ButtonCommand
    {
        get { return (ICommand)GetValue(ButtonCommandProperty); }
        set { SetValue(ButtonCommandProperty, value); }
    }

    public ActiveButton()
    {
        InitializeComponent();
    }
}