using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventSeatService(IMongoRepository<EventSeat> repository, IMapper mapper)
        : GenericEntityService<EventSeat, EventSeatDto>(repository, mapper), IEventSeatService
    {
        public async Task UpdateEventSeatsStates(IList<string> eventSeatsIds, EventSeatState newState, CancellationToken cancellationToken = default)
        {
            foreach (var eventSeatId in eventSeatsIds)
            {
                await _repository.UpdateAsync(eventSeatId, s => s.State, _mapper.Map<EventSeatState>(newState),
                    cancellationToken: cancellationToken);
            }
        }
    }
}
