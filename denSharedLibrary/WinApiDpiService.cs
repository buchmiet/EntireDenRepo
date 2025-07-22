using System.Runtime.InteropServices;

namespace denSharedLibrary;

public class WinApiDpiService : IDpiService
{
    [DllImport("gdi32.dll")]
    public static extern int GetDeviceCaps(IntPtr hDc, int nIndex);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

    public const int LOGPIXELSX = 88;
    public const int LOGPIXELSY = 90;

    public int GetWidth(int resolution, int size)
    {
        TransformToPixels(size, size, out int pixelX, out int _);
        return (int)(resolution * pixelX / 96.0);
    }

    public int GetHeight(int resolution, int size)
    {
        TransformToPixels(size, size, out int _, out int pixelY);
        return (int)(resolution * pixelY / 96.0);
    }

    public void TransformToPixels(double unitX, double unitY, out int pixelX, out int pixelY)
    {
        IntPtr hDc = GetDC(IntPtr.Zero);
        if (hDc != IntPtr.Zero)
        {
            int dpiX = GetDeviceCaps(hDc, LOGPIXELSX);
            int dpiY = GetDeviceCaps(hDc, LOGPIXELSY);

            ReleaseDC(IntPtr.Zero, hDc);

            pixelX = (int)((dpiX / 96.0) * unitX);
            pixelY = (int)((dpiY / 96.0) * unitY);
        }
        else
        {
            throw new ArgumentNullException("Failed to get DC.");
        }
    }
}