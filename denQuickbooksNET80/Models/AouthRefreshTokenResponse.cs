namespace denQuickbooksNET80.Models;

public class AouthRefreshTokenResponse
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int x_refresh_token_expires_in { get; set; }
    public string refresh_token { get; set; }
    public int expires_in { get; set; }
}