using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;
using TicketingSystem.BusinessLogic.Options;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;
using Microsoft.Extensions.Options;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventService(
        IMongoRepository<Event> repository,
        IMapper mapper,
        IMemoryCache cache,
        IOptions<CacheOptions> cacheOptions)
        : GenericEntityService<Event, EventDto>(repository, mapper), IEventService
    {
        private const string AllEventsKey = "AllEvents";
        private const string EventKey = "Event-";

        private readonly IMemoryCache _cache = cache;
        private readonly TimeSpan _slidingExpirationTimeSpan = TimeSpan.FromMinutes(cacheOptions.Value.SlidingExpiration);

        public override Task CreateAsync(EventDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                return _repository.CreateAsync(_mapper.Map<Event>(entity), cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
            finally
            {
                InvalidateCache($"{EventKey}{entity.Id}");
            }
        }

        public override async Task<IReadOnlyCollection<EventDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _cache.GetOrCreateAsync(AllEventsKey, async entry =>
                {
                    entry.SetSlidingExpiration(_slidingExpirationTimeSpan);

                    return _mapper.Map<List<EventDto>>(
                            await _repository.GetAllAsync(cancellationToken));
                }) ?? [];
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Retrieves a <see cref="EventDto"> of <see cref="Event"/> by its ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BusinessLogicException"></exception>
        public override async Task<EventDto> GetByIdAsync(string entityId, CancellationToken cancellationToken = default)
        {
            var result = await _cache.GetOrCreateAsync($"{EventKey}{entityId}", async entry =>
            {
                entry.SetSlidingExpiration(_slidingExpirationTimeSpan);

                var foundEntity = await GetEntityById(entityId, cancellationToken);

                if (foundEntity == null)
                {
                    ThrowBusinessLogicException(entityId);
                }

                return _mapper.Map<EventDto>(foundEntity);
            });

            if (result == null)
            {
                ThrowBusinessLogicException(entityId);
            }

            return result;

            static void ThrowBusinessLogicException(string eventId)
                => throw new BusinessLogicException($"{nameof(Event)} wasn't found by ID {eventId}",
                code: Common.Enums.ErrorCode.NotFound);
        }

        public override Task UpdateAsync(EventDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var mappedEntity = _mapper.Map<Event>(entity);

                return _repository.UpdateAsync(entity.Id, mappedEntity, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
            finally
            {
                InvalidateCache($"{EventKey}{entity.Id}");
            }
        }

        public override Task UpdateAsync<TField>(string id, Expression<Func<Event, TField>> field, TField newValue, long version, CancellationToken cancellationToken = default)
        {
            try
            {
                return _repository.UpdateAsync(id, field, newValue, version, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
            finally
            {
                InvalidateCache($"{EventKey}{id}");
            }
        }

        public override Task DeleteAsync(string entityId, CancellationToken cancellationToken = default)
        {
            try
            {
                return _repository.DeleteAsync(entityId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException(ex.Message, ex);
            }
            finally
            {
                InvalidateCache($"{EventKey}{entityId}");
            }
        }

        private void InvalidateCache(object key)
        {
            _cache.Remove(key);
        }
    }
}
