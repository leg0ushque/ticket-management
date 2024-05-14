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
    public class GenericEntityService<TEntity, TEntityDto>(IMongoRepository<TEntity> repository, IMapper mapper)
        where TEntity : class, IHasId
        where TEntityDto : class, IDto
    {
        private protected readonly IMongoRepository<TEntity> _repository = repository;
        private protected readonly IMapper _mapper = mapper;

        /// <summary>
        /// Creates an <see cref="TEntity"/> entity with data from <see cref="TEntityDto"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BusinessLogicException"></exception>
        public Task CreateAsync(TEntityDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                return _repository.CreateAsync(_mapper.Map<TEntity>(entity), cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieves <see cref="IReadOnlyCollection{TEntity}"> of <see cref="TEntity"/> entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BusinessLogicException"></exception>
        public async Task<IReadOnlyCollection<TEntityDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return _mapper.Map<List<TEntityDto>>(
                        await _repository.GetAllAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieves a <see cref="TEntityDto"> of <see cref="TEntity"/> by its ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BusinessLogicException"></exception>
        public async Task<TEntityDto> GetByIdAsync(string entityId, CancellationToken cancellationToken = default)
        {
            var foundEntity = await GetEntityById(entityId, cancellationToken);

            return foundEntity == null
                ? throw new BusinessLogicException($"{nameof(TEntity)} wasn't found by ID {entityId}", code: Common.Enums.ErrorCode.NotFound)
                : _mapper.Map<TEntityDto>(foundEntity);
        }

        public Task UpdateAsync(TEntityDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var mappedEntity = _mapper.Map<TEntity>(entity);

                return _repository.UpdateAsync(entity.Id, mappedEntity, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        public Task UpdateAsync<TField>(string id, Expression<Func<TEntity, TField>> field, TField newValue, CancellationToken cancellationToken = default)
        {
            try
            {
                return _repository.UpdateAsync(id, field, newValue, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        public Task DeleteAsync(string entityId, CancellationToken cancellationToken = default)
        {
            try
            {
                return _repository.DeleteAsync(entityId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        public async Task<IReadOnlyCollection<TEntityDto>> FilterAsync(Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return _mapper.Map<List<TEntityDto>>(
                        await _repository.FilterAsync(expression, cancellationToken))
                    .AsReadOnly();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        public async Task<IReadOnlyCollection<TEntityDto>> FilterAsync<TField>(Expression<Func<TEntity, TField>> field, IEnumerable<TField> values, CancellationToken cancellationToken = default)
        {
            try
            {
                return _mapper.Map<List<TEntityDto>>(await _repository.FilterAsync(field, values, cancellationToken))
                    .AsReadOnly();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        private async Task<TEntity> GetEntityById(string entityId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _repository.GetByIdAsync(entityId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }
    }
}
