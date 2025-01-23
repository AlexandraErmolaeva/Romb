using Moq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Romb.Application.Helpers;
using Romb.Application.Entities;
using Romb.Application.Mappers;
using Romb.Application.Services;
using Xunit.Abstractions;
using Romb.Application.Dtos;
using Moq.EntityFrameworkCore;

namespace Romb.Application.Tests;

public class EventServiceTest
{
    private readonly Mock<AppDbContext> _mockDbContext;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<EventService>> _mockLogger;
    private readonly IBudgetCalculator _budgetCalculator;
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly EventService _eventService;

    public EventServiceTest(ITestOutputHelper outputHelper)
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<EventMappingProfile>());

        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _mapper = config.CreateMapper();
        _mockLogger = new Mock<ILogger<EventService>>();
        _budgetCalculator = new BudgetCalculator();
        _eventService = new EventService(_mockDbContext.Object, _mapper, _budgetCalculator, _mockLogger.Object);
        _testOutputHelper = outputHelper;
    }

    [Fact]
    public async Task GetAllEventsAsync_ReturnAllEvents()
    {
        var entities = CreateEvents().AsQueryable();

        _mockDbContext.Setup(db => db.Events).ReturnsDbSet(entities);

        var dtos = await _eventService.GetAsync();

        AssertEntitiesAndDtosCollectionMatch(entities, dtos);

        _mockDbContext.Reset();
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnEvent()
    {
        var eventEntities = CreateEvents().AsQueryable();

        _mockDbContext.Setup(db => db.Events).ReturnsDbSet(eventEntities);

        var idForGet = (long)eventEntities.Count() - 1;
        var entity = eventEntities.First(e => e.Id == idForGet);

        var dto = await _eventService.GetByIdAsync(idForGet);

        AssertEntityAndDtoMatch(entity, dto);

        _mockDbContext.Reset();
    }

    private void AssertEntitiesAndDtosCollectionMatch(IEnumerable<EventEntity> entities, IEnumerable<EventOutputDto> dtos)
    {
        Assert.NotNull(dtos);

        var isMatched = dtos.Count() == entities.Count() &&
            dtos.Zip(entities, (e, d) =>
            e.Id == d.Id && e.Name == d.Name && e.CofinanceRate == d.CofinanceRate &&
            e.TotalBudget == d.TotalBudget && e.LocalBudget == d.LocalBudget && e.RegionalBudget == d.RegionalBudget)
            .All(match => match);

        Assert.True(isMatched);
    }

    private void AssertEntityAndDtoMatch(EventEntity entity, EventOutputDto dto)
    {
        Assert.NotNull(dto);

        var isMatched = entity.Id == dto.Id && entity.Name == dto.Name &&
            entity.TotalBudget == dto.TotalBudget && entity.LocalBudget == dto.LocalBudget &&
            entity.RegionalBudget == dto.RegionalBudget;

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