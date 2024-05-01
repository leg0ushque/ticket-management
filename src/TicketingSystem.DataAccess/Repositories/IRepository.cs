using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TicketingSystem.DataAccess.Repositories
{
    public interface IRepository<TEntity>
    {
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
