using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.DataAccess.Repositories
{
    public class GenericMongoRepository<TEntity> : IMongoRepository<TEntity, string>
        where TEntity : IStringKeyEntity
    {
        private readonly IMongoCollection<TEntity> _collection;

        public GenericMongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(new BsonDocument())
                .ToListAsync(cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(Builders<TEntity>.Filter.Eq("_id", new ObjectId(id)))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
        }

        public async Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default)
        {
            await _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", new ObjectId(id)), entity,
                cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", new ObjectId(id)), cancellationToken);
        }
    }
}
