using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.EventsApi.Models;

namespace TicketingSystem.EventsApi.Controllers
{
    /// <summary>
    /// API to manipulate over Events, EventSections, EventRows (inline entity), EventSeats, Tickets
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EventsController(
        IEventService eventService,
        IEventSeatService eventSeatService,
        IEventSectionService eventSectionService,
        IMapper mapper) : ControllerBase
    {
        private readonly IEventService _eventService = eventService;
        private readonly IEventSeatService _eventSeatService = eventSeatService;
        private readonly IEventSectionService _eventSectionService = eventSectionService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Returns a list of Venues with their info (no additional entities attached)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _eventService.GetAllAsync();

            return Ok(events);
        }

        /// <summary>
        /// Returns a list of Event Seats (with Event Section id, Event Row Id) with their statuses and prices
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{eventId}/sections/{sectionId}/seats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFullSeats([FromRoute] string eventId, [FromRoute] string sectionId)
        {
            var foundEvent = await _eventService.GetByIdAsync(eventId);

            if (foundEvent is null)
            {
                return NotFound(nameof(eventId));
            }

            var section = await _eventSectionService.GetByIdAsync(sectionId);

            if (section is null)
            {
                return NotFound(nameof(sectionId));
            }

            return Ok(await GetSeatsInfo(section));
        }

        private async Task<List<EventSeatInfoModel>> GetSeatsInfo(EventSectionDto section)
        {
            var seatsInfo = section.EventRows.SelectMany(er => er.EventSeatsIds, (er, seatId) => new
            {
                sectionId = section.Id,
                sectionClass = section.Class,
                sectionNumber = section.Number,
                rowNumber = er.Number,
                price = er.Price,
                seatId
            })
                .ToList();

            var result = new List<EventSeatInfoModel>();

            foreach (var data in seatsInfo)
            {
                var seat = await _eventSeatService.GetByIdAsync(data.seatId);

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
