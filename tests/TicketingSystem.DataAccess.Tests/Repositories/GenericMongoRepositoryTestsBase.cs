using AutoFixture;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;
using Xunit;

namespace TicketingSystem.UnitTests
{
    public abstract class GenericMongoRepositoryTestsBase<TEntity>
        where TEntity : class, IHasId
    {
        protected string newValue;

        protected IFixture fixture;

        protected List<TEntity> entities;

        protected Mock<IAsyncCursor<TEntity>> asyncCursorMock;
        protected Mock<IMongoDbFactory> mongoDbFactoryMock;
        protected Mock<IMongoCollection<TEntity>> mongoCollectionMock;

        protected IMongoRepository<TEntity> repository;

        public abstract IMongoRepository<TEntity> CreateRepository(IMongoDbFactory factory);

        public void Setup()
        {
            fixture = new Fixture();

            entities = fixture.Build<TEntity>().With(x => x.Id, Guid.NewGuid().ToString()).CreateMany<TEntity>(5).ToList();

            asyncCursorMock = new Mock<IAsyncCursor<TEntity>>();
            asyncCursorMock.Setup(_ => _.Current).Returns(entities);
            asyncCursorMock
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            asyncCursorMock
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            mongoDbFactoryMock = new Mock<IMongoDbFactory>();
            mongoCollectionMock = new Mock<IMongoCollection<TEntity>>();

            // GETs, FILTERs setups

            mongoCollectionMock.Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<TEntity>>(),
                    It.IsAny<FindOptions<TEntity, TEntity>>(),
                    It.IsAny<CancellationToken>()))
              .ReturnsAsync(asyncCursorMock.Object);

            // CREATE setup

            mongoCollectionMock.Setup(x =>
                x.InsertOneAsync(
                    It.IsAny<TEntity>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback<TEntity, InsertOneOptions, CancellationToken>((entity, options, ct) =>
                {
                    entities.Add(entity);
                });

            // UPDATE setups

            var replaceOneResultMock = new Mock<ReplaceOneResult>();
            replaceOneResultMock.SetupGet(r => r.ModifiedCount).Returns(1);

            mongoCollectionMock.Setup(x =>
                x.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<TEntity>>(),
                    It.IsAny<TEntity>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(replaceOneResultMock.Object))
                .Callback<FilterDefinition<TEntity>, TEntity, ReplaceOptions, CancellationToken>((filter, e, options, ct) =>
                {
                    entities.RemoveAll(x => x.Id == entities.FirstOrDefault().Id);
                    entities.Add(e);
                });

            newValue = Guid.NewGuid().ToString();

            var updatedEntity = entities.FirstOrDefault();

            var updateResultMock = new Mock<UpdateResult>();
            updateResultMock.SetupGet(r => r.ModifiedCount).Returns(1);

            mongoCollectionMock.Setup(x =>
                    x.UpdateOneAsync(
                        It.IsAny<FilterDefinition<TEntity>>(),
                        It.IsAny<UpdateDefinition<TEntity>>(),
                        It.IsAny<UpdateOptions>(),
                        It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(updateResultMock.Object))
                .Callback(() =>
                    {
                        updatedEntity.Id = newValue;
                        updatedEntity.Version += 1;
                    });

            // DELETE setup

            mongoCollectionMock.Setup(x =>
                x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<TEntity>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Mock<DeleteResult>().Object))
                .Callback<FilterDefinition<TEntity>, CancellationToken>((filter, ct) =>
                {
                    entities.Remove(entities.FirstOrDefault());
                });

            mongoDbFactoryMock.Setup(x => x.GetCollection<TEntity>(It.IsAny<string>()))
                .Returns(mongoCollectionMock.Object);

            repository = CreateRepository(mongoDbFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_WhenInvoked_ShouldCallFindAsync()
        {
            var result = await repository.GetAllAsync();

            mongoCollectionMock.Verify(x =>
                x.FindAsync(
                    It.Is<ExpressionFilterDefinition<TEntity>>(_ => true),
                    It.IsAny<FindOptions<TEntity, TEntity>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            result.Should().BeEquivalentTo(entities);
        }

        [Fact]
        public async Task GetByIdAsync_WhenInvoked_ShouldCallFindAsync()
        {
            var id = Guid.NewGuid().ToString();

            var result = await repository.GetByIdAsync(id);

            mongoCollectionMock.Verify(x =>
                x.FindAsync(
                    It.IsAny<FilterDefinition<TEntity>>(),
                    It.IsAny<FindOptions<TEntity, TEntity>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            result.Should().BeEquivalentTo(entities.FirstOrDefault());
        }

        [Fact]
        public async Task FilterAsyncWithExpression_WhenInvoked_ShouldCallFindAsync()
        {
            Expression<Func<TEntity, bool>> expression = (x => x.Id == entities.FirstOrDefault().Id);

            var result = await repository.FilterAsync(expression);

            mongoCollectionMock.Verify(x =>
                x.FindAsync(
                    It.IsAny<ExpressionFilterDefinition<TEntity>>(),
                    It.IsAny<FindOptions<TEntity, TEntity>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            result.Should().BeEquivalentTo(entities);
        }

        [Fact]
        public async Task FilterAsyncByFieldValues_WhenInvoked_ShouldCallFindAsync()
        {
            var ids = new[] { entities[0].Id, entities[1].Id };

            Expression<Func<TEntity, string>> expression = (x => x.Id);

            var result = await repository.FilterAsync(expression, values: ids);

            mongoCollectionMock.Verify(x =>
                x.FindAsync(
                    It.IsAny<FilterDefinition<TEntity>>(),
                    It.IsAny<FindOptions<TEntity, TEntity>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            result.Should().BeEquivalentTo(entities);
        }

        [Fact]
        public async Task CreateAsync_WhenInvoked_ShouldCallInsertOneAsync()
        {
            var beforeCountValue = entities.Count;

            var newEntity = fixture.Create<TEntity>();

            await repository.CreateAsync(newEntity);

            mongoCollectionMock.Verify(x =>
                x.InsertOneAsync(
                    It.IsAny<TEntity>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            entities.Should().HaveCount(beforeCountValue + 1);

            entities.Should().ContainEquivalentOf(newEntity);
        }

        [Fact]
        public async Task UpdateAsync_WhenInvoked_ShouldCallReplaceOneAsync()
        {
            var existingEntity = entities.FirstOrDefault();
            var updatedEntity = fixture.Build<TEntity>()
                .With(x => x.Id, existingEntity.Id)
                .With(x => x.Version, existingEntity.Version + 1)
                .Create();

            await repository.UpdateAsync(existingEntity.Id, updatedEntity);

            mongoCollectionMock.Verify(x =>
                x.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<TEntity>>(),
                    It.IsAny<TEntity>(),
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            entities.Should().ContainEquivalentOf(updatedEntity);
        }

        [Fact]
        public async Task UpdateAsync_Field_WhenInvoked_ShouldCallFindOneAndUpdateAsync()
        {
            var id = entities.FirstOrDefault().Id;
            Expression<Func<TEntity, string>> fieldExpression = x => x.Id;

            await repository.UpdateAsync(id, fieldExpression, newValue);

            mongoCollectionMock.Verify(x =>
                    x.UpdateOneAsync(
                        It.IsAny<FilterDefinition<TEntity>>(),
                        It.IsAny<UpdateDefinition<TEntity>>(),
                        It.IsAny<UpdateOptions>(),
                        It.IsAny<CancellationToken>()),
                Times.Once);

            entities.FirstOrDefault().Id.Should().Be(newValue);
        }

        [Fact]
        public async Task DeleteAsync_WhenInvoked_ShouldCallDeleteOneAsync()
        {
            var existingEntity = entities.FirstOrDefault();

            await repository.DeleteAsync(existingEntity.Id);

            mongoCollectionMock.Verify(x =>
                x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<TEntity>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            entities.Should().NotContain(existingEntity);
        }
    }
}