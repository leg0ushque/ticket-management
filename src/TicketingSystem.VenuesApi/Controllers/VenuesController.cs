using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Services;

namespace TicketingSystem.VenuesApi.Controllers
{
    /// <summary>
    /// API to manipulate over Venues, Sections, Rows
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venuesService;
        private readonly ISectionService _sectionsService;
        public VenuesController(
            IVenueService venuesService,
            ISectionService sectionsService)
        {
            _venuesService = venuesService;
            _sectionsService = sectionsService;
        }

        /// <summary>
        /// Returns a list of Venues with their info (no additional entities attached)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVenues()
        {
            var venues = await _venuesService.GetAllAsync();

            return Ok(venues);
        }

        /// <summary>
        /// Returns all sections for venue
        /// </summary>
        /// <param name="venueId">The ID of venue to retrieve data about</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{venueId}/sections")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVenueSections([FromRoute] string venueId)
        {
            var venue = await _venuesService.GetByIdAsync(venueId);
            if(venue == null)
            {
                return NotFound();
            }

            var sections = await _sectionsService.FilterAsync(x => x.VenueId == venueId);

            return Ok(sections);
        }
    }
}
