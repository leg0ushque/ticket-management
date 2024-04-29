using System;
using TicketingSystem.BusinessLogic.Enums;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class EventDto : IDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public string VenueId { get; set; }

        public EventState State { get; set; }
    }
}
