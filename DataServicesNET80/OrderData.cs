namespace DataServicesNET80;

public class orderData
{
    public int orderId { get; set; }
    public decimal Total { get; set; }
    public decimal NetTotal { get; set; }
    public string CountryCode { get; set; }
    public bool IsVat { get; set; }
    public int MarketId { get; set; }
    public DateTime PaidOn { get; set; }
}