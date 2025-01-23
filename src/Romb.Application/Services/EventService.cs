using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Romb.Application.Helpers;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Exceptions;
using System.Text;
using Romb.Application.Repositories;

namespace Romb.Application.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _dbContext;
    private readonly IEventRepository _eventrepository;
    private readonly IMapper _mapper;
    private readonly IBudgetCalculator _budgetCalculator;
    private readonly IRedisService _redisService;
    private readonly ILogger<EventService> _logger;

    private const string ServiceName = nameof(EventService);
    private const string KeyForAllEvent = "events: all";

    private readonly string _separator = new string('-', 30);

    public EventService(AppDbContext dbContext, IEventRepository eventRepository, IMapper mapper, IBudgetCalculator budgetCalculator, IRedisService redisService, ILogger<EventService> logger)
    {
        _dbContext = dbContext;
        _eventrepository = eventRepository;
        _mapper = mapper;
        _budgetCalculator = budgetCalculator;
        _redisService = redisService;
        _logger = logger;
    }

    #region [Getting events]
    public async Task<IEnumerable<EventOutputDto>> GetAsync()
    {
        _logger.LogInformation("[{ServiceName}]: Getting all events from the database...", ServiceName);

        var entities = await _dbContext.Events.AsNoTracking().ToListAsync();

        var outputDtos = CreateOutputDtosCollection(entities);

        var cacheTime = TimeSpan.FromMinutes(10);

        await _redisService.SetAsync(KeyForAllEvent, outputDtos, cacheTime);

        return outputDtos;
    }

    public async Task<EventOutputDto> GetByIdAsync(long id)
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
    public async Task<EventOutputDto> AddAsync(EventInputDto dto)
    {
        _logger.LogInformation("[{ServiceName}]: Adding event to the database...", ServiceName);

        CheckValidity(dto);

        var entity = PrepareEntity(CreateEntityFromInputDto(dto), dto);

        _dbContext.Events.Add(entity);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("[{ServiceName}]: Event successfully added to the database.", ServiceName);

        var outputDto = CreateOutputDtoFromEntity(entity);
        return outputDto;
    }
    #endregion

    #region [Deleting events]
    public async Task DeleteByIdAsync(long id)
    {
        _logger.LogInformation("[{ServiceName}]: Event is being deleted from the database with ID: {Id}...", ServiceName, id);

        var entity = await _dbContext.Events.FindAsync(id);

        CheckExistence(entity);

        _dbContext.Remove(entity);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("[{ServiceName}]: Event has been deleted from the database with ID: {Id}.", ServiceName, id);
    }

    public async Task DeleteAsync()
    {
        _logger.LogInformation("[{ServiceName}]: All events is being deleted from the database...", ServiceName);

        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Events");

        _logger.LogInformation("[{ServiceName}]: All events have been deleted from the database.", ServiceName);
    }
    #endregion

    #region [Updating events]
    public async Task UpdateByIdAsync(long id, EventInputDto dto)
    {
        _logger.LogInformation("[{ServiceName}]: Updating the event in the database with ID: {Id}...", ServiceName, id);

        CheckValidity(dto);

        var entity = await _dbContext.Events.FindAsync(id);

        CheckExistence(entity);

        _logger.LogInformation("[{ServiceName}]: Trying to update event with ID: {Id}...", ServiceName, id);

        var sb = new StringBuilder();

        // TODO: ЗАменить на логгер.
        sb.AppendLine($"\n{_separator}")
            .AppendLine($"- Current name                      : {entity.Name}")
            .AppendLine($"- Name to update                    : {dto.Name}")
            .AppendLine($"{_separator}")
            .AppendLine($"- Current total budget              : {entity.TotalBudget}")
            .AppendLine($"- Total budget to update            : {dto.TotalBudget}")
            .AppendLine($"{_separator}")
            .AppendLine($"- Current cofinance rate            : {entity.CofinanceRate}")
            .AppendLine($"- Cofinance rate to update          : {dto.CofinanceRate}")
            .AppendLine($"{_separator}");

        var previousValueOfRegionalBudget = entity.RegionalBudget;
        var previousValueOfLocalBudget = entity.LocalBudget;

        var isNeedToUpdate = true;

        PrepareEntity(entity, dto, isNeedToUpdate);

        await _dbContext.SaveChangesAsync();

        sb.AppendLine($"- Previous value of regional budget : {previousValueOfRegionalBudget}")
            .AppendLine($"- Current value of regional budget  : {entity.RegionalBudget}")
            .AppendLine($"{_separator}")
            .AppendLine($"- Previous value of local budget    : {previousValueOfLocalBudget}")
            .AppendLine($"- Current value of local budget     : {entity.LocalBudget}")
            .Append($"{_separator}");

        _logger.LogInformation(sb.ToString());

        _logger.LogInformation("[{ServiceName}]: Event has been successfully updated with ID: {Id}", ServiceName, id);
    }
    #endregion

    private void CheckValidity(EventInputDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name cannot be empty.");

        if (dto.CofinanceRate < 0 || dto.CofinanceRate > 100)
            throw new EventCofinanceRateIncorrectValueException("Incorrect cofinance rate.");

        if (dto.TotalBudget <= 0 || dto.TotalBudget > decimal.MaxValue)
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

    private EventEntity PrepareEntity(EventEntity entity, EventInputDto dto, bool isNeedToUpdate = false)
    {
        if (isNeedToUpdate)
            MapInputDtoToEntity(dto, entity);

        (entity.RegionalBudget, entity.LocalBudget) = _budgetCalculator
            .CalculateRegionalAndLocalBudgets(entity.TotalBudget, entity.CofinanceRate);

        return entity;
    }

    #region [Mapping]
    // Маппим сущность в дто для пользователя, создав ее.
    private EventOutputDto CreateOutputDtoFromEntity(EventEntity entity)
    {
        return _mapper.Map<EventOutputDto>(entity);
    }
    // Маппим входящее дто от пользователя в сущность, создав ее.
    private EventEntity CreateEntityFromInputDto(EventInputDto dto)
    {
        return _mapper.Map<EventEntity>(dto);
    }
    // Маппим уже существующую сущность.
    private void MapInputDtoToEntity(EventInputDto dto, EventEntity entity)
    {
        _mapper.Map(dto, entity);
    }
    // Создаем коллекцию для вывода.
    private IEnumerable<EventOutputDto> CreateOutputDtosCollection(IEnumerable<EventEntity> entity)
    {
        return _mapper.Map<IEnumerable<EventOutputDto>>(entity);
    }
    #endregion
}
