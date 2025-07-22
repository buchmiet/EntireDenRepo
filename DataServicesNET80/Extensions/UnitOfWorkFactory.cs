namespace DataServicesNET80.Extensions;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    public IUnitOfWork Create()
    {
        var context = DbContextFactory.GetWriteableContext();
        return new UnitOfWork(context);
           
    }

    public IUnitOfWork CreateWriteable() => Create();

    public IUnitOfWork CreateReadable()
    {
        var context = DbContextFactory.GetContext();
        return new UnitOfWork(context);
    }
}