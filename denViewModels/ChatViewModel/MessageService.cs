using Newtonsoft.Json;


namespace denViewModels;

public class UserMessage
{
    public string Username { get; set; }
    public string Message { get; set; }
}

public interface IMessageService
{
    Task SendMessageAsync(string command, string jsonValue);
    Task SendUserMessage(UserMessage userMessage);
}

public class MessageService : IMessageService
{
    public async Task SendMessageAsync(string command, string jsonValue)
    {
        // Logika wysyłania wiadomości
    }

    public async Task SendUserMessage(UserMessage userMessage)
    {
        var json = JsonConvert.SerializeObject(userMessage);
        await SendMessageAsync("UserMessage", json);
    }
}