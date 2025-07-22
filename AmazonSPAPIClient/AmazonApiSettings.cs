namespace AmazonSPAPIClient;

public class AmazonApiSettings
{
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
    public string BaseUrl { get; set; }
    public string SellerId { get; set; }
    public string Region { get; set; }
    public string TokenUrl { get; set; }
    public string DefaultCarrierCode { get; set; } = "Royal Mail";
    public string DefaultShippingMethod { get; set; } = "Standard";
}