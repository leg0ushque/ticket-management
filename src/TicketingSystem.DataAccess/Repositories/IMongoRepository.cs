using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.DataAccess.Repositories
{
    public interface IMongoRepository<TEntity> : IRepository<TEntity>
        where TEntity : IHasId
    {
        public Task UpdateAsync<TField>(string id, Expression<Func<TEntity, TField>> field, TField newValue,
            CancellationToken cancellationToken = default);

        public Task<List<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default);

        public Task<List<TEntity>> FilterAsync<TField>(Expression<Func<TEntity, TField>> field, IEnumerable<TField> values,
            CancellationToken cancellationToken = default);
    }
}
