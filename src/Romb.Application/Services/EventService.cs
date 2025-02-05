using AutoMapper;
using Romb.Application.Helpers;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Exceptions;
using Romb.Application.Repositories;
using StackExchange.Redis;
using Romb.Application.Extensions;

namespace Romb.Application.Services;

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
        _logger.LogInformation("[{ServiceName}]: Getting all events...", ServiceName);

        var cachedDtos = await _redisService.GetAsync<IEnumerable<EventOutputDto>>(CacheKey.KeyForAllEvent, token);

        if (cachedDtos?.Any() == true)
        {
            _logger.LogInformation("[{ServiceName}]: Cache hit with key: {Key}.", ServiceName, CacheKey.KeyForAllEvent);

            return cachedDtos;
        }

        token.ThrowIfCancellationRequested();

        var entities = await _eventRepository.GetAsync(token);

        _logger.LogInformation("[{ServiceName}]: Get all events from database.", ServiceName);

        var outputDtos = CreateOutputDtosCollection(entities);

        var cacheTime = TimeSpan.FromMinutes(10);

        await _redisService.SetAsync(CacheKey.KeyForAllEvent, outputDtos, cacheTime, token);

        return outputDtos;
    }

    public async Task<EventOutputDto> GetByIdAsync(long id, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting event from the database with ID: {Id}...", ServiceName, id);

        token.ThrowIfCancellationRequested();

        var entity = await _eventRepository.GetByIdAsync(id, token) ?? throw new EntityNotFoundException("Entity not found.");

        var outputDto = CreateOutputDtoFromEntity(entity);

        return outputDto;
    }
    #endregion

    #region [Adding events]
    public async Task<EventOutputDto> AddAsync(EventInputDto dto, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Adding event to the database...", ServiceName);

        dto.CheckValidity();

        var entity = PrepareEntity(CreateEntityFromInputDto(dto), dto);

        token.ThrowIfCancellationRequested();

        await _eventRepository.AddAsync(entity, token);

        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent, token);

        var outputDto = CreateOutputDtoFromEntity(entity);

        return outputDto;
    }
    #endregion

    #region [Deleting events]
    public async Task DeleteByIdAsync(long id, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Event is being deleted from the database with ID: {Id}...", ServiceName, id);

        token.ThrowIfCancellationRequested();

        var entity = await _eventRepository.GetByIdAsync(id, token) ?? throw new EntityNotFoundException("Entity not found.");

        await _eventRepository.DeleteAsync(entity, token);

        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent, token);
    }

    public async Task DeleteAsync(CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: All events is being deleted from the database...", ServiceName);

        token.ThrowIfCancellationRequested();

        await _eventRepository.DeleteAsync(token);

        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent, token);
    }
    #endregion

    #region [Updating events]
    public async Task UpdateByIdAsync(long id, EventInputDto dto, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Updating the event in the database with ID: {Id}...", ServiceName, id);

        token.ThrowIfCancellationRequested();

        var entity = await _eventRepository.GetByIdAsync(id, token) ?? throw new EntityNotFoundException("Entity not found.");

        dto.CheckValidity();

        var previousValueOfRegionalBudget = entity.RegionalBudget;
        var previousValueOfLocalBudget = entity.LocalBudget;

        _ = PrepareEntity(entity, dto, isNeedToUpdate: true);

        await _eventRepository.UpdateAsync(entity, token);

        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent, token);

        PrintUpdatingEntity(entity, dto, previousValueOfRegionalBudget, previousValueOfLocalBudget);
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
    private EventOutputDto CreateOutputDtoFromEntity(EventEntity entity)
    {
        return _mapper.Map<EventOutputDto>(entity);
    }
    private EventEntity CreateEntityFromInputDto(EventInputDto dto)
    {
        return _mapper.Map<EventEntity>(dto);
    }
    private void MapInputDtoToEntity(EventInputDto dto, EventEntity entity)
    {
        _mapper.Map(dto, entity);
    }
    private IEnumerable<EventOutputDto> CreateOutputDtosCollection(IEnumerable<EventEntity> entity)
    {
        return _mapper.Map<IEnumerable<EventOutputDto>>(entity);
    }
    #endregion

    private void PrintUpdatingEntity(EventEntity entity, EventInputDto dto, decimal previousValueOfRegionalBudget, decimal previousValueOfLocalBudget)
    {
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Current name                      : {entity.Name}", entity.Name);
        _logger.LogInformation("- Name to update                    : {dto.Name}", dto.Name);
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Current total budget              : {entity.TotalBudget}", entity.TotalBudget);
        _logger.LogInformation("- Total budget to update            : {dto.TotalBudget}", dto.TotalBudget);
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Current cofinance rate            : {entity.CofinanceRate}", entity.CofinanceRate);
        _logger.LogInformation("- Cofinance rate to update          : {dto.CofinanceRate}", dto.CofinanceRate);
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Previous value of regional budget : {previousValueOfRegionalBudget}", previousValueOfRegionalBudget);
        _logger.LogInformation("- Current value of regional budget  : {entity.RegionalBudget}", entity.RegionalBudget);
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Previous value of local budget    : {previousValueOfLocalBudget}", previousValueOfLocalBudget);
        _logger.LogInformation("- Current value of local budget     : {entity.LocalBudget}", entity.LocalBudget);
        _logger.LogInformation("{_separator}", _separator);
    }
}
