using TicketingSystem.BusinessLogic.Dtos;

namespace TicketingSystem.BusinessLogic.Validators
{
    public interface IValidator<in TEntityDto>
        where TEntityDto : class, IDto
    {
        public void Validate(TEntityDto entity);
    }
}
