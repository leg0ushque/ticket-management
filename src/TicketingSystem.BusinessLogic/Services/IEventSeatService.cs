using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IEventSeatService : IService<EventSeat, EventSeatDto>
    {
        Task UpdateEventSeatsStates(IList<string> eventSeatsIds, EventSeatState newState, CancellationToken cancellationToken = default);
    }
}
