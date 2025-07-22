namespace denModels.OauthApi;

public class OauthTokenObject
{
    private string _refreshToken;
    public string RefreshToken
    {
        get => _refreshToken;

        set
        {

            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            _refreshToken = value;
        }
    }
    public string AppID { get; set; }
    public string CertID { get; set; }
    public string OauthToken { get; set; }
    public string DevID { get; set; }
    public string AccessToken { get; set; }
    public string Client_id { get; set; }
    public string Client_secret { get; set; }
}