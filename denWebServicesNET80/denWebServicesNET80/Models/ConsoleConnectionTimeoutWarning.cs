using DataServicesNET80;

namespace denWebServicesNET80.Models;

public class ConsoleConnectionTimeoutWarning : ITimeoutWarning
{
    private ConsoleConnectionTimeoutWarning _warningWindow;

    public async Task ShowAsync(CancellationToken cancellationToken, TimeSpan delaySeconds)
    {
 
    }

    public void Close()
    {
        _warningWindow?.Close();
        _warningWindow = null;
    }
}