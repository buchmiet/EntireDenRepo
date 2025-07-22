using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;


namespace denViewModels;

public class ColourTextBlockViewModel : ObservableObject
{
    private denSharedLibrary.Color _foregroundColour;
    private string _fontFamily;
    private double _fontSize;
    private denSharedLibrary.Color _backgroundColour;
    private StringBuilder _buffer;

    public denSharedLibrary.Color ForegroundColour
    {
        get => _foregroundColour;
        set => SetProperty(ref _foregroundColour, value);
    }

    public string FontFamily
    {
        get => _fontFamily;
        set => SetProperty(ref _fontFamily, value);
    }

    public double FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    public denSharedLibrary.Color BackgroundColour
    {
        get => _backgroundColour;
        set => SetProperty(ref _backgroundColour, value);
    }

    public StringBuilder Buffer { 
        get { return _buffer; } 
        set {
            _buffer = value;
            StringBuffer = _buffer.ToString();
        }
    }

    public bool CheckIfNotBlank(StringBuilder sb)
    {
        if (sb == null) return false;

        for (int i = 0; i < sb.Length; i++)
        {
            if (sb[i] != ' ') return true;
        }
        return false;
    }

    public  void EnsureLast50Lines()
    {
        int newLineCount = 0;
        int position = Buffer.Length - 1;

        while (position >= 0 && newLineCount < 50)
        {
            if (Buffer[position] == '\n')
            {
                newLineCount++;
            }
            position--;
        }

        if (position > 0)
        {
            Buffer.Remove(0, position + 2); 
        }
    }

    private string _stringBuffer;
        
    public string StringBuffer
    {
        get => _stringBuffer;
        set => SetProperty(ref _stringBuffer, value);
    }
    public ColourTextBlockViewModel()
    {
        Buffer = new StringBuilder();
        StringBuffer = "";

    }

    public void AddString(string wejscie)
    {
        Buffer.Append(wejscie);
        if (CheckIfNotBlank(Buffer))
        {
            StringBuffer = _buffer.ToString();
        }
        
    }

    public void AddChar(char wejscie)
    {
        Buffer.Append(wejscie);
        StringBuffer = _buffer.ToString();
         
    }

    public void AddNewLineChar(char wejscie)
    {
        Buffer.Append(wejscie);
        EnsureLast50Lines();
        StringBuffer = _buffer.ToString();

    }

    public void CursorBlink(char cur)
    {
        if (Buffer.Length > 0)
        {
            Buffer.Remove(Buffer.Length - 1, 1);
        }
        Buffer.Append(cur);
        StringBuffer = _buffer.ToString();

    }

    public void RemoveCursor(char char1, char char2)
    {
        if (Buffer != null)
        {
            char lastChar = Buffer[Buffer.Length - 1];
            if (lastChar == char1 || lastChar == char2)
            {
                  
                Buffer.Remove(Buffer.Length - 1, 1);
                StringBuffer = _buffer.ToString();
            }
        }
    }

    public void FlipCursor(char char1,char char2)
    {
        if (Buffer != null)
        {

            if (Buffer.Length > 0)
            {
                char lastChar = Buffer[Buffer.Length - 1];
                if (lastChar == char2)
                {
                    Buffer.Remove(Buffer.Length - 1, 1);
                }
            }
            Buffer.Append(char1);
            StringBuffer = _buffer.ToString();
               
        }
    }

      


}