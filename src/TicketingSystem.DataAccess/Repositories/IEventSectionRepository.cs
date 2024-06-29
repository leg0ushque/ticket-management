using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using TicketingSystem.DataAccess.Entities;
using MongoDB.Driver;

namespace TicketingSystem.DataAccess.Repositories
{
    public interface IEventSectionRepository : IMongoRepository<EventSection>
    {
        public Task UpdateAsync<TField>(IClientSessionHandle session, string id,
            Expression<Func<EventSection, TField>> field, TField newValue,
            long version,
            CancellationToken cancellationToken = default);
    }
}
