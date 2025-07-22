using DataServicesNET80.DatabaseAccessLayer;

namespace DataServicesNET80;

public class DatabaseAccessLayerForConsoleFactory
{
    public IDatabaseAccessLayer Create()
    {
        var dal = new DatabaseAccessLayer.DatabaseAccessLayer();
        var databaseOperationMediator = new DatabaseOperationMediator
        {
            TimeoutWarning = new ConsoleConnectionTimeoutWarning(),
            MessageDialog = new ConsoleMessageDialog()
        };
        DatabaseOperationExecutor.Instance.SetMediator(databaseOperationMediator);
        return dal;
    }
}