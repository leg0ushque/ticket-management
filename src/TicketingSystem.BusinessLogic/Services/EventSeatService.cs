using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventSeatService : GenericEntityService<EventSeat, EventSeatDto>, IEventSeatService
    {
        public EventSeatService(IMongoRepository<EventSeat> repository, IMapper mapper) : base(repository, mapper)
        { }

        public async Task UpdateEventSeatsStates(IList<string> eventSeatsIds, Enums.EventSeatState newState, CancellationToken cancellationToken = default)
        {
            foreach (var eventSeatId in eventSeatsIds)
            {
                await _repository.UpdateAsync(eventSeatId, s => s.State, _mapper.Map<DataAccess.Enums.EventSeatState>(newState),
                    cancellationToken: cancellationToken);
            }
        }
    }
}
