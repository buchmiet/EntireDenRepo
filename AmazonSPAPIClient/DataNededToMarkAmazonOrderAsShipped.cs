namespace AmazonSPAPIClient;

public class DataNededToMarkAmazonOrderAsShipped
{
    public int Orderid { get; set; }
    public string AmazonOrderId { get; set; }
    public string Tracking { get; set; }
    public string MarketPlaceId { get; set; }
}