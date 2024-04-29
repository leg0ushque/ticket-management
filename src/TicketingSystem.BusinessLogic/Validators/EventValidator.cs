using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class EventValidator : IValidator<EventDto>
    {
        private readonly IMongoRepository<Event> _repository;

        public EventValidator(IMongoRepository<Event> repository)
        {
            _repository = repository;
        }

        public Task Validate(EventDto entity, CancellationToken cancellationToken = default)
        {
            return;
        }
    }
}
