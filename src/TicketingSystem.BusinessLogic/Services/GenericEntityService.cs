using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Validators;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class GenericEntityService<TEntity, TEntityDto> : IService<TEntityDto>
        where TEntity : class, IHasId
        where TEntityDto : class, IDto
    {
        private protected readonly IValidator<TEntityDto> _validator;
        private protected readonly IMongoRepository<TEntity> _repository;
        private protected readonly IMapper _mapper;

        public GenericEntityService(IValidator<TEntityDto> validator, IMongoRepository<TEntity> repository, IMapper mapper)
        {
            _validator = validator;
            _repository = repository;
            _mapper = mapper;
        }

        public virtual Task CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default)
        {
            _validator.Validate(entity, cancellationToken);

            return _repository.CreateAsync(_mapper.Map<TEntity>(entity), cancellationToken);
        }

        public async virtual Task<IReadOnlyCollection<TEntityDto>> GetAllAsync(int? pageNumber = null, int? pageSize = null,
            CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<TEntityDto>>(
                    await _repository.GetAllAsync(cancellationToken))
                .AsReadOnly();
        }

        public async virtual Task<TEntityDto> GetById(string entityId, CancellationToken cancellationToken = default)
        {
            try
            {
                return _mapper.Map<TEntityDto>(
                    await _repository.GetByIdAsync(entityId, cancellationToken));
            }
            catch (ArgumentException)
            {
                throw new BusinessLogicException($"There is no {nameof(TEntity)} found by Id='{entityId}'.");
            }
        }

        public virtual Task UpdateAsync(TEntityDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _validator.Validate(entity, cancellationToken);

                var mappedEntity = _mapper.Map<TEntity>(entity);

                return _repository.UpdateAsync(entity.Id, mappedEntity, cancellationToken);
            }
            catch (ArgumentException)
            {
                throw new BusinessLogicException($"There is no {nameof(TEntity)} found by Id='{entity.Id}' to update.");
            }
        }

        public virtual Task DeleteAsync(string entityId, CancellationToken cancellationToken = default)
        {
            try
            {
                return _repository.DeleteAsync(entityId, cancellationToken);
            }
            catch (ArgumentException)
            {
                throw new BusinessLogicException($"There is no {nameof(TEntity)} found by Id='{entityId}' to delete.");
            }
        }
    }
}
