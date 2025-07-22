namespace denSharedLibrary;

public class SummaryModel
{
    public List<SummaryOrderItem> SummaryOrderItem { get; set; } = [];
    public Dictionary<int, int> TotalQuantities { get; set; } = [];
    public Dictionary<int, string> ItemLocations { get; set; } = [];
    public Dictionary<int, string> ItemNames { get; set; } = [];
    public Dictionary<int, string> ItemMpns { get; set; } = [];

    public int Tools { get; set; } = 0;

    public List<string> MultipleOrderSummary { get; set; }
}