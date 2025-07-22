namespace denSharedLibrary;

public class SummaryOrderItem
{
    public int OrderId { get; set; }
    public string orderTxn { get; set; }
    public string market { get; set; }
    public List<SummaryOrderProduct> summaryOrderProduct { get; set; } = new List<SummaryOrderProduct>();
}