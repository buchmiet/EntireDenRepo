using System.Xml.Serialization;

namespace denSharedLibrary;

[XmlRoot("themes")]
public class Themes
{
    [XmlElement("theme")]
    public List<Theme> ThemesList { get; set; }
}

public class Theme
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlElement("ready")]
    public string Ready { get; set; }

    [XmlElement("cursoron")]
    public int CursorOnInt { get; set; }

    [XmlElement("cursoroff")]
    public int CursorOffInt { get; set; }

    [XmlIgnore] // Ignoruj tę właściwość podczas serializacji/deserializacji
    public char CursorOn
    {
        get { return (char)CursorOnInt; }
    }

    [XmlIgnore] // Ignoruj tę właściwość podczas serializacji/deserializacji
    public char CursorOff
    {
        get { return (char)CursorOffInt; }
    }

    [XmlElement("foregroundColours")]
    public ForegroundColours ForegroundColours { get; set; }

    [XmlElement("background")]
    public Color Background { get; set; }

    [XmlElement("border")]
    public Color Border { get; set; }

    [XmlElement("fontfile")]
    public string FontFile { get; set; }

    //[XmlElement("fontfamily")]
    //public string FontFamily { get; set; }
    //<fontfamily>Px437 ATI 9x16</fontfamily>
    //    	<fontfamily>Px437 ATI 9x16</fontfamily>
    //    	<fontfamily>Pet Me 64</fontfamily>
    //    <fontfamily>Px437 IBM CGAthin</fontfamily>
    //    <fontfamily>ZX82 System</fontfamily>
    //   <fontfamily>IntelOne Mono</fontfamily>

    [XmlElement("fontsize")]
    public int FontSize { get; set; }

    [XmlElement("fontsizeX")]
    public int FontSizeX { get; set; }

    [XmlElement("fontsizeY")]
    public int FontSizeY { get; set; }
}

public class ForegroundColours
{
    [XmlElement("color")]
    public List<Color> Colors { get; set; }
}

public class Color
{
    [XmlAttribute("r")]
    public string R { get; set; }

    [XmlAttribute("g")]
    public string G { get; set; }

    [XmlAttribute("b")]
    public string B { get; set; }
    [XmlAttribute("a")]
    public string A { get; set; } = "FF"; // domyślnie ustawione na FF (nieprzezroczyste)
}