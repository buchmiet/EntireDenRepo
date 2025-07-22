namespace DataServicesNET80;

public record DatabaseOperationMediator
{
    public ITimeoutWarning TimeoutWarning { get; set; }
    public IMessageDialog MessageDialog { get; set; }
}