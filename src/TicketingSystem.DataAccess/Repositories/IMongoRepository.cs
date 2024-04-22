using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.DataAccess.Repositories
{
    public interface IMongoRepository<TEntity> : IRepository<TEntity, string>
        where TEntity : IStringKeyEntity
    { }
}
