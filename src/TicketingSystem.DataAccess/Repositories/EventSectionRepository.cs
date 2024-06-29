using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.Common.Exceptions;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;

namespace TicketingSystem.DataAccess.Repositories
{
    public class EventSectionRepository : GenericMongoRepository<EventSection>, IEventSectionRepository
    {
        public EventSectionRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory)
        { }

        public override string _collectionName => "EventSections";

        public async Task UpdateAsync<TField>(IClientSessionHandle session,
            string id, Expression<Func<EventSection, TField>> field, TField newValue, long version,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<EventSection>.Filter.Where(e => e.Id == id && e.Version == version);

            var updateDefinition = Builders<EventSection>.Update
                    .Set(field, newValue)
                    .Inc(x => x.Version, 1);

            var updateOptions = new FindOneAndUpdateOptions<EventSection>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = false
            };

            var result = await _collection.FindOneAndUpdateAsync(session, filter, updateDefinition, updateOptions,
                cancellationToken);

            if (result == null)
            {
                throw new OutdatedVersionException();
            }
        }
    }
}
