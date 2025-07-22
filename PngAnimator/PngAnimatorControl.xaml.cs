using denSharedLibrary;
using denViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PngAnimator;

/// <summary>
/// Interaction logic for UserControl1.xaml
/// </summary>
/// 
public partial class PngAnimatorControl : UserControl
{
    public static IDispatcherTimer dispatcherTimer;

    public PngAnimatorControl()
    {
        InitializeComponent();
        DataContext = new PngAnimatorControlViewModel(dispatcherTimer);
    }

    public static readonly DependencyProperty FolderProperty =
        DependencyProperty.Register("Folder", typeof(string), typeof(PngAnimatorControl),
            new PropertyMetadata("", OnFolderChanged));

    public string Folder
    {
        get { return (string)GetValue(FolderProperty); }
        set { SetValue(FolderProperty, value); }
    }

    private static void OnFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as PngAnimatorControl;
        var viewModel = control?.DataContext as PngAnimatorControlViewModel;
        if (viewModel != null)
        {
            viewModel.Folder = (string)e.NewValue;
        }
    }

    public static readonly DependencyProperty FilePatternProperty =
        DependencyProperty.Register("FilePattern", typeof(string), typeof(PngAnimatorControl),
            new PropertyMetadata("", OnFilePatternChanged));

    public string FilePattern
    {
        get { return (string)GetValue(FilePatternProperty); }
        set { SetValue(FilePatternProperty, value); }
    }

    private static void OnFilePatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as PngAnimatorControl;
        var viewModel = control?.DataContext as PngAnimatorControlViewModel;
        if (viewModel != null)
        {
            viewModel.FilePattern = (string)e.NewValue;
        }
    }

    public static readonly DependencyProperty GoingProperty =
        DependencyProperty.Register("Going", typeof(bool), typeof(PngAnimatorControl),
            new PropertyMetadata(false, OnGoingChanged));

    public bool Going
    {
        get { return (bool)GetValue(GoingProperty); }
        set { SetValue(GoingProperty, value); }
    }

    private static void OnGoingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as PngAnimatorControl;
        var viewModel = control?.DataContext as PngAnimatorControlViewModel;
        if (viewModel != null)
        {
            viewModel.Going = (bool)e.NewValue;
        }
    }
}