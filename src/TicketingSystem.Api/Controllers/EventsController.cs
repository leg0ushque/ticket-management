using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.WebApi.Filters;

namespace TicketingSystem.WebApi.Controllers
{
    /// <summary>
    /// API to manipulate over Events, EventSections, EventRows (inline entity), EventSeats, Tickets
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [OutdatedVersionExceptionFilter]
    public class EventsController(
        IEventService eventService,
        IEventSectionService eventSectionService) : ControllerBase
    {
        private const int CacheSecondsDuration = 60;

        private readonly IEventService _eventService = eventService;
        private readonly IEventSectionService _eventSectionService = eventSectionService;

        /// <summary>
        /// Returns a list of Venues with their info (no additional entities attached)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = CacheSecondsDuration, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _eventService.GetAllAsync();

            return Ok(events);
        }

        /// <summary>
        /// Returns a list of Event Sections by EventId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{eventId}/sections")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = CacheSecondsDuration, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetEventsSections([FromRoute] string eventId)
        {
            var eventSections = await _eventSectionService.GetSectionsByEventIdAsync(eventId);

            return Ok(eventSections);
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
        [ResponseCache(Duration = CacheSecondsDuration, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetFullSeats([FromRoute] string eventId, [FromRoute] string sectionId)
        {
            _ = await _eventService.GetByIdAsync(eventId);

            return Ok(await _eventSectionService.GetSeatsInfo(sectionId));
        }
    }
}
