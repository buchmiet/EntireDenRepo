using DataServicesNET80.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace DataServicesNET80.Extensions;

public class UnitOfWork : IUnitOfWork
{

    private readonly Dictionary<Type, object> _repositories;
    private bool _disposed;
    readonly DbContext Context;
    private IDbContextTransaction _transaction;

    public UnitOfWork(DbContext context)
    {
        Context = context;
        _repositories = [];
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            Context.Dispose();                   
        }
        _disposed = true;
    }

    public void Dispose()
    {

        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

      

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {


        if (_repositories.ContainsKey(typeof(TEntity)))
        {
            return (IRepository<TEntity>)_repositories[typeof(TEntity)];
        }


        if (typeof(TEntity) == typeof(order))
        {

            return new orderRepository(Context) as IRepository<TEntity>;
        }

        if (typeof(TEntity) == typeof(completeview))
        {

            return new CompleteViewRepository(Context) as IRepository<TEntity>;
        }

        var repositoryInstance = new EFRepository<TEntity>(Context);
        _repositories.Add(typeof(TEntity), repositoryInstance);
        return repositoryInstance;
    }

    public async Task DeleteAllFromTable(string tableName)
    {
        await Context.Database.ExecuteSqlRawAsync("delete from " + tableName);
    }

    public Task SaveChangesAsync()
    {

        return Context.SaveChangesAsync();
    }
    public async Task BeginTransactionAsync()
    {
        _transaction = await Context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
        }
    }

}