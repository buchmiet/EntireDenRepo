using DataServicesNET80.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DataServicesNET80.Extensions;

public class CompleteViewRepository : ICompleteViewRepository
{

    private readonly DbSet<completeview> _dbSet;

    public CompleteViewRepository(DbContext context)
    {
        _dbSet = context.Set<completeview>();
    }

    public async IAsyncEnumerable<completeview> GetAllAsyncStream(Expression<Func<completeview, bool>> predicate)
    {
        await foreach (var entity in _dbSet.Where(predicate).AsAsyncEnumerable())
        {
            yield return entity;
        }
    }

    public async Task<IEnumerable<TResult>> GetSelectedColumnsAsync<TResult>(
        Expression<Func<completeview, bool>> filter,
        Expression<Func<completeview, TResult>> selector)
    {
        return await _dbSet.Where(filter).Select(selector).ToListAsync();
    }

    public Task<IQueryable<completeview>> GetAllIncludingAsync(params Expression<Func<completeview, object>>[] includeProperties)
    {
        IQueryable<completeview> query = _dbSet;
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return Task.FromResult(query);
    }

    public bool Any(Expression<Func<completeview, bool>> predicate)
    {
        bool query = _dbSet.Any(predicate);
        return query;
    }

    public Task<IQueryable<completeview>> GetAllAsync(Expression<Func<completeview, bool>> predicate)
    {
        IQueryable<completeview> query = _dbSet.Where(predicate).AsNoTracking();
        return Task.FromResult(query);

    }

    public Task<IQueryable<completeview>> GetAllAsNoTrackingAsync(Expression<Func<completeview, bool>> predicate)
    {
        IQueryable<completeview> query = _dbSet.Where(predicate).AsNoTracking();
        return Task.FromResult(query);
    }

    public Task<IQueryable<completeview>> GetAllAsNoTrackingAsync()
    {
        IQueryable<completeview> query = _dbSet.AsNoTracking();
        return Task.FromResult(query);
    }

    public IAsyncEnumerable<completeview> GetAllAsAsyncEnumerable(Expression<Func<completeview, bool>> predicate = null)
    {
        IQueryable<completeview> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query.AsAsyncEnumerable();
    }

    public Task<IQueryable<completeview>> GetAllAsync()
    {
        IQueryable<completeview> query = _dbSet.AsNoTracking();
        return Task.FromResult(query);
    }


    public async Task<Answers.Answer> GetByIdAsync(object id)
    {
        var answer = Answers.Answer.Prepare($"getting completeview for id {id}");
        var entity = await _dbSet.FindAsync(id).ConfigureAwait(false);
        if (entity is null)
        {
            return answer.Error("Not found");
        }
        return answer.WithValue(entity);
    }

    public async Task AddAsync(completeview entity)
    {
        await _dbSet.AddAsync(entity).ConfigureAwait(false);
    }

     

    public async Task UpdateAsync(completeview entity)
    {
        _dbSet.Attach(entity);
        _dbSet.Entry(entity).State = EntityState.Modified;
    }
    public async Task DeleteAsync(completeview entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<completeview> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> CountAsync(Expression<Func<completeview, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate).ConfigureAwait(false);
    }

    public async Task UpdateRangeAsync(IEnumerable<completeview> entities)
    {
        foreach (var entity in entities)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
        }
    }

    public async Task AddRangeAsync(IEnumerable<completeview> entities)
    {
        await _dbSet.AddRangeAsync(entities).ConfigureAwait(false);

    }

    public Task<completeview?> GetOnesync(Expression<Func<completeview?, bool>> predicate)
    {
        Task<completeview?> query = _dbSet.FirstOrDefaultAsync(predicate);
        return query;
    }

    public Task<completeview?> GetOneAsync(Expression<Func<completeview, bool>> predicate, Expression<Func<completeview, object>> orderByDescending = null)
    {

        IQueryable<completeview> query = _dbSet.Where(predicate);

        if (orderByDescending != null)
        {
            query = query.OrderByDescending(orderByDescending);
        }

        return query.FirstOrDefaultAsync();

    }



    public async Task<TProperty> MaxAsync<TProperty>(Expression<Func<completeview, bool>> filter, Expression<Func<completeview, TProperty>> selector)
    {
        return await _dbSet
            .Where(filter)
            .MaxAsync(selector)
            .ConfigureAwait(false);
    }

    public async Task<TProperty> MinAsync<TProperty>(Expression<Func<completeview, bool>> filter, Expression<Func<completeview, TProperty>> selector)
    {
        return await _dbSet
            .Where(filter)
            .MinAsync(selector)
            .ConfigureAwait(false);
    }

    public IQueryable<completeview> Query()
    {
        return _dbSet;
    }


    public async Task<List<completeview>> GetDistinctCompletesWithinTimeRange(DateTime startDate, DateTime endDate, HashSet<string> countries,int  locationid)
    {
        return await _dbSet
            .Where(ov => ov.paidOn >= startDate && ov.paidOn <= endDate)
            .Where(ov => countries.Contains(ov.CountryCode))
            .Where(ov=>ov.locationID==locationid)
            .GroupBy(ov => ov.orderID)
            .Select(group => group.FirstOrDefault()) // wybiera pierwszy element z każdej grupy
            .ToListAsync().ConfigureAwait(false);
    }

     
}