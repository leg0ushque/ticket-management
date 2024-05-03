using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventSeatService(
        IMongoRepository<EventSeat> eventSeatRepository,
        IMongoRepository<CartItem> cartItemRepository,
        IMapper mapper)
        : GenericEntityService<EventSeat, EventSeatDto>(eventSeatRepository, mapper), IEventSeatService
    {
        private readonly IMongoRepository<CartItem> _cartItemRepository = cartItemRepository;

        public async Task UpdateEventSeatsStates(IList<string> eventSeatsIds, EventSeatState newState, CancellationToken cancellationToken = default)
        {
            if (eventSeatsIds is null || eventSeatsIds.Count == 0)
            {
                throw new BusinessLogicException("No seats to update");
            }

            foreach (var eventSeatId in eventSeatsIds)
            {
                await _repository.UpdateAsync(eventSeatId, s => s.State, _mapper.Map<EventSeatState>(newState),
                    cancellationToken: cancellationToken);
            }
        }

        public async Task BookSeatsInCart(string cartId)
        {
            var seats = (await _cartItemRepository.FilterAsync(s => s.CartId == cartId))?.Select(ci => ci.EventSeatId).ToList();

            await UpdateEventSeatsStates(seats ?? Enumerable.Empty<string>().ToList(), EventSeatState.Booked);
        }


        public async Task<List<EventSeatInfoModel>> GetSeatsInfo(EventSectionDto section, CancellationToken cancellationToken = default)
        {
            var seatsInfo = section.EventRows.SelectMany(er => er.EventSeatsIds, (er, seatId) => new
            {
                sectionId = section.Id,
                sectionClass = section.Class,
                sectionNumber = section.Number,
                rowNumber = er.Number,
                price = er.Price,
                seatId
            }).ToList();

            var result = new List<EventSeatInfoModel>();

            foreach (var data in seatsInfo)
            {
                var seat = await _repository.GetByIdAsync(data.seatId, cancellationToken);

                result.Add(new EventSeatInfoModel
                {
                    EventSectionId = data.sectionId,
                    EventSectionClass = data.sectionClass,
                    EventSectionNumber = data.sectionNumber,
                    EventRowNumber = data.rowNumber,
                    EventRowPrice = data.price,
                    EventSeatNumber = seat.Number,
                    EventSeatState = _mapper.Map<EventSeatState>(seat.State),
                });
            }

            return result;
        }
    }
}