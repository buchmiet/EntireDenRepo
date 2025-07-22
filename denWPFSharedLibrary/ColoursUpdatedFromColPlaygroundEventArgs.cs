using System.Windows.Media.Imaging;

namespace denWPFSharedLibrary;

public class ColoursUpdatedFromColPlaygroundEventArgs : EventArgs
{
    public Dictionary<int, BitmapSource> colours { get; private set; }

    public ColoursUpdatedFromColPlaygroundEventArgs(Dictionary<int, BitmapSource> _colours)
    {
        _colours = colours;
    }
}