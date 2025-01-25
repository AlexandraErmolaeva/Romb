using Moq;
using Microsoft.EntityFrameworkCore;
using Romb.Application.Entities;
using Xunit.Abstractions;
using Moq.EntityFrameworkCore;
using Romb.Application.Repositories;

namespace Romb.Application.Tests;

public class EventServiceTest
{
    private readonly Mock<AppDbContext> _mockDbContext;
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly EventRepository _eventRepository;

    public EventServiceTest(ITestOutputHelper outputHelper)
    {
        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _eventRepository = new EventRepository(_mockDbContext.Object);
        _testOutputHelper = outputHelper;
    }

    [Fact]
    public async Task GetAllEventsAsync_ReturnAllEvents()
    {
        var mockEntities = CreateEvents().AsQueryable();

        _mockDbContext.Setup(db => db.Events).ReturnsDbSet(mockEntities);

        var expectedEntities = await _eventRepository.GetAsync();

        AssertEntitiesCollectionMatch(mockEntities, expectedEntities);

        _mockDbContext.Reset();
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnEvent()
    {
        var mockEntities = CreateEvents().AsQueryable();

        _mockDbContext.Setup(db => db.Events).ReturnsDbSet(mockEntities);

        var idForGet = (long)mockEntities.Count() - 1;

        var mockEntity = mockEntities.First(e => e.Id == idForGet);

        var expectedEntity = await _eventRepository.GetByIdAsync(idForGet);

        AssertEntityMatch(mockEntity, expectedEntity);

        _mockDbContext.Reset();
    }

    private void AssertEntitiesCollectionMatch(IEnumerable<EventEntity> entities, IEnumerable<EventEntity> expectedEntities)
    {
        Assert.NotNull(expectedEntities);

        var isMatched = expectedEntities.Count() == entities.Count() &&
            expectedEntities.Zip(entities, (e, d) =>
            e.Id == d.Id && e.Name == d.Name && e.CofinanceRate == d.CofinanceRate &&
            e.TotalBudget == d.TotalBudget && e.LocalBudget == d.LocalBudget && e.RegionalBudget == d.RegionalBudget)
            .All(match => match);

        Assert.True(isMatched);
    }

    private void AssertEntityMatch(EventEntity entity, EventEntity expectedEntity)
    {
        Assert.NotNull(expectedEntity);

        var isMatched = entity.Id == expectedEntity.Id && entity.Name == expectedEntity.Name &&
            entity.TotalBudget == expectedEntity.TotalBudget && entity.LocalBudget == expectedEntity.LocalBudget &&
            entity.RegionalBudget == expectedEntity.RegionalBudget;

        Assert.True(isMatched);
    }

    private List<EventEntity> CreateEvents()
    {
        var entities = new List<EventEntity>();

        for (int i = 1; i <= 10; i++)
        {
            var entity = new EventEntity
            {
                Id = i,
                Name = "Event" + i,
                CofinanceRate = 100 - i,
                TotalBudget = 1000 * i
            };

            entity.RegionalBudget = entity.TotalBudget / 100 * entity.CofinanceRate;
            entity.LocalBudget = entity.TotalBudget - (entity.TotalBudget / 100 * entity.CofinanceRate);

            entities.Add(entity);
        }

        return entities;
    }
}