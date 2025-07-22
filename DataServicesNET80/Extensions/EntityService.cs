using System.Collections.Immutable;
using System.Linq.Expressions;
using Answers;
using Microsoft.EntityFrameworkCore;

namespace DataServicesNET80.Extensions;

public interface IEntityService<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity, bool saveChanges = true);
    Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true);
    bool Any(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task DeleteAsync(TEntity entity, bool saveChanges = true);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true);
    IAsyncEnumerable<TEntity> GetAllAsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate = null);
    Task<IQueryable<TEntity>> GetAllAsync();
    Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    IAsyncEnumerable<TEntity> GetAllAsyncStream(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> GetAllIncludingAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
    Task<IEnumerable<TResult>> GetAllSelectedColumnsAsync<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector);
    Task<Answer> GetByIdAsync(int id);
    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByDescending = null);
    Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProperty>> selector);
    Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProperty>> selector);
    IQueryable<TEntity> Query();
    Task UpdateAsync(TEntity entity, bool saveChanges = true);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true);
}

public class EntityService<TEntity> : IEntityService<TEntity> where TEntity : class
{
    private readonly IUnitOfWork _unitOfWork;
    public readonly IRepository<TEntity> _repository;

    public IAsyncEnumerable<TEntity> GetAllAsyncStream(Expression<Func<TEntity, bool>> predicate) =>
        _repository.GetAllAsyncStream(predicate);

    public IQueryable<TEntity> Query()
    {
        return _repository.Query();
    }

    public Task<IEnumerable<TResult>> GetAllSelectedColumnsAsync<TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> selector)
    {
        return _repository.GetSelectedColumnsAsync(filter, selector);
    }

    public async Task<IEnumerable<TEntity>> GetAllIncludingAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = await _repository.GetAllIncludingAsync(includeProperties);
        return await query.Where(filter).ToListAsync();
    }

    public EntityService(IUnitOfWork unitOfWork)
    {

        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.GetRepository<TEntity>();
    }

    public Task<Answers.Answer> GetByIdAsync(int id)=> _repository.GetByIdAsync(id);
        

    public Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate) =>

        _repository.GetAllAsync(predicate);

    public async Task<ImmutableArray<TEntity>> GetAllAsImmutableArrayAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var query = _repository.GetAllAsNoTrackingAsync(predicate);
        var list = await query;
        return [..list];
    }

    public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByDescending = null)
    {

        return await _repository.GetOneAsync(predicate, orderByDescending);

    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        bool query = _repository.Any(predicate);
        return query;
    }


    public Task<IQueryable<TEntity>> GetAllAsync()=> _repository.GetAllAsync();
    public async Task<ImmutableArray<TEntity>> GetAllAsImmutableArrayAsync()
    {
        var query = _repository.GetAllAsNoTrackingAsync();
        var list = await query;
        return [.. list];
    }


    public IAsyncEnumerable<TEntity> GetAllAsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate = null)
    {
        return _repository.GetAllAsAsyncEnumerable(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _repository.CountAsync(predicate);//.ConfigureAwait(false);
    }

    public async Task AddAsync(TEntity entity, bool saveChanges = true)
    {
        await _repository.AddAsync(entity).ConfigureAwait(false);
        if (saveChanges)
        {
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task UpdateAsync(TEntity entity, bool saveChanges = true)
    {
        await _repository.UpdateAsync(entity).ConfigureAwait(false);
        if (saveChanges)
        {
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true)
    {
        await _repository.UpdateRangeAsync(entities).ConfigureAwait(false);
        if (saveChanges)
        {
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true)
    {
        await _repository.AddRangeAsync(entities).ConfigureAwait(false);
        if (saveChanges)
        {
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task DeleteAsync(TEntity entity, bool saveChanges = true)
    {
        await _repository.DeleteAsync(entity).ConfigureAwait(false);
        if (saveChanges)
        {
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true)
    {
        await _repository.DeleteRangeAsync(entities);
        if (saveChanges)
        {
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }


    public async Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProperty>> selector)
    {
        return await _repository.MaxAsync(filter, selector).ConfigureAwait(false);//.ConfigureAwait(false);
    }
    public async Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProperty>> selector)
    {
        return await _repository.MinAsync(filter, selector).ConfigureAwait(false);//.ConfigureAwait(false);
    }
}