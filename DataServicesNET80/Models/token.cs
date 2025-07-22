namespace DataServicesNET80.Models;

public partial class token
{
    public int Id { get; set; }

    public string? ebayToken { get; set; }

    public string? quickBooksToken { get; set; }

    public string? quickBooksRefresh { get; set; }

    public string? AmAccessKey { get; set; }

    public string? AmSecret { get; set; }

    public string? quickBooksAuthS { get; set; }

    public string? ebayOauthToken { get; set; }

    public string? ebayRefreshToken { get; set; }

    public string? paypalToken { get; set; }

    public int? locationID { get; set; }

    public string? client_id { get; set; }

    public string? secret_id { get; set; }

    public string? DevID { get; set; }

    public string? AppID { get; set; }

    public string? CertID { get; set; }

    public string? AmazonSPAPIToken { get; set; }

    public string? AmazonSPAPIRefreshToken { get; set; }

    public string? AmazonSPAPIClientID { get; set; }

    public string? AmazonSPAPIClientSecret { get; set; }
}
