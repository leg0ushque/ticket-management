﻿using TicketingSystem.Common.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string CartId { get; set; }

        public UserRole Role { get; set; }
    }
}
