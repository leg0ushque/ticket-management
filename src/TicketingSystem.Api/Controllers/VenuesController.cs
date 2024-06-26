﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.WebApi.Filters;

namespace TicketingSystem.WebApi.Controllers
{
    /// <summary>
    /// API to manipulate over Venues, Sections, Rows
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [BusinessLogicExceptionFilter]
    public class VenuesController(IVenueService venuesService)
        : ControllerBase
    {
        private readonly IVenueService _venuesService = venuesService;

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
            var sections = await _venuesService.GetVenueSectionsAsync(venueId);

            return Ok(sections);
        }
    }
}
