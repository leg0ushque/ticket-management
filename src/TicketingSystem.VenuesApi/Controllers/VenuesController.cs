using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Services;

namespace TicketingSystem.VenuesApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venuesService;
        private readonly ISectionService _sectionsService;
        private readonly ILogger<VenuesController> _logger;
        public VenuesController(
            IVenueService venuesService,
            ISectionService sectionsService,
            ILogger<VenuesController> logger)
        {
            _venuesService = venuesService;
            _sectionsService = sectionsService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public async Task<JsonResult> GetVenues()
        {
            var venues = await _venuesService.GetAllAsync();

            return new JsonResult(venues);
        }

        [HttpGet]
        [Route("{id}/sections")]
        public async Task<JsonResult> GetVenueSections([FromRoute] string id)
        {
            var sections = await _sectionsService.FilterAsync(x => x.VenueId == id);

            return new JsonResult(sections);
        }
    }
}
