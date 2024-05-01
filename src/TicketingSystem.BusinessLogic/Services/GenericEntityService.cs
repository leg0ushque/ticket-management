using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class GenericEntityService<TEntity, TEntityDto>
        where TEntity : class, IHasId
        where TEntityDto : class, IDto
    {
        private protected readonly IMongoRepository<TEntity> _repository;
        private protected readonly IMapper _mapper;

        public GenericEntityService(IMongoRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default)
        {
            return _repository.CreateAsync(_mapper.Map<TEntity>(entity), cancellationToken);
        }

        public async Task<IReadOnlyCollection<TEntityDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<TEntityDto>>(
                    await _repository.GetAllAsync(cancellationToken));
        }

        public async Task<TEntityDto> GetByIdAsync(string entityId, CancellationToken cancellationToken = default)
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

        public Task UpdateAsync(TEntityDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var mappedEntity = _mapper.Map<TEntity>(entity);

                return _repository.UpdateAsync(entity.Id, mappedEntity, cancellationToken);
            }
            catch (ArgumentException)
            {
                throw new BusinessLogicException($"There is no {nameof(TEntity)} found by Id='{entity.Id}' to update.");
            }
        }

        public Task DeleteAsync(string entityId, CancellationToken cancellationToken = default)
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

        public async Task<IReadOnlyCollection<TEntityDto>> FilterAsync(Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<TEntityDto>>(
                    await _repository.FilterAsync(expression, cancellationToken))
                .AsReadOnly();
        }

        public async Task<IReadOnlyCollection<TEntityDto>> FilterAsync<TField>(Expression<Func<TEntity, TField>> field, IEnumerable<TField> values, CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<TEntityDto>>(await _repository.FilterAsync(field, values, cancellationToken))
                .AsReadOnly();
        }
    }
}
