using System.Linq.Expressions;
using Answers;

namespace DataServicesNET80.Extensions;

public interface IRepository<TEntity> where TEntity : class
{
    IAsyncEnumerable<TEntity> GetAllAsyncStream(Expression<Func<TEntity, bool>> predicate);
    Task<IQueryable<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] includeProperties);
    IQueryable<TEntity> Query();
    Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByDescending = null);
    Task<TProperty> MaxAsync<TProperty>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProperty>> selector);
    Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProperty>> selector);
    Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IQueryable<TEntity>> GetAllAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IQueryable<TEntity>> GetAllAsNoTrackingAsync();
    Task<IQueryable<TEntity>> GetAllAsync();
    Task<Answer> GetByIdAsync(object id);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    bool Any(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TResult>> GetSelectedColumnsAsync<TResult>(Expression<Func<TEntity, bool>> filter,Expression<Func<TEntity, TResult>> selector);
    IAsyncEnumerable<TEntity> GetAllAsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate = null);
}