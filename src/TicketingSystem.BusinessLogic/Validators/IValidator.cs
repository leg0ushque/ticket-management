using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;

namespace TicketingSystem.BusinessLogic.Validators
{
    public interface IValidator<in TEntityDto>
        where TEntityDto : class, IDto
    {
        public Task Validate(TEntityDto entity, CancellationToken cancellationToken = default);
    }
}
