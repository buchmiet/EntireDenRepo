using System.Net;
using System.Net.Http.Headers;
using System.Text;
using denModels.OauthApi;
using Serilog;

namespace OauthApi;

public interface IOauthService
{
    void AddOauthService(OauthTokenHandler tokenHandler, string serviceIdentifier);
    HttpRequestMessage CreateGetRequest(OauthTokenHandler otf, string callname, string parameters);
    Task<OauthCallResponseObject> GetResponseFromApiCallWithWithInput(string service, OauthRequestType requestType, string callname, string inputJsonString, Action<string> pisz, int locationID, bool LogEachEvent);
    Task<OauthCallResponseObject> GetResponseFromApiCall(string service, string callname, string parameters, Action<string> pisz, int locationID, bool LogEachEvent);
    HttpRequestMessage CreatePatchRequest(OauthTokenHandler otf, string callname, string inputJsonString);
    OauthTokenHandler GetOauthTokenHandler(string service);
    Dictionary<string, OauthTokenHandler> OauthServices { get; }
    Task<string> Refresh(OauthTokenHandler otf, HttpRequestMessage httpRequestMessage);
}

public class OauthService : IOauthService
{

    public void AddOauthService(OauthTokenHandler tokenHandler, string serviceIdentifier)
    {
        OauthServices.Add(serviceIdentifier, tokenHandler);
    }
    public Dictionary<string, OauthTokenHandler> OauthServices = new();

    Dictionary<string, OauthTokenHandler> IOauthService.OauthServices => throw new NotImplementedException();

    public async Task<string> Refresh(OauthTokenHandler otf, HttpRequestMessage httpRequestMessage)
    {
        var result = await otf.Client.SendAsync(httpRequestMessage).ConfigureAwait(false);
        var response= await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        return response;
    }

    public HttpRequestMessage CreateGetRequest(OauthTokenHandler otf, string callname, string parameters)
    {
        var requestUrl = otf.RequestUrl + callname + (string.IsNullOrEmpty(parameters) ? "" : "?" + parameters);
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUrl));
        var requestHeaders = otf.GetRequestHeaders();
        if (requestHeaders.Count != 0)
        {
            foreach (var keyValuePair in requestHeaders)
            {
                if (keyValuePair.Key.Equals("Authorization"))
                {
                    request.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
                }
                else
                {
                    request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (!requestHeaders.Any(p=>p.Key.Equals("Authorization")))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", otf.OauthToken.OauthToken);
        }
        return request;
    }

    public HttpRequestMessage CreatePatchRequest(OauthTokenHandler otf, string callname, string inputJsonString)
    {
        var requestUrl = otf.RequestUrl + callname;
        var request = new HttpRequestMessage(HttpMethod.Patch, new Uri(requestUrl))
        {
            Content = new StringContent(inputJsonString, Encoding.UTF8, "application/json")
        };

        var requestHeaders = otf.GetRequestHeaders();
        if (requestHeaders.Any())
        {
            foreach (var keyValuePair in requestHeaders)
            {
                if (keyValuePair.Key.Equals("Authorization"))
                {
                    request.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
                }
                else
                {
                    request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (!requestHeaders.Any(p => p.Key.Equals("Authorization")))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", otf.OauthToken.OauthToken);
        }
        return request;
    }


    public HttpRequestMessage CreatePostRequest(OauthTokenHandler otf, string callname, string inputJsonString)
    {
        var requestUrl = otf.RequestUrl;
        var request = new HttpRequestMessage(HttpMethod.Post, requestUrl + callname)
        {
            Content = new ByteArrayContent(Encoding.UTF8.GetBytes(inputJsonString))
        };
        var requestHeaders = otf.GetRequestHeaders();
        if (requestHeaders.Any())
        {
            foreach (var keyValuePair in requestHeaders)
            {
                if (keyValuePair.Key.Equals("Authorization"))
                {
                    request.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
                }
                else
                {
                    request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (!requestHeaders.Any(p => p.Key.Equals(("Authorization"))))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", otf.OauthToken.OauthToken);
        }
        return request;
    }

 


    public async Task<OauthCallResponseObject> GetResponseFromApiCallWithWithInput(string service, OauthRequestType requestType, string callname, string inputJsonString, Action<string> pisz, int locationID,bool LogEachEvent)
    {
        var otf = OauthServices[service];
        if (otf.OauthToken == null)
        {
            otf.OauthToken = await otf.TokenRetriever(1,true);
        }
        HttpRequestMessage requestMessage=null;
        if (requestType == OauthRequestType.POST)
        {
            requestMessage= CreatePostRequest(otf, callname, inputJsonString);
        }
        else
        {
            requestMessage=CreatePatchRequest(otf, callname, inputJsonString);
        }

        var result = await otf.Client.SendAsync(requestMessage).ConfigureAwait(false);

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {

            var refreshHeader = otf.GetRefreshHeader(otf.OauthToken);
            var newToken = otf.ConvertResponseStringToToken(await Refresh(otf, refreshHeader).ConfigureAwait(false), pisz);
            if (!string.IsNullOrEmpty(newToken.Key))
            {
                await otf.TokenSetter(newToken.Key, newToken.Value, locationID, LogEachEvent).ConfigureAwait(false);
                otf.OauthToken.AccessToken = newToken.Value;
                otf.OauthToken.RefreshToken = newToken.Key;
                if (requestType == OauthRequestType.POST)
                {
                    requestMessage=CreatePostRequest(otf, callname, inputJsonString);
                }
                else
                {
                    requestMessage= CreatePatchRequest(otf, callname, inputJsonString);
                }
                result = await otf.Client.SendAsync(requestMessage).ConfigureAwait(false);
                return await GetOauthCallResponseObject();
            }
        }
        return await GetOauthCallResponseObject();

        async Task<OauthCallResponseObject> GetOauthCallResponseObject()
        {
            return new OauthCallResponseObject
            {
                ResponseHeaders = result.Headers,
                StatusCode = result.StatusCode,
                ResponseString = await result.Content.ReadAsStringAsync().ConfigureAwait(false),
            };
        }
    }






    public async Task<OauthCallResponseObject> GetResponseFromApiCall(string service, string callname, string parameters, Action<string> pisz, int locationID, bool LogEachEvent)
    {
        OauthTokenHandler otf = null;
        HttpRequestMessage request = null;
        HttpResponseMessage result = null;
            
        try
        {
            otf = OauthServices[service];
            if (otf.OauthToken == null)
            {
                otf.OauthToken = await otf.TokenRetriever(1, true).ConfigureAwait(false);
            }
            request = CreateGetRequest(otf, callname, parameters);

            var hedy = "{" + Environment.NewLine;

            foreach (var e in request.Headers)
            {
                hedy += $"{{ {e.Key}, {e.Value} }},";
            }
            hedy += "}";

            

            result = await otf.Client.SendAsync(request).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while setting up or sending request in GetResponseFromApiCall. Service: {Service}, CallName: {CallName}, Parameters: {Parameters}, Exception: {ExceptionMessage}", service, callname, parameters, ex.Message);
            pisz($"Error while calling service {service}, call name {callname} with parameters {parameters}: {ex.InnerException?.Message ?? ex.Message}");
            return null; // Możesz zwrócić null lub inny odpowiedni typ błędu
        }

        if (result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden)
        {
            try
            {
                HttpRequestMessage refreshHeader = otf.GetRefreshHeader(otf.OauthToken);
                var responseString = await Refresh(otf, refreshHeader).ConfigureAwait(false);

                Log.Debug("Refreshing token for service {Service}. Refresh token response: {ResponseString}", service, responseString);

                KeyValuePair<string, string> newToken = otf.ConvertResponseStringToToken(responseString, pisz);
                if (!string.IsNullOrEmpty(newToken.Value))
                {
                    await otf.TokenSetter(newToken.Key, newToken.Value, locationID, LogEachEvent).ConfigureAwait(false);

                    otf.OauthToken.OauthToken = newToken.Value;
                    request = CreateGetRequest(otf, callname, parameters);
                    result = await otf.Client.SendAsync(request).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during token refresh for service {Service}. CallName: {CallName}, Parameters: {Parameters}, Exception: {ExceptionMessage}", service, callname, parameters, ex.Message);
                return null; // Możesz zwrócić null lub inny odpowiedni typ błędu
            }
        }

        try
        {
            return new OauthCallResponseObject
            {
                ResponseHeaders = result.Headers,
                StatusCode = result.StatusCode,
                ResponseString = await result.Content.ReadAsStringAsync().ConfigureAwait(false),
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while creating response object in GetResponseFromApiCall. Service: {Service}, CallName: {CallName}, Parameters: {Parameters}, Exception: {ExceptionMessage}", service, callname, parameters, ex.Message);
            return null; // Możesz zwrócić null lub inny odpowiedni typ błędu
        }
    }

    public OauthTokenHandler GetOauthTokenHandler(string service) => OauthServices[service];


}