using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Answers;
using DataServicesNET80.Extensions;
using denModels.Persistent;
using Microsoft.EntityFrameworkCore;

namespace DataServices.Adapter.Services
{
    public class Ouath_TokenService:EntityService<OauthToken>
    {


        public Ouath_TokenService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        private readonly IUnitOfWork _unitOfWork;
        public readonly IRepository<DataServicesNET80.Models.OauthToken> _repository;

        public IAsyncEnumerable<OauthToken> GetAllAsyncStream(Expression<Func<OauthToken, bool>> predicate) =>
            _repository.GetAllAsyncStream(predicate);

        public IQueryable<OauthToken> Query()
        {
            return _repository.Query();
        }

        public Task<IEnumerable<TResult>> GetAllSelectedColumnsAsync<TResult>(
        Expression<Func<OauthToken, bool>> filter,
        Expression<Func<OauthToken, TResult>> selector)
        {
            return _repository.GetSelectedColumnsAsync(filter, selector);
        }

        public async Task<IEnumerable<OauthToken>> GetAllIncludingAsync(Expression<Func<OauthToken, bool>> filter, params Expression<Func<OauthToken, object>>[] includeProperties)
        {
            var query = await _repository.GetAllIncludingAsync(includeProperties);
            return await query.Where(filter).ToListAsync();
        }


        public Task<Answer> GetByIdAsync(int id) => _repository.GetByIdAsync(id);


        public Task<IQueryable<OauthToken>> GetAllAsync(Expression<Func<OauthToken, bool>> predicate) =>

            _repository.GetAllAsync(predicate);

        public async Task<ImmutableArray<OauthToken>> GetAllAsImmutableArrayAsync(Expression<Func<OauthToken, bool>> predicate)
        {
            var query = _repository.GetAllAsNoTrackingAsync(predicate);
            var list = await query;
            return [.. list];
        }

        public async Task<OauthToken?> GetOneAsync(Expression<Func<OauthToken, bool>> predicate, Expression<Func<OauthToken, object>> orderByDescending = null)
        {

            return await _repository.GetOneAsync(predicate, orderByDescending);

        }

        public bool Any(Expression<Func<OauthToken, bool>> predicate)
        {
            bool query = _repository.Any(predicate);
            return query;
        }


        public Task<IQueryable<OauthToken>> GetAllAsync() => _repository.GetAllAsync();
        public async Task<ImmutableArray<OauthToken>> GetAllAsImmutableArrayAsync()
        {
            var query = _repository.GetAllAsNoTrackingAsync();
            var list = await query;
            return [.. list];
        }


        public IAsyncEnumerable<OauthToken> GetAllAsAsyncEnumerable(Expression<Func<OauthToken, bool>> predicate = null)
        {
            return _repository.GetAllAsAsyncEnumerable(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<OauthToken, bool>> predicate)
        {
            return await _repository.CountAsync(predicate);//.ConfigureAwait(false);
        }

        public async Task AddAsync(OauthToken entity, bool saveChanges = true)
        {
            await _repository.AddAsync(entity).ConfigureAwait(false);
            if (saveChanges)
            {
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(OauthToken entity, bool saveChanges = true)
        {
            await _repository.UpdateAsync(entity).ConfigureAwait(false);
            if (saveChanges)
            {
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<OauthToken> entities, bool saveChanges = true)
        {
            await _repository.UpdateRangeAsync(entities).ConfigureAwait(false);
            if (saveChanges)
            {
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task AddRangeAsync(IEnumerable<OauthToken> entities, bool saveChanges = true)
        {
            await _repository.AddRangeAsync(entities).ConfigureAwait(false);
            if (saveChanges)
            {
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(OauthToken entity, bool saveChanges = true)
        {
            await _repository.DeleteAsync(entity).ConfigureAwait(false);
            if (saveChanges)
            {
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        public async Task DeleteRangeAsync(IEnumerable<OauthToken> entities, bool saveChanges = true)
        {
            await _repository.DeleteRangeAsync(entities);
            if (saveChanges)
            {
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }


        public async Task<TProperty> MaxAsync<TProperty>(Expression<Func<OauthToken, bool>> filter, Expression<Func<OauthToken, TProperty>> selector)
        {
            return await _repository.MaxAsync(filter, selector).ConfigureAwait(false);//.ConfigureAwait(false);
        }
        public async Task<TProperty> MinAsync<TProperty>(Expression<Func<OauthToken, bool>> filter, Expression<Func<OauthToken, TProperty>> selector)
        {
            return await _repository.MinAsync(filter, selector).ConfigureAwait(false);//.ConfigureAwait(false);
        }
    }
}

