using System.Linq.Expressions;
using Answers;
using Microsoft.EntityFrameworkCore;

namespace DataServicesNET80.Extensions;

public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    public async IAsyncEnumerable<TEntity> GetAllAsyncStream(Expression<Func<TEntity, bool>> predicate)
    {
        await foreach (var entity in _dbSet.Where(predicate).AsAsyncEnumerable())
        {
            yield return entity;
        }
    }


    public Task<IQueryable<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = _dbSet;
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return Task.FromResult(query);
    }

    public async Task<IEnumerable<TResult>> GetSelectedColumnsAsync<TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> selector)
    {
        return await _dbSet.Where(filter).Select(selector).ToListAsync();
    }


    private readonly DbSet<TEntity> _dbSet;

    public EFRepository(DbContext context)        
    {        
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByDescending = null)
    { 
        IQueryable<TEntity> query = _dbSet.Where(predicate);

        if (orderByDescending != null)
        {
            query = query.OrderByDescending(orderByDescending);
        }              
        return await query.FirstOrDefaultAsync();
    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        bool query = _dbSet.Any(predicate);
        return query;
    }

    public Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {       
        IQueryable<TEntity> query = _dbSet.Where(predicate);
        return Task.FromResult(query);
    }

    public Task<IQueryable<TEntity>> GetAllAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate).AsNoTracking();
        return Task.FromResult(query);
    }


    public IAsyncEnumerable<TEntity> GetAllAsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query.AsAsyncEnumerable();
    }

    public async Task<IQueryable<TEntity>> GetAllAsync()
    {
        IQueryable<TEntity> query= _dbSet;
        return query;
    }

    public Task<IQueryable<TEntity>> GetAllAsNoTrackingAsync()
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();
        return Task.FromResult(query);
    }

    public async Task<Answer> GetByIdAsync(object id)
    {
        var answer = Answer.Prepare($"getting entity by id {id}");
        var response = await _dbSet.FindAsync(id).ConfigureAwait(false);
        if (response == null)
        {
            return answer.Error("Not found");
        }
        return answer.WithValue(response);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity).ConfigureAwait(false);      
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Attach(entity);
        _dbSet.Entry(entity).State = EntityState.Modified;
    }

    public async Task DeleteAsync(TEntity entity)
    {            
        _dbSet.Remove(entity);            
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate).ConfigureAwait(false);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
        }
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities).ConfigureAwait(false);
    }



    public async Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProperty>> selector)
    {
        return await _dbSet
            .Where(filter)
            .MaxAsync(selector)
            .ConfigureAwait(false);
    }

    public async Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProperty>> selector)
    {
        return await _dbSet
            .Where(filter)
            .MinAsync(selector)
            .ConfigureAwait(false);
    }

    public IQueryable<TEntity> Query()
    {
        return _dbSet;
    }
}