using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.EventsApi.Models;

namespace TicketingSystem.EventsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventSeatService _eventSeatService;
        private readonly IEventSectionService _eventSectionService;
        public EventsController(
            IEventService eventService,
            IEventSeatService eventSeatService,
            IEventSectionService eventSectionService,
            ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _eventSeatService = eventSeatService;
            _eventSectionService = eventSectionService;
        }
    }
}
