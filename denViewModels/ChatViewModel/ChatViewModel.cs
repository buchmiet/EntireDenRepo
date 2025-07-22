using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using denViewModels;

namespace ChatViewModel;

public class ChatViewModel : ObservableObject
{
    private readonly IMessageService _messageService;
    private string _currentMessage;
    private ObservableCollection<UserMessage> _messages;

    public ChatViewModel(IMessageService messageService)
    {
        _messageService = messageService;
        _messages = new ObservableCollection<UserMessage>();
    }

    public string CurrentMessage
    {
        get => _currentMessage;
        set => SetProperty(ref _currentMessage, value);
    }

    public ObservableCollection<UserMessage> Messages => _messages;

    public ICommand SendMessageCommand => new RelayCommand(async () =>
    {
        var message = new UserMessage
        {
            Username = "TwójNazwaUżytkownika", // Do zmiany
            Message = CurrentMessage
        };

        await _messageService.SendUserMessage(message);
        Messages.Add(message);
        CurrentMessage = string.Empty;
    });

    // Metoda do odbierania wiadomości
    public void ReceiveMessage(string jsonMessage)
    {
        var userMessage = JsonConvert.DeserializeObject<UserMessage>(jsonMessage);
        Messages.Add(userMessage);
    }
}