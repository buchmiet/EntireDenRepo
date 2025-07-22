namespace AmazonSPAPIClient;

public class AmazonErrorResponse
{
    public List<AmazonError> errors { get; set; }
}
public class AmazonError
{
    public string code { get; set; }
    public string details { get; set; }
    public string message { get; set; }
}