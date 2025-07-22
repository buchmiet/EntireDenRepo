using DataServicesNET80.Models;
using System.Linq.Expressions;

namespace DataServicesNET80.Extensions;

// Specjalizowana wersja EntityService dla order
public class orderService : EntityService<order>
{
    private orderRepository _orderRepository;
    //IRepository<order> _orderRepository;

    public orderService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _orderRepository = unitOfWork.GetRepository<order>() as orderRepository;           
    }

        
    public async Task<Dictionary<int, int>> CountordersForCustomersAsync(List<int> customerIds)
    {
        // Pobieramy zamówienia spełniające kryteria i wykonujemy grupowanie i agregację
        var ordersQuery =(await _orderRepository.GetAllAsync(order => customerIds.Contains(((order)(object)order).customerID)).ConfigureAwait(false))
            .GroupBy(order => ((order)(object)order).customerID)
            .Select(group => new { CustomerId = group.Key, Count = group.Count() })
            .ToDictionary(g => g.CustomerId, g => g.Count);

        return ordersQuery;
    }
     


    public Task< IQueryable<order>> GetordersWithItemsAsync(Expression<Func<order, bool>> predicate)
    {
        return _orderRepository.GetordersWithItemsAsync(predicate);
      
    }

}