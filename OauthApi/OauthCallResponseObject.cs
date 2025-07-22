using System.Net.Http.Headers;
using System.Net;

namespace OauthApi;

public record OauthCallResponseObject
{
    public string ResponseString { get; set; }
    public HttpResponseHeaders ResponseHeaders { get; set; }
    public HttpStatusCode StatusCode { get; set; }

}