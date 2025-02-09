using AutoMapper;
using Romb.Application.Helpers;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Exceptions;
using Romb.Application.Repositories;
using Romb.Application.Extensions;

namespace Romb.Application.Services;

public class PlannedEventService : IPlannedEventService
{
    private readonly IPlannedEventRepository _plannedEventRepository;
    private readonly IMapper _mapper;
    private readonly IBudgetCalculator _budgetCalculator;
    private readonly IRedisService _redisService;
    private readonly ILogger<PlannedEventService> _logger;

    private const string ServiceName = nameof(PlannedEventService);

    private readonly string _separator = new string('-', 30);

    public PlannedEventService(IPlannedEventRepository plannedEventRepository,
                               IMapper mapper,
                               IBudgetCalculator budgetCalculator,
                               IRedisService redisService,
                               ILogger<PlannedEventService> logger)
    {
        _plannedEventRepository = plannedEventRepository;
        _mapper = mapper;
        _budgetCalculator = budgetCalculator;
        _redisService = redisService;
        _logger = logger;
    }

    #region [Getting events]
    public async Task<IEnumerable<PlannedEventOutputDto>> GetAsync(CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting all events...", ServiceName);

        var cachedDtos = await _redisService.GetAsync<IEnumerable<PlannedEventOutputDto>>(CacheKey.KeyForAllEvent, token);

        if (cachedDtos?.Any() == true)
        {
            _logger.LogInformation("[{ServiceName}]: Cache hit with key: {Key}.", ServiceName, CacheKey.KeyForAllEvent);

            return cachedDtos;
        }

        token.ThrowIfCancellationRequested(); 

        var entities = await _plannedEventRepository.GetAsync(token);

        _logger.LogInformation("[{ServiceName}]: Get all events from database.", ServiceName);

        var outputDtos = CreateOutputDtosCollection(entities);

        var cacheTime = TimeSpan.FromMinutes(10);

        await _redisService.SetAsync(CacheKey.KeyForAllEvent, outputDtos, cacheTime, token);

        return outputDtos;
    }

    public async Task<PlannedEventOutputDto> GetByIdAsync(long id, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting event from the database with ID: {Id}...", ServiceName, id);

        token.ThrowIfCancellationRequested();

        var entity = await _plannedEventRepository.GetByIdAsync(id, token) ?? throw new EntityNotFoundException("Entity not found.");

        var outputDto = CreateOutputDtoFromEntity(entity);

        return outputDto;
    }
    #endregion

    #region [Adding events]
    public async Task<PlannedEventOutputDto> AddAsync(PlannedEventInputDto dto, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Adding event to the database...", ServiceName);

        dto.CheckValidity();

        var entity = PrepareEntity(CreateEntityFromInputDto(dto), dto);

        token.ThrowIfCancellationRequested();

        await _plannedEventRepository.AddAsync(entity, token);

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

        var entity = await _plannedEventRepository.GetByIdAsync(id, token) ?? throw new EntityNotFoundException("Entity not found.");

        await _plannedEventRepository.DeleteAsync(entity, token);

        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent, token);
    }

    public async Task DeleteAsync(CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: All events is being deleted from the database...", ServiceName);

        token.ThrowIfCancellationRequested();

        await _plannedEventRepository.DeleteAsync(token);

        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent, token);
    }
    #endregion

    #region [Updating events]
    public async Task UpdateByIdAsync(long id, PlannedEventInputDto dto, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Updating the event in the database with ID: {Id}...", ServiceName, id);

        dto.CheckValidity();

        token.ThrowIfCancellationRequested();

        var entity = await _plannedEventRepository.GetByIdAsync(id, token) ?? throw new EntityNotFoundException("Entity not found.");

        var previousValueOfRegionalBudget = entity.PlannedRegionalBudget;
        var previousValueOfLocalBudget = entity.PlannedLocalBudget;

        PrintUpdatingEntity(entity, dto, previousValueOfRegionalBudget, previousValueOfLocalBudget);

        _ = PrepareEntity(entity, dto, isNeedToUpdate: true);

        await _plannedEventRepository.UpdateAsync(entity, token);

        await _redisService.RemoveAsync(CacheKey.KeyForAllEvent, token);
    }
    #endregion

    private PlannedEventEntity PrepareEntity(PlannedEventEntity entity, PlannedEventInputDto dto, bool isNeedToUpdate = false)
    {
        if (isNeedToUpdate)
            MapInputDtoToEntity(dto, entity);

        var totalBudget = entity.TotalBudget;
        var plannedCofinanceRate = entity.PlannedCofinanceRate;

        entity.PlannedRegionalBudget = _budgetCalculator.CalculatePlannedRegionalBudget(totalBudget, plannedCofinanceRate);
        entity.PlannedLocalBudget = _budgetCalculator.CalculatePlannedLocalBudget(totalBudget, plannedCofinanceRate);

        return entity;
    }

    #region [Mapping]
    private PlannedEventOutputDto CreateOutputDtoFromEntity(PlannedEventEntity entity)
    {
        return _mapper.Map<PlannedEventOutputDto>(entity);
    }
    private PlannedEventEntity CreateEntityFromInputDto(PlannedEventInputDto dto)
    {
        return _mapper.Map<PlannedEventEntity>(dto);
    }
    private void MapInputDtoToEntity(PlannedEventInputDto dto, PlannedEventEntity entity)
    {
        _mapper.Map(dto, entity);
    }
    private IEnumerable<PlannedEventOutputDto> CreateOutputDtosCollection(IEnumerable<PlannedEventEntity> entity)
    {
        return _mapper.Map<IEnumerable<PlannedEventOutputDto>>(entity);
    }
    #endregion

    private void PrintUpdatingEntity(PlannedEventEntity entity, PlannedEventInputDto dto, decimal previousValueOfRegionalBudget, decimal previousValueOfLocalBudget)
    {
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Current name                      : {entity.Name}", entity.Name);
        _logger.LogInformation("- Name to update                    : {dto.Name}", dto.Name);
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Current total budget              : {entity.TotalBudget}", entity.TotalBudget);
        _logger.LogInformation("- Total budget to update            : {dto.TotalBudget}", dto.TotalBudget);
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Current cofinance rate            : {entity.CofinanceRate}", entity.PlannedCofinanceRate);
        _logger.LogInformation("- Cofinance rate to update          : {dto.CofinanceRate}", dto.PlannedCofinanceRate);
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Previous value of regional budget : {previousValueOfRegionalBudget}", previousValueOfRegionalBudget);
        _logger.LogInformation("- Current value of regional budget  : {entity.RegionalBudget}", entity.PlannedRegionalBudget);
        _logger.LogInformation("{_separator}", _separator);
        _logger.LogInformation("- Previous value of local budget    : {previousValueOfLocalBudget}", previousValueOfLocalBudget);
        _logger.LogInformation("- Current value of local budget     : {entity.LocalBudget}", entity.PlannedLocalBudget);
        _logger.LogInformation("{_separator}", _separator);
    }
}
