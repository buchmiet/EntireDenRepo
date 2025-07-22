namespace DataServicesNET80;

public interface ITimeoutWarning
{
    Task ShowAsync(CancellationToken cancellationToken, TimeSpan delaySeconds);
    void Close();
}