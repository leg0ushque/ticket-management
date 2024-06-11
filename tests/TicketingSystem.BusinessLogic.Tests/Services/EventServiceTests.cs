using Amazon.Runtime.Internal.Util;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Options;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;
using Xunit;
using Opts = Microsoft.Extensions.Options.Options;

namespace TicketingSystem.BusinessLogic.Tests.Services
{
    public class EventServiceTests
    {
        private readonly List<EventDto> _events;
        private readonly int _selectedIndex;

        private readonly Mock<IMongoRepository<Event>> _mongoRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Fixture _fixture;

        private Mock<ICacheEntry> _cacheEntryMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private IOptions<CacheOptions> _cacheOptions;

        private readonly IEventService _service;

        public EventServiceTests()
        {
            _fixture = new Fixture();

            _events = _fixture.CreateMany<EventDto>(5).ToList();
            _selectedIndex = 1;

            _cacheOptions = Opts.Create(new CacheOptions());

            _mapperMock = new Mock<IMapper>();
            _mapperMock
                .Setup(m => m.Map<List<EventDto>>(It.IsAny<List<Event>>()))
                .Returns(_events);
            _mapperMock
                .Setup(m => m.Map<EventDto>(It.IsAny<Event>()))
                .Returns(_events[_selectedIndex]);

            _mongoRepositoryMock = new Mock<IMongoRepository<Event>>();
            _mongoRepositoryMock
                .Setup(m => m.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event());

            _memoryCacheMock = new Mock<IMemoryCache>();

            SetupCacheMock();

            _service = new EventService(_mongoRepositoryMock.Object,
                _mapperMock.Object, _memoryCacheMock.Object, _cacheOptions);
        }

        [Fact]
        public async Task CreateAsync_ShouldInvalidateCache()
        {
            // Arrange
            var entityDto = CreateEvent();

            // Act
            await _service.CreateAsync(entityDto);

            // Assert
            _memoryCacheMock.Verify(x => x.Remove(It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldInvalidateCache()
        {
            // Arrange
            var entityDto = CreateEvent();

            // Act
            await _service.UpdateAsync(entityDto);

            // Assert
            _memoryCacheMock.Verify(x => x.Remove(It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldInvalidateCache()
        {
            // Arrange
            var entity = CreateEvent();

            // Act
            await _service.DeleteAsync(entity.Id);

            // Assert
            _memoryCacheMock.Verify(x => x.Remove(It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldCallGetOrCreateAsync()
        {
            // Act
            var result = await _service.GetAllAsync();

            // Assert
            _memoryCacheMock.Verify(x => x.Remove(It.IsAny<object>()),
                Times.Never);

            _mapperMock.Verify(x => x.Map<List<EventDto>>(It.IsAny<object>()),
                Times.Once);
            result.Should().BeEquivalentTo(_events);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldCallGetOrCreateAsync()
        {
            // Act
            var result = await _service.GetByIdAsync(_events[_selectedIndex].Id);

            // Assert
            _memoryCacheMock.Verify(x => x.Remove(It.IsAny<object>()),
                Times.Never);

            _mapperMock.Verify(x => x.Map<EventDto>(It.IsAny<object>()),
                Times.Once);
            result.Should().BeEquivalentTo(_events[_selectedIndex]);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCachedData()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());

            var service = new EventService(_mongoRepositoryMock.Object,
                _mapperMock.Object, cache, _cacheOptions);

            // Act
            var firstCallResult = await service.GetByIdAsync(_events[_selectedIndex].Id);
            var secondCallResult = await service.GetByIdAsync(_events[_selectedIndex].Id);

            // Assert
            firstCallResult.Should().BeEquivalentTo(_events[_selectedIndex]);
            secondCallResult.Should().BeEquivalentTo(_events[_selectedIndex]);

            _mapperMock.Verify(repo => repo.Map<EventDto>(It.IsAny<Event>()),
                Times.Once);
        }

        private EventDto CreateEvent()
            => _fixture.Create<EventDto>();

        private void SetupCacheMock()
        {
            _memoryCacheMock = new Mock<IMemoryCache>();
            _cacheEntryMock = new Mock<ICacheEntry>();

            object valuePayload = null;
            _cacheEntryMock
                .SetupSet(mce => mce.Value = It.IsAny<object>())
                .Callback<object>(v => valuePayload = v);

            TimeSpan? slidingExpiration = TimeSpan.FromMinutes(1);
            _cacheEntryMock
                .SetupSet(mce => mce.SlidingExpiration = It.IsAny<TimeSpan?>())
                .Callback<TimeSpan?>(dto => slidingExpiration = dto);

            _memoryCacheMock
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(false);
                    _memoryCacheMock
                        .Setup(x => x.CreateEntry(It.IsAny<object>()))
                        .Returns(_cacheEntryMock.Object);
        }
    }
}
