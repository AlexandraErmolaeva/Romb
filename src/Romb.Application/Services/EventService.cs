using AutoMapper;
using Romb.Application.Helpers;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Exceptions;
using Romb.Application.Repositories;
using StackExchange.Redis;
using Romb.Application.Extensions;

namespace Romb.Application.Services;

// TODO: Разобраться с токенами отмены.
// TODO: транзакции.

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly IBudgetCalculator _budgetCalculator;
    private readonly IRedisService _redisService;
    private readonly ILogger<EventService> _logger;

    private const string ServiceName = nameof(EventService);

    private readonly string _separator = new string('-', 30);

    public EventService(IEventRepository eventRepository,
                        IMapper mapper, IBudgetCalculator budgetCalculator,
                        IRedisService redisService, ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
        _budgetCalculator = budgetCalculator;
        _redisService = redisService;
        _logger = logger;
    }

    #region [Getting events]
    public async Task<IEnumerable<EventOutputDto>> GetAsync(CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting all events from the database...", ServiceName);

        try
        {
            var cachedDtos = await _redisService.GetAsync<IEnumerable<EventOutputDto>>(CacheKey.KeyForAllEvent);

            if (cachedDtos?.Any() == true)
            {
                _logger.LogInformation("[{ServiceName}]: Cache hit with key: {Key}", ServiceName, CacheKey.KeyForAllEvent);

                return cachedDtos;
            }

            var entities = await _eventRepository.GetAsync();

            var outputDtos = CreateOutputDtosCollection(entities);

            // TODO: нужно понять, как правильно кешировать, если вдруг и на этом этапе выскочила ошибка.
            var cacheTime = TimeSpan.FromMinutes(10);

            await _redisService.SetAsync(CacheKey.KeyForAllEvent, outputDtos, cacheTime);

            return outputDtos;
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "[{ServiceName}]: An error has occurred in Redis, data is being loaded from the database.", ServiceName);

            var entities = await _eventRepository.GetAsync();

            return CreateOutputDtosCollection(entities);
        }
    }

    public async Task<EventOutputDto> GetByIdAsync(long id, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting event from the database with ID: {Id}...", ServiceName, id);

        var entity = await _eventRepository.GetByIdAsync(id);

        var outputDto = entity is not null ? CreateOutputDtoFromEntity(entity) : throw new EntityNotFoundException("Entity not found.");

        return outputDto;
    }
    #endregion

    #region [Adding events]
    public async Task<EventOutputDto> AddAsync(EventInputDto dto, CancellationToken token = default)
    {
        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent);

        _logger.LogInformation("[{ServiceName}]: Adding event to the database...", ServiceName);

        dto.CheckValidity();

        var entity = PrepareEntity(CreateEntityFromInputDto(dto), dto);

        await _eventRepository.AddAsync(entity);

        _logger.LogInformation("[{ServiceName}]: Event successfully added to the database.", ServiceName);

        var outputDto = CreateOutputDtoFromEntity(entity);

        return outputDto;
    }
    #endregion

    #region [Deleting events]
    public async Task DeleteByIdAsync(long id, CancellationToken token = default)
    {
        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent);

        _logger.LogInformation("[{ServiceName}]: Event is being deleted from the database with ID: {Id}...", ServiceName, id);

        var entity = await _eventRepository.GetByIdAsync(id) ?? throw new EntityNotFoundException("Entity not found.");

        await _eventRepository.DeleteAsync(entity);

        _logger.LogInformation("[{ServiceName}]: Event has been deleted from the database with ID: {Id}.", ServiceName, id);
    }

    public async Task DeleteAsync(CancellationToken token = default)
    {
        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent);

        _logger.LogInformation("[{ServiceName}]: All events is being deleted from the database...", ServiceName);

        await _eventRepository.DeleteAsync();

        _logger.LogInformation("[{ServiceName}]: All events have been deleted from the database.", ServiceName);
    }
    #endregion

    #region [Updating events]
    public async Task UpdateByIdAsync(long id, EventInputDto dto, CancellationToken token = default)
    {
        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent);

        _logger.LogInformation("[{ServiceName}]: Updating the event in the database with ID: {Id}...", ServiceName, id);

        var entity = await _eventRepository.GetByIdAsync(id) ?? throw new EntityNotFoundException("Entity not found.");

        dto.CheckValidity();

        var previousValueOfRegionalBudget = entity.RegionalBudget;
        var previousValueOfLocalBudget = entity.LocalBudget;

        _ = PrepareEntity(entity, dto, isNeedToUpdate : true);

        await _eventRepository.UpdateAsync(entity);

        PrintUpdatingEntity(entity, dto, previousValueOfRegionalBudget, previousValueOfLocalBudget);

        _logger.LogInformation("[{ServiceName}]: Event has been successfully updated with ID: {Id}", ServiceName, id);
    }
    #endregion

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

    private void PrintUpdatingEntity(EventEntity entity, EventInputDto dto, decimal previousValueOfRegionalBudget, decimal previousValueOfLocalBudget)
    {
        _logger.LogInformation("\n{_separator}\n", _separator);
        _logger.LogInformation("- Current name                      : {entity.Name}\n", entity.Name);
        _logger.LogInformation("- Name to update                    : {dto.Name}\n", dto.Name);
        _logger.LogInformation("{_separator}\n", _separator);
        _logger.LogInformation("- Current total budget              : {entity.TotalBudget}\n", entity.TotalBudget);
        _logger.LogInformation("- Total budget to update            : {dto.TotalBudget}\n", dto.TotalBudget);
        _logger.LogInformation("{_separator}\n", _separator);
        _logger.LogInformation("- Current cofinance rate            : {entity.CofinanceRate}\n", entity.CofinanceRate);
        _logger.LogInformation("- Cofinance rate to update          : {dto.CofinanceRate}\n", dto.CofinanceRate);
        _logger.LogInformation("{_separator}\n", _separator);
        _logger.LogInformation("- Previous value of regional budget : {previousValueOfRegionalBudget}\n", previousValueOfRegionalBudget);
        _logger.LogInformation("- Current value of regional budget  : {entity.RegionalBudget}\n", entity.RegionalBudget);
        _logger.LogInformation("{_separator}\n", _separator);
        _logger.LogInformation("- Previous value of local budget    : {previousValueOfLocalBudget}\n", previousValueOfLocalBudget);
        _logger.LogInformation("- Current value of local budget     : {entity.LocalBudget}\n", entity.LocalBudget);
        _logger.LogInformation("{_separator}\n", _separator);
    }
}
