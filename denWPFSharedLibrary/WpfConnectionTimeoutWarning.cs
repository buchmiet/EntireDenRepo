using DataServicesNET80;
using System.Windows;

namespace denWPFSharedLibrary;

public class WpfConnectionTimeoutWarning : ITimeoutWarning
{
    private ConnectionTimeoutWarningWindow _warningWindow;

    public async Task ShowAsync(CancellationToken cancellationToken, TimeSpan delaySeconds)
    {
        try
        {
            await Task.Delay(delaySeconds, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            // Jeśli operacja została anulowana, nie pokazuj okienka
            return;
        }

        // Jeśli operacja nie została anulowana, pokaż okienko informacyjne
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _warningWindow = new ConnectionTimeoutWarningWindow();
            _warningWindow.Show();
        });

        cancellationToken.Register(() =>
        {
            Application.Current.Dispatcher.Invoke(Close);
        });
    }

    public void Close()
    {
        _warningWindow?.Close();
        _warningWindow = null;
    }
}