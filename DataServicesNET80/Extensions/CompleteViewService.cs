using DataServicesNET80.Models;

namespace DataServicesNET80.Extensions;

public class CompleteViewService : EntityService<completeview>
{
    private CompleteViewRepository _completesVieRepository;
    //IRepository<order> _orderRepository;

    public CompleteViewService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _completesVieRepository = unitOfWork.GetRepository<completeview>() as CompleteViewRepository;
    }


    public async Task<List<completeview>> GetDistinctCompletesWithinTimeRange(DateTime startDate, DateTime endDate, HashSet<string> countries,int locationid)
    {
        return await _completesVieRepository.GetDistinctCompletesWithinTimeRange(startDate,endDate,countries,locationid);
              
    }





}