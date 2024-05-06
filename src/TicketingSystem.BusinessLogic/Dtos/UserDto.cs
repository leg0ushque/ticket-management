using TicketingSystem.Common.Enums;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class UserDto : IDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CartId { get; set; }

        public string Email { get; set; }

        public UserRole Role { get; set; }
    }
}
