using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;

namespace TicketingSystem.DataAccess.Repositories
{
    public abstract class GenericMongoRepository<TEntity> : IMongoRepository<TEntity>
        where TEntity : class, IHasId
    {
        public abstract string _collectionName { get; }

        private readonly IMongoCollection<TEntity> _collection;

        protected GenericMongoRepository(IMongoDbFactory mongoDbFactory)
        {
            _collection = mongoDbFactory.GetCollection<TEntity>(_collectionName);
        }

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

            return _collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
        }

        public Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default)
        {
            return _collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", id), entity,
                cancellationToken: cancellationToken);
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id), cancellationToken);
        }
    }
}
