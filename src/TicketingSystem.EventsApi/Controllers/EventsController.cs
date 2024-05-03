using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Services;

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
        IEventSectionService eventSectionService) : ControllerBase
    {
        private readonly IEventService _eventService = eventService;
        private readonly IEventSeatService _eventSeatService = eventSeatService;
        private readonly IEventSectionService _eventSectionService = eventSectionService;

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
            _ = await _eventService.GetByIdAsync(eventId);

            var section = await _eventSectionService.GetByIdAsync(sectionId);

            return Ok(await _eventSeatService.GetSeatsInfo(section));
        }
    }
}
