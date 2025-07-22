using denSharedLibrary;
using System.Reflection;
using System.Windows;

namespace denWPFSharedLibrary;

public class WpfDpiService : IDpiService
{
    public int GetWidth(int resolution, int size)
    {
        var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
        var dpiX = (int)dpiXProperty.GetValue(null, null);
        var inches = size / 25.4;
        return (int)(resolution * inches / dpiX * 96);
    }

    public int GetHeight(int resolution, int size)
    {
        var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
        var dpiY = (int)dpiYProperty.GetValue(null, null);
        var inches = size / 25.4;
        return (int)(resolution * inches / dpiY * 96);
    }
}