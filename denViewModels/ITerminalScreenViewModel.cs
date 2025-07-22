using denSharedLibrary;

namespace denViewModels;

public interface ITerminalScreenViewModel
{   
    bool ScrollToEnd { get; set; }

    void AddString(string text, int color);
    denSharedLibrary.Color GetBorder();
    Theme GetTheme();
    void SetTheme(string theme);
    void NextTheme();
}