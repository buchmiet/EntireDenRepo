namespace DataServicesNET80.Models;

public partial class OauthTokenWithService
{
    public int OauthTokenId { get; set; }
    public int LocationId { get; set; }
    public int ServiceId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AppId { get; set; }
    public string? CertId { get; set; }
    public string? DevId { get; set; }
    public string? AdditionalData { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string ServiceName { get; set; } = null!;
}