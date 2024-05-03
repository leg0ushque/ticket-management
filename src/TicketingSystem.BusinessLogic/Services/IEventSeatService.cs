using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IEventSeatService : IService<EventSeat, EventSeatDto>
    {
        public Task<List<EventSeatInfoModel>> GetSeatsInfo(EventSectionDto section, CancellationToken cancellationToken = default);

        public Task BookSeatsInCart(string cartId);

        public Task UpdateEventSeatsStates(IList<string> eventSeatsIds, EventSeatState newState,
            CancellationToken cancellationToken = default);
    }
}
