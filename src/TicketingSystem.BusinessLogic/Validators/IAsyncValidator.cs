using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;

namespace TicketingSystem.BusinessLogic.Validators
{
    public interface IAsyncValidator<in TEntityDto>
        where TEntityDto : class, IDto
    {
        public Task ValidateAsync(TEntityDto entity, CancellationToken cancellationToken = default);
    }
}
