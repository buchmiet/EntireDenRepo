//using DataServicesNET80.Extensions;
//using OauthTokenModel = denModels.Persistent.OauthToken;
//using System.Linq.Expressions;

//namespace DataServices.Adapter.Services;

//public class ModelService<T> 
//{
//    private readonly IEntityService<T> _entityService;

//    public ModelService(IEntityService<T> entityService)
//    {
//        _entityService = entityService;
//    }

//    public async Task<T?> GetOneAsync(Expression<Func<TU, bool>> predicate, Expression<Func<TU, object>> orderByDescending = null)
//    {
//        TU? response = await _entityService.GetOneAsync(predicate, orderByDescending);
//        var oaService = new ModelService<OauthTokenModel>(null);
//        return response.ToDomainModel();
//    }
//}