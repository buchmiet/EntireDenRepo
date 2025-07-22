namespace AmazonSPAPIClient;

public class AmazonPatchReport
{
    public string sku { get; set; }
    public string status { get; set; }
    public string submissionId { get; set; }
    public List<Issue> issues { get; set; }
}

public class Issue
{
    public string code { get; set; }
    public string message { get; set; }
    public string severity { get; set; }
}