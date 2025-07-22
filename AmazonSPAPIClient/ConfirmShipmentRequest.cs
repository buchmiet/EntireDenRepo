namespace AmazonSPAPIClient;

public class ConfirmShipmentRequest
{
    public PackageDetail packageDetail { get; set; }
    public string marketplaceId { get; set; }   
}

public class PackageDetail
{
    public string packageReferenceId { get; set; }
    public string carrierCode { get; set; }
    public string carrierName { get; set; }
    public string shippingMethod { get; set; }
    public string trackingNumber { get; set; }
    public string shipDate { get; set; }
    public List<OrderItem> orderItems { get; set; }
}

public class OrderItem
{
    public string orderItemId { get; set; }
    public int quantity { get; set; }
}