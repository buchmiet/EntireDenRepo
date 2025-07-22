using DataServicesNET80.Models;
using System.Linq.Expressions;

namespace DataServicesNET80.Extensions;

public interface IorderRepository : IRepository<order>
{
     
    Task<IQueryable<order>> GetordersWithItemsAsync(Expression<Func<order, bool>> predicate);
    Task<DateTime> MinAsync(Expression<Func<order, DateTime>> predicate);
    Task<DateTime> MaxAsync(Expression<Func<order, DateTime>> predicate);
}