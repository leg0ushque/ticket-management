using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.Common.Exceptions;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;

namespace TicketingSystem.DataAccess.Repositories
{
    public abstract class GenericMongoRepository<TEntity> : IMongoRepository<TEntity>
        where TEntity : class, IHasId
    {
        public const int DefaultVersion = 1;
        public const int NoItems = 0;

        public abstract string _collectionName { get; }

        protected readonly IMongoClient _client;
        protected readonly IMongoCollection<TEntity> _collection;

        protected GenericMongoRepository(IMongoDbFactory mongoDbFactory)
        {
            _client = mongoDbFactory.Client;
            _collection = mongoDbFactory.GetCollection<TEntity>(_collectionName);
        }

        public IMongoClient Client { get => _client; }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _collection.FindAsync(_ => true,
                cancellationToken: cancellationToken);

            return await items.ToListAsync(cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var item = await _collection.FindAsync(Builders<TEntity>.Filter.Eq("_id", id),
                cancellationToken: cancellationToken);

            return await item.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            var items = await _collection.FindAsync(Builders<TEntity>.Filter.Where(expression),
                cancellationToken: cancellationToken);

            return await items.ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> FilterAsync<TField>(Expression<Func<TEntity, TField>> field, IEnumerable<TField> values, CancellationToken cancellationToken = default)
        {
            var items = await _collection.FindAsync(Builders<TEntity>.Filter.In(field, values),
                cancellationToken: cancellationToken);

            return await items.ToListAsync(cancellationToken);
        }

        public Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.Version = 1;

            return _collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
        }

        public async Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default)
        {
            var version = entity.Version;
            var filter = Builders<TEntity>.Filter.Where(e => e.Id == id && e.Version == version);

            entity.Version++;

            var result = await _collection.ReplaceOneAsync(filter, entity,
                cancellationToken: cancellationToken);

            if (result.ModifiedCount == NoItems)
            {
                throw new OutdatedVersionException();
            }
        }

        public async Task UpdateAsync<TField>(string id, Expression<Func<TEntity, TField>> field, TField newValue, CancellationToken cancellationToken = default)
        {
            var item = await _collection.FindAsync(Builders<TEntity>.Filter.Eq("_id", id),
                cancellationToken: cancellationToken);

            var entity = await item.FirstOrDefaultAsync(cancellationToken);

            var version = entity.Version;
            var filter = Builders<TEntity>.Filter.Where(e => e.Id == id && e.Version == version);

            entity.Version++;

            var update = Builders<TEntity>.Update
                    .Set(field, newValue)
                    .Set(x => x.Version, entity.Version);

            var result = await _collection.UpdateOneAsync(filter, update,
                cancellationToken: cancellationToken);

            if (result.ModifiedCount == NoItems)
            {
                throw new OutdatedVersionException();
            }
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id), cancellationToken);
        }
    }
}
