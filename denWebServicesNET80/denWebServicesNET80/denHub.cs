using denWebServicesNET80.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace denWebServicesNET80;

public delegate void ConnectionStatusChangedEventHandler(string connectionId, bool isConnected);

public class ConnectionCheckerService
{
    
    public event ConnectionStatusChangedEventHandler ConnectionStatusChanged;

    public void NotifyConnectionStatusChange(string connectionId, bool isConnected)
    {
        ConnectionStatusChanged?.Invoke(connectionId, isConnected);
    }
}



public interface ISignalRActions
{
    Task<Dictionary<string, bool>> CheckIfConnectionIsAlive(List<string> connectionIds);
}

public class SignalRActions : ISignalRActions
{
    private readonly IHubContext<denHub> _hubContext;
    private readonly ConnectionCheckerService _connectionCheckerService;

    public SignalRActions(IHubContext<denHub> hubContext, ConnectionCheckerService connectionCheckerService)
    {
        _hubContext = hubContext;
        _connectionCheckerService = connectionCheckerService;
    }

    public async Task<Dictionary<string, bool>> CheckIfConnectionIsAlive(List<string> connectionIds)
    {
        var tasks = new List<Task>();
        var responses = new ConcurrentDictionary<string, bool>();

        void onConnectionStatusChanged(string connId, bool isConnected)
        {
            if (connectionIds.Contains(connId) && isConnected)
            {
                responses[connId] = true;
            }
        }

        _connectionCheckerService.ConnectionStatusChanged += onConnectionStatusChanged;

        try
        {
            foreach (var connectionId in connectionIds)
            {
                var tcs = new TaskCompletionSource<bool>();
                tasks.Add(CheckSingleConnection(connectionId, tcs, responses));
                responses.TryAdd(connectionId, false); 
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
              
            _connectionCheckerService.ConnectionStatusChanged -= onConnectionStatusChanged;
        }

        return responses.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }


    private async Task CheckSingleConnection(string connectionId, TaskCompletionSource<bool> tcs, ConcurrentDictionary<string, bool> responses)
    {
        await _hubContext.Clients.Client(connectionId).SendAsync("LifeCheck");

        var delay = Task.Delay(5000); // Timeout 5 sekund
        var completedTask = await Task.WhenAny(tcs.Task, delay);

        if (completedTask == tcs.Task && tcs.Task.Result)
        {
            // Klient odpowiedział
            responses[connectionId] = true;
        }
        // W przeciwnym przypadku pozostaje false
    }




}


public class denHub:Hub
{
    private readonly ConnectionCheckerService _connectionCheckerService;

    public async Task AcknowledgeLifeCheck(string connectionId)
    {
        _connectionCheckerService.NotifyConnectionStatusChange(connectionId, true);
    }

    public delegate Task CommandAction(ICommandValue commandValue, HubCallerContext context);

        
    public interface ICommandValue
    {
           
    }

    public class LoginCommandValue : ICommandValue
    {
        public string Password { get; set; }
    }

    public class GetSensitiveInformationCommandValue : ICommandValue
    {
        public string HandShake { get; set; }
    }

      

    public class CommandHandler
    {
        private Dictionary<string, (Type commandValueType, CommandAction action)> commandMap;
        private IUserServices _userServices;
        private SendResponseDelegate _delegacik;

        public CommandHandler(IUserServices userServices, SendResponseDelegate delegacik)
        {
            _userServices = userServices;
            _delegacik = delegacik;
            commandMap = new Dictionary<string, (Type, CommandAction)>
            {
                { "Log In", (typeof(LoginCommandValue), LoginAction) },
                { "Get Sensitive information", (typeof(GetSensitiveInformationCommandValue), GetSensitiveInformationAction) }
            };
        }

        public async Task HandleCommand(string command, string jsonValue, HubCallerContext context)
        {
            if (commandMap.TryGetValue(command, out var commandInfo))
            {
                var commandValue = (ICommandValue)JsonConvert.DeserializeObject(jsonValue, commandInfo.commandValueType);
                await commandInfo.action(commandValue, context);
                return;
            }

            throw new InvalidOperationException("Unknown command");
        }

        private async Task LoginAction(ICommandValue commandValue, HubCallerContext context)
        {
            var loginValue = (LoginCommandValue)commandValue;
            var connectionId = context.ConnectionId;
              
        }

        private async Task GetSensitiveInformationAction(ICommandValue commandValue, HubCallerContext context)
        {
            var connectionId = context.ConnectionId;
            var handshake = (GetSensitiveInformationCommandValue) commandValue;
            var si = await _userServices.GetSensitiveInformationForConnectionIDAndHandshake(connectionId,
                handshake.HandShake);
            var jsonstring = JsonConvert.SerializeObject(si);
            _delegacik(connectionId, "SensitiveDataIncoming", jsonstring);
        }

    }



    public class CommandMessage
    {
        public string Command { get; set; }
        public string Value { get; set; }
    }


    private readonly CommandHandler commandHandler;
    private IUserServices _userServices;
    public delegate Task SendResponseDelegate(string connectionId, string methodName, string commandValue);

    public denHub(IUserServices userServices, ConnectionCheckerService connectionCheckerService)
    {
        commandHandler = new CommandHandler(userServices, SendResponse);
        _userServices = userServices;
        _connectionCheckerService = connectionCheckerService;
    }


    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var connectionId = Context.ConnectionId;
          
        await _userServices.DisconnectClient(connectionId);

        await base.OnDisconnectedAsync(exception);
    }


    public override async Task OnConnectedAsync()
    {
        var handshakeToken = Context.GetHttpContext().Request.Query["handshake"].ToString();
        var connectionId = Context.ConnectionId;
        var response = await _userServices.ConnectClient(handshakeToken,connectionId);
        var clientName = await _userServices.GetClientName(response.UserClientNamesId);
        try
        {
            await Clients.Client(connectionId).SendAsync("INameYou", clientName);
        }
        catch (Exception ex)
        {
            var trs=ex.ToString();
        }

        await base.OnConnectedAsync();
    }

    public async Task SendResponse(string connectionId, string methodName, string commandValue)
    {
        await Clients.Client(connectionId).SendAsync(methodName, commandValue);
    }


    public async Task SendMessage(string command, string jsonValue)
    {
        await commandHandler.HandleCommand(command, jsonValue, Context);
    }

}