using denSharedLibrary;
using System.Windows;

namespace denWPFSharedLibrary;

public class WpfClipboardService : IClipboardService
{
    public Task<string?> GetTextAsync()=> Task.FromResult(Clipboard.ContainsText() ? Clipboard.GetText() : null);

    public bool TryGettingText(out string text)
    {
        if (Clipboard.ContainsText())
        {
            text = Clipboard.GetText();
            return true;
        }

        text = string.Empty;
        return false;
    }  

}