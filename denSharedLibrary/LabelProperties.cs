namespace denSharedLibrary;

public enum LabelType
{
    ProductLabel,
    ReturnLabel,
    AddressLabel
}
public class LabelProperties
{
    public string LabelName { get; set; }
    public int? TopFont { get; set; }
    public float? LargeFont { get; set; }
    public float? LesserFont { get; set; }
    public float? BottomFont { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int? CentralLineSpacing { get; set; }
    public int? BottomMargin { get; set; }
    public int? TopMargin { get; set; }
    public bool Landscape { get; set; }
    public LabelType LabelType { get; set; }
}