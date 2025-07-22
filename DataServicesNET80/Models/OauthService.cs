
namespace DataServicesNET80.Models;

public class OauthService
{
    public int OauthServiceId { get; set; }
    public string ServiceName { get; set; }
    public string Description { get; set; }

    public virtual ICollection<OauthToken> OauthTokens { get; set; }
}