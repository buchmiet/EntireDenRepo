namespace denSharedLibrary;

public interface IClipboardService
{
    Task<string?> GetTextAsync();
    bool TryGettingText(out string text);
}