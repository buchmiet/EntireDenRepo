namespace DataServicesNET80;

public class ConsoleMessageDialog : IMessageDialog
{

    static bool GetBooleanFromInput(string input)
    {           
        return true;
    }

    public async Task<int> ShowErrorAsync(string message, string title)
    {
        return 1;
    }

    public async Task<bool> ShowYesNoDialogAsync(string message, string title)
    {
        return true;
    }
}