using TicketingSystem.DataAccess.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class User : IStringKeyEntity
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public UserRole Role { get; set; }
    }
}
