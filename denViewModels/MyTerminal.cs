using denSharedLibrary;

namespace denViewModels;

public class MyTerminal 
{

    ITerminalScreenViewModel KVM;
    public MyTerminal(ITerminalScreenViewModel _KVM)
    {
        KVM = _KVM;

    }


    public event Action<string> ThemeChanged;


    public void Napisz(string s)
    {

        KVM.AddString(s, 0);
    }
    public void Napisz(string s, int color)
    {
        KVM.AddString(s, color);
    }
    public void NapiszLinie(string s)
    {
        KVM.AddString(s + Environment.NewLine, 0);
    }

    public void NapiszLinie(string s, int color)
    {
        KVM.AddString(s + Environment.NewLine, color);
    }

    public void NapiszLinieKolorowo(string s)
    {
        foreach (char c in s)
        {
            if (Char.IsDigit(c))
            {
                KVM.AddString(c.ToString(), 1);
            }
            else
            {
                KVM.AddString(c.ToString(), 0);
            }
        }
        KVM.AddString(Environment.NewLine, 0);
    }

    public void NapiszKolorowo(string s)
    {
        foreach (char c in s)
        {
            if (Char.IsDigit(c))
            {
                KVM.AddString(c.ToString(), 1);
            }
            else
            {
                KVM.AddString(c.ToString(), 0);
            }
        }

    }

    public void nadpiszLinie(string s)
    {
        //   cb.replaceLastLine(s + Environment.NewLine);
    }

    public string GetTheme()
    {
        return KVM.GetTheme().Name;
    }

    public Color GetBorder()
    {
        return KVM.GetBorder();
    }

    public void GetNextTheme()
    {
        KVM.NextTheme();

        ThemeChanged?.Invoke(KVM.GetTheme().Name);
    }

}