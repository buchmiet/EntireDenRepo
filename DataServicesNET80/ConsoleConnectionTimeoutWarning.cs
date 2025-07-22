

namespace DataServicesNET80;

public class ConsoleConnectionTimeoutWarning : ITimeoutWarning
{
    private ConsoleConnectionTimeoutWarning _warningWindow;

    public Task ShowAsync(CancellationToken cancellationToken, TimeSpan delaySeconds)
    {
        return Task.CompletedTask;
    }

    public void Close()
    {
        _warningWindow?.Close();
        _warningWindow = null;
    }
}