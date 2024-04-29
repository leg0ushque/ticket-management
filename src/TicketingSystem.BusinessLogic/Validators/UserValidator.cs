using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class UserValidator : IAsyncValidator<UserDto>
    {
        private readonly IMongoRepository<User> _repository;

        public UserValidator(IMongoRepository<User> repository)
        {
            _repository = repository;
        }

        public Task ValidateAsync(UserDto entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entity.FirstName))
            {
                throw new BusinessLogicException("First Name must not be empty.");
            }

            if (string.IsNullOrEmpty(entity.LastName))
            {
                throw new BusinessLogicException("Last Name must not be empty.");
            }

            if (string.IsNullOrEmpty(entity.LastName))
            {
                throw new BusinessLogicException("Last Name must not be empty.");
            }

            return CheckEmailIsUnique(entity, cancellationToken);
        }

        private async Task CheckEmailIsUnique(UserDto entity, CancellationToken cancellationToken = default)
        {
            var result = await _repository.FilterAsync(t => t.Email == entity.Email && t.Id != entity.Id, cancellationToken);

            if (result.Any())
            {
                throw new BusinessLogicException("Email is already taken.");
            }
        }
    }
}
