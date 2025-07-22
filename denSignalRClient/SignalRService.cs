using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace denSignalRClient;

public class SignalRService
{
    private Func<Task> _onDataReceivedAsync;
    private Func<Task> _onDataSentAsync;

    public void ConfigureOnDataReceivedAndSent(Func<Task> onDataReceivedAsync, Func<Task> onDataSentAsync)
    {
        _onDataReceivedAsync = onDataReceivedAsync;
        _onDataSentAsync = onDataSentAsync;
    }

    public delegate Task SDProviderDelegate(SensitiveInformation sd);

    public SDProviderDelegate SDProvider;

    public class CommandMessage
    {
        public string Command { get; set; }
        public string Value { get; set; }
    }

    private bool _isConnected = false;

    private HubConnection _connection;
    private string _userName { get; set; }
    private string _clientName { get; set; }
    private string _handShake { get; set; }
    private string _hubUrl { get; set; }


    public class SensitiveInformation
    {

        public int SensitiveInformationId { get; set; } //PK

        public int UserId { get; set; } // FK to UserMaxClientsAssociation
        public string DBHost { get; set; }
        public string DBPort { get; set; }
        public string DBUserName { get; set; }
        public string DBPassword { get; set; }
        public string DBname { get; set; }
    }



    public void SetSessionToken(string sessionToken, string huburl)
    {
        _handShake = sessionToken;
        _hubUrl = huburl;
        var urlWithToken = $"{_hubUrl}?handshake={_handShake}";
        _connection = new Microsoft.AspNetCore.SignalR.Client.HubConnectionBuilder()
            .WithUrl(urlWithToken)
            .Build();
          
        _connection.On<string>("SensitiveDataIncoming", async (response) => HandleSensitiveDataIncoming(response));
        _connection.On("LifeCheck", HandleLifeCheck);

        _connection.On<string>("INameYou", async (name) =>
        {
            _clientName = name;
          
        });

        _connection.Closed += async (error) =>
        {
            if (_isConnected)
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await ConnectAsync();
            }
        };
    }


    private async Task HandleSensitiveDataIncoming(string response)
    {
        _onDataReceivedAsync?.Invoke();
        var sd = JsonConvert.DeserializeObject<SensitiveInformation>(response);
        await SDProvider(sd);
            
    }

    private async Task HandleLifeCheck()
    {
        _onDataReceivedAsync?.Invoke();
        //        Console.WriteLine("client "+_clientName+" for user "+_userName+" is being tested for signes of life");
        await _connection.InvokeAsync("AcknowledgeLifeCheck", _connection.ConnectionId);
    }


    public SignalRService(string userName)
    {
        _userName = userName;
    }

    public async Task ConnectAsync()
    {
        //      Console.WriteLine("connecting via signalr to obtain clientname for user {0} and handshake {1}",_userName,_handShake);

        if (_connection.State == HubConnectionState.Disconnected)
        {

            await _connection.StartAsync();
            _isConnected = true;
            _onDataSentAsync?.Invoke();
        }
    }

    public async Task DisconnectAsync()
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            _isConnected = false;
            await _connection.StopAsync();
        }
    }


    public async Task SendMessageAsync(string command, string jsonValue)
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            _onDataSentAsync?.Invoke();
            await _connection.SendAsync("SendMessage", command, jsonValue);
        }
        else
        {
            throw new InvalidOperationException("You are not connected");
        }
    }
}