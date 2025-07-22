namespace denModels.OauthApi;

public class OauthTokenHandler
{
    public delegate Task<OauthTokenObject> GetOauthTokenDelegate(int locationid, bool logEachEvent);
    public delegate HttpRequestMessage GetRefreshHeaderDelegate(OauthTokenObject weToken);
    public delegate KeyValuePair<string, string> ResponseStringToTokensDelegate(string token, Action<string> pisz);
    public delegate Dictionary<string, string> GetRequestHeadersDelegate();

    public string RequestUrl { get; set; }

    public delegate Task UpdateTokenDelegate(string refresh_token, string access_token, int locationId, bool LogEachEvent);

    public ResponseStringToTokensDelegate ConvertResponseStringToToken { get; set; }
    public GetRequestHeadersDelegate GetRequestHeaders { get; set; }
    public OauthTokenObject OauthToken { get; set; } = null;
    public GetRefreshHeaderDelegate GetRefreshHeader;
    public GetOauthTokenDelegate TokenRetriever { get; set; }
    public UpdateTokenDelegate TokenSetter { get; set; }
    public HttpClient Client { get; set; }
}