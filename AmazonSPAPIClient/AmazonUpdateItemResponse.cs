namespace AmazonSPAPIClient;

public class AmazonUpdateItemResponse
{
    public string sku { get; set; }
    public string status { get; set; }
    public string submissionId { get; set; }
    public object[] issues { get; set; }
}