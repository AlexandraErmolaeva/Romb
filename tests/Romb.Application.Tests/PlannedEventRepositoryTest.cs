using Moq;
using Microsoft.EntityFrameworkCore;
using Romb.Application.Entities;
using Xunit.Abstractions;
using Moq.EntityFrameworkCore;
using Romb.Application.Repositories;

namespace Romb.Application.Tests;

public class PlannedEventRepositoryTest
{
    private readonly Mock<AppDbContext> _mockDbContext;
    private readonly PlannedEventRepository _eventRepository;

    public PlannedEventRepositoryTest(ITestOutputHelper outputHelper)
    {
        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _eventRepository = new PlannedEventRepository(_mockDbContext.Object);
    }

    [Fact]
    public async Task GetAllEventsAsync_ReturnAllEvents()
    {
        var mockEntities = CreateEntities().AsQueryable();

        _mockDbContext.Setup(db => db.PlannedEvents).ReturnsDbSet(mockEntities);

        var expectedEntities = await _eventRepository.GetAsync();

        AssertEntitiesCollectionMatch(mockEntities, expectedEntities);

        _mockDbContext.Reset();
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnEvent()
    {
        var mockEntities = CreateEntities().AsQueryable();

        _mockDbContext.Setup(db => db.PlannedEvents).ReturnsDbSet(mockEntities);

        var idForGet = (long)mockEntities.Count() - 1;

        var mockEntity = mockEntities.First(e => e.Id == idForGet);

        var expectedEntity = await _eventRepository.GetByIdAsync(idForGet);

        AssertEntityMatch(mockEntity, expectedEntity);

        _mockDbContext.Reset();
    }

    private void AssertEntitiesCollectionMatch(IEnumerable<PlannedEventEntity> entities, IEnumerable<PlannedEventEntity> expectedEntities)
    {
        Assert.NotNull(expectedEntities);

        var isMatched = expectedEntities.Count() == entities.Count() &&
            expectedEntities.Zip(entities, (e, d) =>
            e.Id == d.Id && e.Name == d.Name && e.PlannedCofinanceRate == d.PlannedCofinanceRate &&
            e.TotalBudget == d.TotalBudget && e.PlannedLocalBudget == d.PlannedLocalBudget && e.PlannedRegionalBudget == d.PlannedRegionalBudget)
            .All(match => match);

        Assert.True(isMatched);
    }

    private void AssertEntityMatch(PlannedEventEntity entity, PlannedEventEntity expectedEntity)
    {
        Assert.NotNull(expectedEntity);

        var isMatched = entity.Id == expectedEntity.Id && entity.Name == expectedEntity.Name &&
            entity.TotalBudget == expectedEntity.TotalBudget && entity.PlannedLocalBudget == expectedEntity.PlannedLocalBudget &&
            entity.PlannedRegionalBudget == expectedEntity.PlannedRegionalBudget;

        Assert.True(isMatched);
    }

    private List<PlannedEventEntity> CreateEntities()
    {
        var entities = new List<PlannedEventEntity>();

        for (int i = 1; i <= 10; i++)
        {
            var entity = new PlannedEventEntity
            {
                Id = i,
                Name = "Event" + i,
                PlannedCofinanceRate = 100 - i,
                TotalBudget = 1000 * i
            };

            entity.PlannedRegionalBudget = entity.TotalBudget / 100 * entity.PlannedCofinanceRate;
            entity.PlannedLocalBudget = entity.TotalBudget - (entity.TotalBudget / 100 * entity.PlannedCofinanceRate);

            entities.Add(entity);
        }

        return entities;
    }
}