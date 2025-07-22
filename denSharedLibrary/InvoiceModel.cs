namespace denSharedLibrary;

public class InvoiceModel
{
    public string MarketName { get; set; }
    public int InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    private decimal _total = 0;
    public decimal Total { get => Math.Round(_total, 2); set => _total = value; }
    private decimal _vatRate = 0;
    public decimal VatRate
    {
        get => Math.Round(_vatRate, 2);
        set => _vatRate = value;
    }

    public bool IsVat { get; set; }
    public string CurrencySymbol { get; set; }

    private decimal _priceNet = 0;
    public decimal PriceNet
    {
        get => Math.Round(_priceNet, 2);
        set => _priceNet = value;
    }
    private decimal _vATpaid = 0;
    public decimal VATpaid
    {
        get => Math.Round(_vATpaid, 2);
        set => _vATpaid = value;
    }

    public string AddressAsAString { get; set; }

    public List<QPDFOrderItems> Items { get; set; }
}