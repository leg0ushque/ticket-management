using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IService<TEntityDto>
        where TEntityDto : class, IDto
    {
        Task CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<TEntityDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TEntityDto> GetById(string entityId, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntityDto entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(string entityId, CancellationToken cancellationToken = default);
    }
}
