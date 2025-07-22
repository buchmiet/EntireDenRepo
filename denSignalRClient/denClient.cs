using Newtonsoft.Json;
using System.Net;
using System.Text;

using static denSignalRClient.SignalRService;

namespace denSignalRClient;

public class TokenResponse
{
    public string tokenType { get; set; }
    public string accessToken { get; set; }
    public int expiresIn { get; set; }
    public string refreshToken { get; set; }
}

public class denClient
{
    public static string webapiurl =// "https://localhost:7291/";
        "https://buchmiet.com/";

    private Func<Task> _onDataReceivedAsync;
    private Func<Task> _onDataSentAsync;

    public void ConfigureOnDataReceivedAndSent(Func<Task> onDataReceivedAsync, Func<Task> onDataSentAsync)
    {
        _onDataReceivedAsync = onDataReceivedAsync;
        _onDataSentAsync = onDataSentAsync;
    }

    private Action<string> _log;

    public string _username { get; set; }
    public string _password { get; set; }
    private readonly IHttpClientFactory _clientFactory;
    private string _handshake { get; set; }
    private SignalRService sr;
    public bool connected = false;

    public string access_token { get; set; }
    public string refresh_token { get; set; }

    public async Task<string> GetCall(HttpClient client, string call)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, webapiurl + call);
        request.Headers.Add("Authorization", "Bearer " + access_token);
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            request = new HttpRequestMessage(HttpMethod.Post, webapiurl + "refresh");
            var content = new StringContent($"{{\r\n \"refreshToken\":\"{refresh_token}\"\r\n}}", Encoding.UTF8, "application/json");
            request.Content = content;
            response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var tkresponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
                access_token = tkresponse.accessToken;
                refresh_token = tkresponse.refreshToken;
                request = new HttpRequestMessage(HttpMethod.Get, webapiurl + call);
                request.Headers.Add("Authorization", "Bearer " + access_token);
                response = await client.SendAsync(request);
            }
            else
            {
                return null;
            }
        }

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            return null;
        }
    }

    public denClient(string username, string password, IHttpClientFactory clientFactory, Action<string> log)
    {
        _username = username;
        _password = password;
        _clientFactory = clientFactory;
        _log = log;
    }

    public async Task<bool> Connect()
    {
        var client = _clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, webapiurl + "login");
        var content = new StringContent($"{{\r\n \"email\":\"{_username}\",\r\n \"password\":\"{_password}\"\r\n}}", Encoding.UTF8, "application/json");
        request.Content = content;

        try
        {
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var tkresponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
            access_token = tkresponse.accessToken;
            refresh_token = tkresponse.refreshToken;

            _handshake = await GetCall(client, "api/Client/AllocateClient");
            if (_handshake == null)
            {
                return false;
            }
            connected = true;
            sr = new SignalRService(_username);
            if (_onDataReceivedAsync != null && _onDataSentAsync != null)
            {
                sr.ConfigureOnDataReceivedAndSent(_onDataReceivedAsync, _onDataSentAsync);
            }
            sr.SetSessionToken(_handshake, webapiurl + "denHub");
            await sr.ConnectAsync();

            return true;
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _log($"Exception thrown : '{ex.Message}', inner exception '{ex.InnerException}'. Please check whether SSL certificate is valid.");
            throw ex;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return false;
        }
    }

    public async Task Disconnect()
    {
        if (sr != null && connected)
        {
            await sr.DisconnectAsync();
        }
    }

    public class GetSensitiveInformationCommandValue
    {
        public string HandShake { get; set; }
    }

    public async Task<SensitiveInformation> GetSensitiveData()
    {
        var tcs = new TaskCompletionSource<SensitiveInformation>();
        sr.SDProvider = async (SensitiveInformation sd) =>
        {
            tcs.TrySetResult(sd);
        };

        var cv = new GetSensitiveInformationCommandValue { HandShake = _handshake };

        await sr.SendMessageAsync("Get Sensitive information", JsonConvert.SerializeObject(cv));

        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(10000));
        if (completedTask == tcs.Task)
        {
            var sd = await tcs.Task;
            return sd;
        }
        else
        {
            return null; // lub obsłuż timeout w inny sposób
        }
    }
}