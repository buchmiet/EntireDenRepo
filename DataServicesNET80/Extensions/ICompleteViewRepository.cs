using DataServicesNET80.Models;

namespace DataServicesNET80.Extensions;

public interface ICompleteViewRepository:IRepository<completeview>
{
    Task<List<completeview>> GetDistinctCompletesWithinTimeRange(DateTime startDate, DateTime endDate,
        HashSet<string> countries, int locationid);
}