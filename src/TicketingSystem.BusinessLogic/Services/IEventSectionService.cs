using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IEventSectionService : IService<EventSection, EventSectionDto>
    {
        public Task UpdateEventSeatsStateAsync(string eventId, SectionSeatsModel[] sectionSeatsList, EventSeatState fromState,EventSeatState toState,
            CancellationToken cancellationToken = default);

        public Task BookSeatsOfEventAsync(List<EventSectionSeatsModel> groupedItems,
            CancellationToken cancellationToken = default);

        public Task ExecuteBookingTransactionAsync(List<EventSectionSeatsModel> groupedItems,
            CancellationToken cancellationToken = default);

        public Task<EventSeatDto> UpdateEventSeatState(string seatId, string eventId, EventSeatState state,
            CancellationToken cancellationToken = default);

        public Task<EventSectionDto> GetSectionBySeatIdAsync(string seatId, string eventId = null,
            CancellationToken cancellationToken = default);

        public Task<List<EventSectionDto>> GetSectionsByEventIdAsync(string eventId,
            CancellationToken cancellationToken = default);

        public Task<List<EventSeatInfoModel>> GetSeatsInfo(string sectionId,
            CancellationToken cancellationToken = default);
    }
}
