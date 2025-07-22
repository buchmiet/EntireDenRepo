namespace DataServicesNET80;

public interface IConnectionTimeoutWarning
{
    Task ShowWarningAsync();
    Task CloseWarning();
}