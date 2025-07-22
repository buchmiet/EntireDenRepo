namespace DataServicesNET80.Extensions;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
    IUnitOfWork CreateWriteable();
    IUnitOfWork CreateReadable();
}