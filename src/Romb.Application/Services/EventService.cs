using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Romb.Application.Calculators;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Exceptions;

namespace Romb.Application.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IBudgetCalculator _budgetCalculator;
    private readonly ILogger<EventService> _logger;

    private const string ServiceName = nameof(EventService);

    public EventService(AppDbContext dbContext, IMapper mapper, IBudgetCalculator budgetCalculator, ILogger<EventService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _budgetCalculator = budgetCalculator;
        _logger = logger;
    }

    #region [Mapping]
    // Маппим сущность в дто для пользователя, создав ее.
    public EventOutputDto CreateOutputDtoFromEntity(EventEntity entity)
    {
        return _mapper.Map<EventOutputDto>(entity);
    }
    // Маппим входящее дто от пользователя в сущность, создав ее.
    public EventEntity CreateEntityFromInputDto(EventInputDto dto)
    {
        return _mapper.Map<EventEntity>(dto);
    }

    public void MapInputDtoToEntity(EventInputDto dto, EventEntity entity)
    {
        _mapper.Map(dto, entity);
    }

    public IEnumerable<EventOutputDto> CreateOutputDtosCollection(IEnumerable<EventEntity> entity)
    {
        return _mapper.Map<IEnumerable<EventOutputDto>>(entity);
    }
    #endregion

    #region [Getting events]
    public async Task<IEnumerable<EventOutputDto>> GetAllEventsAsync()
    {
        _logger.LogInformation("[{ServiceName}]: Getting all events from the database...", ServiceName);

        var entities = await _dbContext.Events.AsNoTracking().ToListAsync();

        return CreateOutputDtosCollection(entities);
    }

    public async Task<EventOutputDto> GetEventByIdAsync(long id)
    {
        _logger.LogInformation("[{ServiceName}]: Getting event from the database with ID: {Id}...", ServiceName, id);

        var entity = await _dbContext.Events
            .AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);

        CheckExistence(entity);

        var outputDto = CreateOutputDtoFromEntity(entity);
        return outputDto;
    }
    #endregion

    #region [Adding events]
    public async Task<EventOutputDto> AddEventAsync(EventInputDto dto)
    {
        _logger.LogInformation("[{ServiceName}]: Adding event to the database...", ServiceName);

        CheckValidityOfEvent(dto);

        var entity = CreateEntityFromInputDto(dto);

        _dbContext.Events.Add(entity);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("[{ServiceName}]: Event successfully added to the database.", ServiceName);

        var outputDto = CreateOutputDtoFromEntity(entity);
        return outputDto;
    }
    #endregion

    #region [Deleting events]
    public async Task DeleteEventByIdAsync(long id)
    {
        _logger.LogInformation("[{ServiceName}]: Event is being deleted from the database with ID: {Id}...", ServiceName, id);

        var entity = await _dbContext.Events.FindAsync(id);

        CheckExistence(entity);

        _dbContext.Remove(entity);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("[{ServiceName}]: Event has been deleted from the database with ID: {Id}.", ServiceName, id);
    }

    public async Task DeleteAllEventAsync()
    {
        _logger.LogInformation("[{ServiceName}]: All events is being deleted from the database...", ServiceName);

        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Events");

        // TODO: нужна ли здесь проверка, что вся таблица действительно пуста?
        _logger.LogInformation("[{ServiceName}]: All events have been deleted from the database.", ServiceName);
    }
    #endregion

    #region [Updating events]
    public async Task UpdateEventByIdAsync(long id, EventInputDto dto)
    {
        _logger.LogInformation("[{ServiceName}]: Updating the event in the database with ID: {Id}...", ServiceName, id);

        CheckValidityOfEvent(dto); // TODO: Может быть добавить экстеншены, чтоб было типо dto.Check..();

        var entity = await _dbContext.Events.FindAsync(id);

        CheckExistence(entity);

        // TODO: Добавить разделитель и вынести логику.
        _logger.LogInformation(
            "[{ServiceName}]: Trying to update event with ID: {Id}...\n" +
            "----------------------------\n" +
            "- Current name                      : {Entity.Name}\n" +
            "- Name to update                    : {Dto.Name}\n" +
            "----------------------------\n" +
            "- Current total budget              : {Entity.TotalBudget}\n" +
            "- Total budget to update            : {Dto.TotalBudget}\n" +
            "----------------------------\n" +
            "- Current cofinance rate            : {Entity.CofinanceRate}\n" +
            "- Cofinance rate to update          : {Dto.CofinanceRate}" +
            "----------------------------\n",
            ServiceName, entity.Id, entity.Name, dto.Name, entity.TotalBudget, dto.TotalBudget, entity.CofinanceRate, dto.CofinanceRate);

        var previousValueOfRegionalBudget = entity.RegionalBudget;
        var previousValueOfLocalBudget = entity.LocalBudget;

        PrepareEventEntity(entity, dto);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "- Previous value of regional budget : {PreviousValueOfRegionalBudget}\n" +
            "- Current value of regional budget  : {Entity.RegionalBudget}\n" +
            "----------------------------\n" +
            "- Previous value of local budget    : {PreviousValueOfLocalBudget}\n" +
            "- Current value of local budget     : {Entity.LocalBudget}" +
            "----------------------------\n", 
            previousValueOfRegionalBudget, entity.RegionalBudget, previousValueOfLocalBudget, entity.LocalBudget);

        _logger.LogInformation("[{ServiceName}]: Event has been successfully updated with ID: {Id}", ServiceName, id);
    }
    #endregion

    private void CheckValidityOfEvent(EventInputDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name cannot be empty.");

        if (dto.CofinanceRate < 0 || dto.CofinanceRate > 100)
            throw new EventCofinanceRateIncorrectValueException("Incorrect cofinance rate.");

        if (dto.TotalBudget < 0 || dto.TotalBudget > decimal.MaxValue)
            throw new EventTotalBudgetIncorrectValueException("Incorrect total budget value.");
    }

    private void CheckExistence<T>(T entity)
    {
        var entityTypeName = typeof(T).Name;

        _logger.LogInformation("[{ServiceName}]: Checking if the entity exists...", ServiceName);

        if (entity is null)
        {
            _logger.LogWarning("[{ServiceName}]: Entity not found.", ServiceName);

            throw new KeyNotFoundException($"Entity not found.");
        }

        _logger.LogInformation("[{ServiceName}]: {ItemType} was successfully recieved from database.", ServiceName, entityTypeName);
    }

    private void CheckCollectionExistance<T>(IEnumerable<T> collection)
    {
        var collectionTypeName = typeof(T).Name;

        _logger.LogInformation("[{ServiceName}]: Checking if the entities collection exists...", ServiceName);

        if (!collection.Any())
        {
            _logger.LogWarning("[{ServiceName}]: Collection of {CollectionType} is empty.", ServiceName, collectionTypeName);

            throw new KeyNotFoundException($"Collection of {collectionTypeName} is empty.");
        }

        _logger.LogInformation("[{ServiceName}]: Collection was successfully recieved from database.", ServiceName);
    }

    private void PrepareEventEntity(EventEntity entity, EventInputDto dto)
    {
        MapInputDtoToEntity(dto, entity);

        (entity.RegionalBudget, entity.LocalBudget) = _budgetCalculator
            .CalculateRegionalAndLocalBudgets(entity.TotalBudget, entity.CofinanceRate);
    }
}
