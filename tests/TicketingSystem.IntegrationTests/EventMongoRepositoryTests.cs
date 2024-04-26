using AutoFixture;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;
using Xunit;

namespace TicketingSystem.IntegrationTests
{
    public class EventMongoRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly IFixture _fixture;
        private readonly IMongoRepository<Event> _repository;
        private Event _testEntity;

        public Event CreateEntity() => _fixture.Build<Event>()
            .With(e => e.Name)
            .With(e => e.Description)
            .With(e => e.StartTime)
            .With(e => e.EndTime)
            .Create();

        public EventMongoRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = new Fixture();
            _repository = fixture.EventRepositoryInstance;
            _testEntity = CreateEntity();
        }

        [Fact]
        public async Task ShouldCreateEntityAsync()
        {
            // Act
            await _repository.CreateAsync(_testEntity);

            // Assert
            var savedEntity = await _repository.GetByIdAsync(_testEntity.Id);

            savedEntity.Should().NotBeNull();
            savedEntity.Name.Should().Be(_testEntity.Name);

            await TearDown();
        }

        [Fact]
        public async Task ShouldGetAllEntitiesAsync()
        {
            // Arrange
            await _repository.CreateAsync(_testEntity);

            // Act
            var entities = await _repository.GetAllAsync();

            // Assert
            entities.Should().NotBeEmpty();

            await TearDown();
        }

        [Fact]
        public async Task ShouldUpdateEntityAsync()
        {
            // Arrange
            await _repository.CreateAsync(_testEntity);

            // Act
            await _repository.UpdateAsync(_testEntity.Id, _testEntity);

            // Assert
            var savedEntity = await _repository.GetByIdAsync(_testEntity.Id);
            savedEntity.Name.Should().BeEquivalentTo(_testEntity.Name);

            await TearDown();
        }

        [Fact]
        public async Task ShouldDeleteEntityAsync()
        {
            // Arrange
            await _repository.CreateAsync(_testEntity);

            // Act
            await _repository.DeleteAsync(_testEntity.Id);

            // Assert
            var deletedEntity = await _repository.GetByIdAsync(_testEntity.Id);
            deletedEntity.Should().BeNull();
        }

        private async Task TearDown()
        {
            await _repository.DeleteAsync(_testEntity.Id);
        }
    }
}
