using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.DataAccess.Repositories
{
    public interface IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
    {
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        Task UpdateAsync(TKey id, TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
