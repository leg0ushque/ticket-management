using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IEventSeatService : IService<EventSeat, EventSeatDto>
    {
        Task UpdateEventSeatsStates(IList<string> eventSeatsIds, Enums.EventSeatState newState, CancellationToken cancellationToken = default);
    }
}
