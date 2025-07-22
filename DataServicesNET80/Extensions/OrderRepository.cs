using DataServicesNET80.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DataServicesNET80.Extensions;

public class orderRepository(DbContext context) : IRepository<order>
{    
    private readonly DbSet<order> _dbSet = context.Set<order>();

    public async IAsyncEnumerable<order> GetAllAsyncStream(Expression<Func<order, bool>> predicate)
    {
        await foreach (var entity in _dbSet.Where(predicate).AsAsyncEnumerable())
        {
            yield return entity;
        }
    }

    public async Task<IEnumerable<TResult>> GetSelectedColumnsAsync<TResult>(
        Expression<Func<order, bool>> filter,
        Expression<Func<order, TResult>> selector)
    {
        return await _dbSet.Where(filter).Select(selector).ToListAsync();
    }

    public Task<IQueryable<order>> GetAllIncludingAsync(params Expression<Func<order, object>>[] includeProperties)
    {
        IQueryable<order> query = _dbSet;
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return Task.FromResult(query);
    }

    public bool Any(Expression<Func<order, bool>> predicate)
    {
        bool query = _dbSet.Any(predicate);
        return query;
    }

    public Task<IQueryable<order>> GetAllAsync(Expression<Func<order, bool>> predicate)
    {
        IQueryable<order> query = _dbSet.AsNoTracking().Where(predicate);
        return Task.FromResult(query);

    }

    public IAsyncEnumerable<order> GetAllAsAsyncEnumerable(Expression<Func<order, bool>> predicate = null)
    {
        IQueryable<order> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query.AsAsyncEnumerable();
    }

    public Task<IQueryable<order>> GetAllAsync()
    {
        IQueryable<order> query = _dbSet.AsNoTracking();
        return Task.FromResult(query);
    }


    public async Task<Answers.Answer> GetByIdAsync(object id)
    {
        var answer = Answers.Answer.Prepare($"getting order by id {id}");
        var order = await _dbSet.FindAsync(id).ConfigureAwait(false);
        if (order is null)
        {
            return answer.Error("Not found");
        }
        return answer.WithValue(order);
    }

    public async Task AddAsync(order entity)
    {
        await _dbSet.AddAsync(entity).ConfigureAwait(false);
    }

    public async Task<IQueryable<order>> GetordersWithItemsAsync(Expression<Func<order, bool>> predicate)
    {       
        return  _dbSet.AsNoTracking().Where(predicate).Include(o => o.orderitems);
    }

    public async Task UpdateAsync(order entity)
    {
        _dbSet.Attach(entity);
        _dbSet.Entry(entity).State = EntityState.Modified;        
    }
    public async Task DeleteAsync(order entity)
    {            
        _dbSet.Remove(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<order> entities)
    {
        _dbSet.RemoveRange(entities);            
    }

    public async Task<int> CountAsync(Expression<Func<order, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate).ConfigureAwait(false);
    }

    public async Task UpdateRangeAsync(IEnumerable<order> entities)
    {
        foreach (var entity in entities)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
        }    
    }

    public async Task AddRangeAsync(IEnumerable<order> entities)
    {
        await _dbSet.AddRangeAsync(entities).ConfigureAwait(false);

    }

    public Task<order?> GetOnesync(Expression<Func<order?, bool>> predicate)
    {
        Task<order?> query = _dbSet.FirstOrDefaultAsync(predicate);
        return query;
    }

    public Task<order?> GetOneAsync(Expression<Func<order, bool>> predicate, Expression<Func<order, object>> orderByDescending = null)
    {

        IQueryable<order> query = _dbSet.Where(predicate);

        if (orderByDescending != null)
        {
            query = query.OrderByDescending(orderByDescending);
        }

        return query.FirstOrDefaultAsync();
            
    }



    public async Task<TProperty> MaxAsync<TProperty>(Expression<Func<order, bool>> filter, Expression<Func<order, TProperty>> selector)
    {
        return await _dbSet
            .Where(filter)
            .MaxAsync(selector)
            .ConfigureAwait(false);
    }

    public async Task<TProperty> MinAsync<TProperty>(Expression<Func<order, bool>> filter, Expression<Func<order, TProperty>> selector)
    {
        return await _dbSet
            .Where(filter)
            .MinAsync(selector)
            .ConfigureAwait(false);
    }

    public IQueryable<order> Query()
    {
        return _dbSet;
    }

        
    public Task<IQueryable<order>> GetAllAsNoTrackingAsync(Expression<Func<order, bool>> predicate)
    {
        IQueryable<order> query = _dbSet.Where(predicate).AsNoTracking();
        return Task.FromResult(query);
    }

    public Task<IQueryable<order>> GetAllAsNoTrackingAsync()
    {
        IQueryable<order> query = _dbSet.AsNoTracking();
        return Task.FromResult(query);
    }


}