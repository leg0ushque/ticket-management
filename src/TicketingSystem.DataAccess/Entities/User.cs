using TicketingSystem.DataAccess.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public UserRole Role { get; set; }
    }
    public class Payment : BaseEntity
    {
        public string[] CartItemIds { get; set; }

        public PaymentState State { get; set; }
    }
}
