using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IService<TEntity, TEntityDto>
        where TEntity : class, IHasId
        where TEntityDto : class, IDto
    {
        Task CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<TEntityDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TEntityDto> GetByIdAsync(string entityId, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntityDto entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(string entityId, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<TEntityDto>> FilterAsync(Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<TEntityDto>> FilterAsync<TField>(Expression<Func<TEntity, TField>> field, IEnumerable<TField> values,
            CancellationToken cancellationToken = default);
    }
}
