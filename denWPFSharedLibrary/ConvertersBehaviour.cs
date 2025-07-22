using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using denSharedLibrary;
using System.IO;
using System.Windows.Media.Imaging;
using denModels;
using Microsoft.Xaml.Behaviors;
using static denSharedLibrary.Colours;

namespace denWPFSharedLibrary;

using System.Windows;


public static class WindowExtensions
{
    public static readonly DependencyProperty SetSizeToContentProperty = DependencyProperty.RegisterAttached(
        "SetSizeToContent",
        typeof(bool),
        typeof(WindowExtensions),
        new PropertyMetadata(false, OnSetSizeToContentChanged));

    public static bool GetSetSizeToContent(Window obj)
    {
        return (bool)obj.GetValue(SetSizeToContentProperty);
    }

    public static void SetSetSizeToContent(Window obj, bool value)
    {
        obj.SetValue(SetSizeToContentProperty, value);
    }

    private static void OnSetSizeToContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Window window && (bool)e.NewValue)
        {
            window.SizeToContent = SizeToContent.WidthAndHeight;
            SetSetSizeToContent(window, false); // Reset
        }
    }
}
public class WindowSizeAndPositionBehavior : Behavior<Window>
{
    public double WindowWidth
    {
        get { return (double)GetValue(WindowWidthProperty); }
        set { SetValue(WindowWidthProperty, value); }
    }

    public static readonly DependencyProperty WindowWidthProperty =
        DependencyProperty.Register("WindowWidth", typeof(double), typeof(WindowSizeAndPositionBehavior),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnWindowSizeChanged));

    public double WindowHeight
    {
        get { return (double)GetValue(WindowHeightProperty); }
        set { SetValue(WindowHeightProperty, value); }
    }

    public static readonly DependencyProperty WindowHeightProperty =
        DependencyProperty.Register("WindowHeight", typeof(double), typeof(WindowSizeAndPositionBehavior),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnWindowSizeChanged));

    private static void OnWindowSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = d as WindowSizeAndPositionBehavior;
        if (behavior?.AssociatedObject != null && !behavior._isUserSizeChange)
        {
            behavior.AssociatedObject.Width = behavior.WindowWidth;
            behavior.AssociatedObject.Height = behavior.WindowHeight;
        }
    }



    private bool _isUserSizeChange = false;

    private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _isUserSizeChange = true;
        WindowWidth = e.NewSize.Width;
        WindowHeight = e.NewSize.Height;
        _isUserSizeChange = false;
    }

    public double WindowTop
    {
        get { return (double)GetValue(WindowTopProperty); }
        set { SetValue(WindowTopProperty, value); }
    }

    public static readonly DependencyProperty WindowTopProperty =
        DependencyProperty.Register("WindowTop", typeof(double), typeof(WindowSizeAndPositionBehavior),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnWindowPositionChanged));

    public double WindowLeft
    {
        get { return (double)GetValue(WindowLeftProperty); }
        set { SetValue(WindowLeftProperty, value); }
    }

    public static readonly DependencyProperty WindowLeftProperty =
        DependencyProperty.Register("WindowLeft", typeof(double), typeof(WindowSizeAndPositionBehavior),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnWindowPositionChanged));

    private static void OnWindowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = d as WindowSizeAndPositionBehavior;
        if (behavior?.AssociatedObject != null && !behavior._isUserPositionChange)
        {
            behavior.AssociatedObject.Top = behavior.WindowTop;
            behavior.AssociatedObject.Left = behavior.WindowLeft;
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
        AssociatedObject.LocationChanged += AssociatedObject_LocationChanged;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
        AssociatedObject.LocationChanged -= AssociatedObject_LocationChanged;
        base.OnDetaching();
    }

    private bool _isUserPositionChange = false;

    private void AssociatedObject_LocationChanged(object sender, EventArgs e)
    {
        _isUserPositionChange = true;
        WindowTop = AssociatedObject.Top;
        WindowLeft = AssociatedObject.Left;
        _isUserPositionChange = false;
    }

}


public class DimensionsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is double width && values[1] is double height)
        {
            return new  MySizeInfo { Width = width, Height = height };
        }
        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

    
public class DisplayedCurrencyToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DisplayedCurrency displayedCurrency)
        {
            return displayedCurrency.ToString() == parameter.ToString();
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value && parameter.ToString() == "MainCurrency" ? DisplayedCurrency.MainCurrency : DisplayedCurrency.OrderCurrency;
    }
}

public class PrintLinesToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is PrintLines printLines)
        {
            return printLines == PrintLines.ForAllSoldProducts ? parameter.ToString() == "ForAllSoldProducts" : parameter.ToString() == "ForOneProduct";
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? parameter.ToString() == "ForAllSoldProducts" ? PrintLines.ForAllSoldProducts : PrintLines.ForOneProduct : Binding.DoNothing;
    }
}

  

public class FontName2FontFamilyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string fileName)
        {
            return "pack://application:,,,/Data/#" + denSharedLibrary.ThemesActions.FileName2FamilyName[fileName];
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class ColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is denSharedLibrary.Color color)
        {
            var r = System.Convert.ToByte(System.Convert.ToInt32(color.R, 16));
            var g = System.Convert.ToByte(System.Convert.ToInt32(color.G, 16));
            var b = System.Convert.ToByte(System.Convert.ToInt32(color.B, 16));
            var a = System.Convert.ToByte(System.Convert.ToInt32(color.A, 16)); // Dodane dla obsługi wartości alfa
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, r, g, b));
        }
        return System.Windows.Media.Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ByteArrayToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is byte[] imageData)
        {
            using (var stream = new MemoryStream(imageData))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                    
                return image;
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class ComboBoxTextChangedBehavior : Behavior<ComboBox>
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(ComboBoxTextChangedBehavior), new PropertyMetadata(string.Empty));

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnComboBoxLoaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= OnComboBoxLoaded;
        if (AssociatedObject.Template.FindName("PART_EditableTextBox", AssociatedObject) is TextBox textBox)
        {
            textBox.TextChanged -= OnTextBoxTextChanged;
        }
        base.OnDetaching();
    }

    private void OnComboBoxLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (AssociatedObject.Template.FindName("PART_EditableTextBox", AssociatedObject) is TextBox textBox)
        {
            textBox.TextChanged += OnTextBoxTextChanged;
            var binding = new Binding("Text") { Source = this, Mode = BindingMode.TwoWay };
            textBox.SetBinding(TextBox.TextProperty, binding);
        }
    }

    private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            Text = textBox.Text;
        }
    }
}


public class SuppressMouseBehavior : Behavior<CheckBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseLeftButtonDown += CheckBox_PreviewMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseLeftButtonDown -= CheckBox_PreviewMouseLeftButtonDown;
        base.OnDetaching();
    }

    private void CheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;

        // Programowo zaznacz/odznacz checkboxa
        CheckBox cb = sender as CheckBox;
        if (cb != null)
        {
            cb.IsChecked = !cb.IsChecked;
        }
    }
}

public class BoolToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isFiltering = (bool)value;
        var columnType = (string)parameter;

        if (columnType == "Left")
        {
            return isFiltering ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        }
        else if (columnType == "Right")
        {
            return isFiltering ? new GridLength(5, GridUnitType.Star) : new GridLength(1, GridUnitType.Star);
        }

        return new GridLength(1, GridUnitType.Star);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}




public class RGBToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RGB rgb)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromRgb(rgb.R, rgb.G, rgb.B));
        }
        return System.Windows.Media.Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public static class ScrollViewerExtensions
{
    public static readonly DependencyProperty ScrollToEndProperty =
        DependencyProperty.RegisterAttached("ScrollToEnd", typeof(bool), typeof(ScrollViewerExtensions), new PropertyMetadata(false, OnScrollToEndChanged));

    public static bool GetScrollToEnd(ScrollViewer obj)
    {
        return (bool)obj.GetValue(ScrollToEndProperty);
    }

    public static void SetScrollToEnd(ScrollViewer obj, bool value)
    {
        obj.SetValue(ScrollToEndProperty, value);
    }

    private static void OnScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ScrollViewer scrollViewer && (bool)e.NewValue)
        {
            scrollViewer.ScrollToEnd();
            SetScrollToEnd(scrollViewer, false); // Reset wartości
        }
    }
}

  



public class MouseControl : ContentControl
{
    public static readonly DependencyProperty MouseEnterCommandProperty = DependencyProperty.Register(
        "MouseEnterCommand", typeof(ICommand), typeof(MouseControl));

    public ICommand MouseEnterCommand
    {
        get { return (ICommand)GetValue(MouseEnterCommandProperty); }
        set { SetValue(MouseEnterCommandProperty, value); }
    }

    public static readonly DependencyProperty MouseLeftButtonDownCommandProperty = DependencyProperty.Register(
        "MouseLeftButtonDownCommand", typeof(ICommand), typeof(MouseControl));

    public ICommand MouseLeftButtonDownCommand
    {
        get { return (ICommand)GetValue(MouseLeftButtonDownCommandProperty); }
        set { SetValue(MouseLeftButtonDownCommandProperty, value); }
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        MouseEnterCommand?.Execute(null);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        MouseLeftButtonDownCommand?.Execute(null);
    }
}

public class MouseDownControl : ContentControl
{
    public static readonly DependencyProperty MouseLeftButtonDownCommandProperty = DependencyProperty.Register(
        "MouseLeftButtonDownCommand", typeof(ICommand), typeof(MouseDownControl));

    public ICommand MouseLeftButtonDownCommand
    {
        get { return (ICommand)GetValue(MouseLeftButtonDownCommandProperty); }
        set { SetValue(MouseLeftButtonDownCommandProperty, value); }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        MouseLeftButtonDownCommand?.Execute(null);
    }
}

    

public class FadeOutBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
        "IsVisible", typeof(bool), typeof(FadeOutBehavior), new PropertyMetadata(true, OnIsVisibleChanged));

    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    private static async void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is FadeOutBehavior behavior))
            return;

        if ((bool)e.NewValue)
        {
            behavior.AssociatedObject.Visibility = Visibility.Visible;
            behavior.AssociatedObject.Opacity = 1;
        }
        else
        {
            await Task.Delay(1500); // wait before starting fade-out animation

            var animation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            behavior.AssociatedObject.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Opacity = IsVisible ? 1 : 0;
    }
}






public class StringStartsWithConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && parameter is string parameterValue)
        {
            return stringValue.StartsWith(parameterValue);
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectionChangedCommandBehavior : Behavior<DataGrid>
{
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(SelectionChangedCommandBehavior), new PropertyMetadata(null));

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += OnSelectionChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Command?.Execute(AssociatedObject.SelectedItem);
    }
}


public class MouseClickBehavior : Behavior<DataGrid>
{
    public ICommand ClickCommand
    {
        get { return (ICommand)GetValue(ClickCommandProperty); }
        set { SetValue(ClickCommandProperty, value); }
    }

    public static readonly DependencyProperty ClickCommandProperty =
        DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(MouseClickBehavior), new PropertyMetadata(null));

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonDown;
    }

    private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        if (dataGrid != null)
        {
            var item = dataGrid.SelectedItem;
            if (item != null && ClickCommand != null && ClickCommand.CanExecute(item))
            {
                ClickCommand.Execute(item);
            }
        }
    }
}


    
public class MouseDoubleClickBehavior : Behavior<DataGrid>
{
    public ICommand DoubleClickCommand
    {
        get { return (ICommand)GetValue(DoubleClickCommandProperty); }
        set { SetValue(DoubleClickCommandProperty, value); }
    }

    public static readonly DependencyProperty DoubleClickCommandProperty =
        DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(MouseDoubleClickBehavior), new PropertyMetadata(null));

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
    }

    private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        if (dataGrid != null)
        {
            var item = dataGrid.SelectedItem;
            if (item != null && DoubleClickCommand != null && DoubleClickCommand.CanExecute(item))
            {
                DoubleClickCommand.Execute(item);
            }
        }
    }
}


public class ArrowKeyCommandBehavior : Behavior<TextBox>
{
    public static readonly DependencyProperty LeftCommandProperty =
        DependencyProperty.Register("LeftCommand", typeof(ICommand), typeof(ArrowKeyCommandBehavior), new PropertyMetadata(null));

    public static readonly DependencyProperty RightCommandProperty =
        DependencyProperty.Register("RightCommand", typeof(ICommand), typeof(ArrowKeyCommandBehavior), new PropertyMetadata(null));

    public ICommand LeftCommand
    {
        get { return (ICommand)GetValue(LeftCommandProperty); }
        set { SetValue(LeftCommandProperty, value); }
    }

    public ICommand RightCommand
    {
        get { return (ICommand)GetValue(RightCommandProperty); }
        set { SetValue(RightCommandProperty, value); }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewKeyDown += HandleKeyDown;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewKeyDown -= HandleKeyDown;
    }

    private void HandleKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Left && LeftCommand != null && LeftCommand.CanExecute(null))
        {
            LeftCommand.Execute(null);
            e.Handled = true;
        }
        else if (e.Key == System.Windows.Input.Key.Right && RightCommand != null && RightCommand.CanExecute(null))
        {
            RightCommand.Execute(null);
            e.Handled = true;
        }
    }
}

public class EnterKeyCommandBehavior : Behavior<TextBox>
{
    public static readonly DependencyProperty EnterCommandProperty =
        DependencyProperty.Register("EnterCommand", typeof(ICommand), typeof(EnterKeyCommandBehavior), new PropertyMetadata(null));



    public ICommand EnterCommand
    {
        get { return (ICommand)GetValue(EnterCommandProperty); }
        set { SetValue(EnterCommandProperty, value); }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewKeyDown += HandleEnterKey;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewKeyDown -= HandleEnterKey;
    }

    private void HandleEnterKey(object sender, KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter && EnterCommand != null && EnterCommand.CanExecute(null))
        {
            EnterCommand.Execute(null);
            e.Handled = true;
        }
    }
}



public class NullToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? new GridLength(0, GridUnitType.Pixel) : new GridLength(1, GridUnitType.Star);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class BooleanToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool width)
        {
            if (width) return new GridLength(1, GridUnitType.Star);
            return new GridLength(0, GridUnitType.Pixel); 
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class StringToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double width)
        {
            if (width > 0)
            {
                return new GridLength(width, GridUnitType.Star);
            }
            return new GridLength(1, GridUnitType.Star);
        }
        return new GridLength(1, GridUnitType.Star);
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is GridLength gridLength)
        {
            return gridLength.Value.ToString();
        }
        return "1";
    }

}



public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NotNullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        var o= (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed; 
        return o;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility && (Visibility)value == Visibility.Visible;
    }
}

public class NotBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        var o= (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
        return o;//(value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility && (Visibility)value == Visibility.Visible;
    }
}


public class MarketToVisibilityConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        var market = value as  Idname;
        if (market == null)
        {
            return Visibility.Collapsed;
        }
        if (parameter.ToString().Equals("1"))
        {
            if (market.Id == 1)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
        if (market.Id == 3 || market.Id == 4 || market.Id == 5 || market.Id == 6 || market.Id == 7 || market.Id == 10 || market.Id == 11)
        {
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}