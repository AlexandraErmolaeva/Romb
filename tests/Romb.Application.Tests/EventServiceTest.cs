using Moq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Romb.Application.Calculators;
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
        var eventEntities = CreateEvents().AsQueryable();

        _mockDbContext.Setup(db => db.Events).ReturnsDbSet(eventEntities);

        var resultEventDtos = await _eventService.GetAllEventsAsync();

        AssertEntitiesAndDtosCollectionMatch(eventEntities, resultEventDtos);

        _mockDbContext.Reset();
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnEvent()
    {
        var eventEntities = CreateEvents().AsQueryable();

        _mockDbContext.Setup(db => db.Events).ReturnsDbSet(eventEntities);

        var idForGet = (long)eventEntities.Count() - 1;
        var entity = eventEntities.First(e => e.Id == idForGet);

        var resultEventDto = await _eventService.GetEventByIdAsync(idForGet);

        AssertEntityAndDtoMatch(entity, resultEventDto);

        _mockDbContext.Reset();
    }

    private void AssertEntitiesAndDtosCollectionMatch(IEnumerable<EventEntity> entities, IEnumerable<EventOutputDto> resultDtos)
    {
        Assert.NotNull(resultDtos);

        var isMatched = resultDtos.Count() == entities.Count() &&
            resultDtos.Zip(entities, (e, d) =>
            e.Id == d.Id && e.Name == d.Name && e.CofinanceRate == d.CofinanceRate &&
            e.TotalBudget == d.TotalBudget && e.LocalBudget == d.LocalBudget && e.RegionalBudget == d.RegionalBudget)
            .All(match => match);

        Assert.True(isMatched);
    }

    private void AssertEntityAndDtoMatch(EventEntity entity, EventOutputDto resultDto)
    {
        Assert.NotNull(resultDto);

        var isMatched = entity.Id == resultDto.Id && entity.Name == resultDto.Name &&
            entity.TotalBudget == resultDto.TotalBudget && entity.LocalBudget == resultDto.LocalBudget &&
            entity.RegionalBudget == resultDto.RegionalBudget;

        Assert.True(isMatched);
    }

    private List<EventEntity> CreateEvents()
    {
        var eventEntities = new List<EventEntity>();

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

            eventEntities.Add(entity);
        }

        return eventEntities;
    }
}