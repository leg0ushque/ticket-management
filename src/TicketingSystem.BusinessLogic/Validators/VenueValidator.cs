using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class VenueValidator : IValidator<VenueDto>
    {
        public void Validate(VenueDto entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                throw new BusinessLogicException("Name must not be empty.");
            }

            if (string.IsNullOrEmpty(entity.Address))
            {
                throw new BusinessLogicException("Address must not be empty.");
            }
        }
    }
}
