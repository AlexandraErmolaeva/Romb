using AutoMapper;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Exceptions;
using Romb.Application.Extensions;
using Romb.Application.Helpers;
using Romb.Application.Repositories;

namespace Romb.Application.Services;

public class ActualEventService : IActualEventService
{
    private readonly IPlannedEventRepository _plannedEventRepository;
    private readonly IActualEventRepository _actualEventRepository;
    private readonly IMapper _mapper;
    private readonly IBudgetCalculator _budgetCalculator;
    private readonly ILogger<ActualEventService> _logger;

    private const string ServiceName = nameof(ActualEventService);

    public ActualEventService(IPlannedEventRepository plannedEventRepository, IActualEventRepository actualEventRepository, IMapper mapper, IBudgetCalculator budgetCalculator, ILogger<ActualEventService> logger)
    {
        _plannedEventRepository = plannedEventRepository;
        _actualEventRepository = actualEventRepository;
        _budgetCalculator = budgetCalculator;
        _mapper = mapper;
        _logger = logger;
    }

    #region [Getting events]
    public async Task<IEnumerable<ActualEventResponceDto>> GetAsync(CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting event from the database...", ServiceName);

        token.ThrowIfCancellationRequested();

        var entities = await _actualEventRepository.GetAsync(token);

        var outputDtos = CreateOutputDtosCollection(entities);

        return outputDtos;
    }

    public async Task<ActualEventResponceDto> GetByIdAsync(long id, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting event from the database with ID: {Id}...", ServiceName, id);

        token.ThrowIfCancellationRequested();

        var entity = await _actualEventRepository.GetByIdAsync(id, token) ?? throw new EntityNotFoundException("Entity not found.");

        var outputDto = CreateOutputDtoFromEntity(entity);

        return outputDto;
    }

    public async Task<IEnumerable<ActualEventResponceDto>> GetByTargetCodeAsync(string targetCode, CancellationToken token = default)
    {
        if (targetCode is null || string.IsNullOrWhiteSpace(targetCode))
            throw new ArgumentException("Target code is incorrect.");

        _logger.LogInformation("[{ServiceName}]: Getting events from the database with target code: {TargetCode}...", ServiceName, targetCode);

        token.ThrowIfCancellationRequested();

        var entities = await _actualEventRepository.GetByTargetCodeAsync(targetCode, token) ?? throw new EntityNotFoundException("Entities not found.");

        var outputDto = CreateOutputDtosCollection(entities);

        return outputDto;
    }
    #endregion

    #region [Adding events]
    public async Task<ActualEventResponceDto> AddAsync(ActualEventRequestDto requestDto, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Adding event to the database...", ServiceName);

        requestDto.CheckValidity();

        token.ThrowIfCancellationRequested();

        var plannedEvent = await _plannedEventRepository.GetByIdAsync(requestDto.PlannedEventId, token) ?? throw new EntityNotFoundException("Entity not found.");

        var entity = PrepareEntity(plannedEvent, CreateEntityFromInputDto(requestDto));

        token.ThrowIfCancellationRequested();

        await _actualEventRepository.AddAsync(entity, token);

        var responceDto = CreateOutputDtoFromEntity(entity);

        return responceDto;
    }
    #endregion

    #region [Updating event]
    public async Task UpdateByTargetCodeAsync(string targetCode, CancellationToken token = default)
    {
        if (targetCode is null || string.IsNullOrWhiteSpace(targetCode))
            throw new ArgumentException("Target code is incorrect.");

        _logger.LogInformation("[{ServiceName}]: Getting events from the database with target code: {TargetCode}...", ServiceName, targetCode);

        token.ThrowIfCancellationRequested();

        var entities = await _actualEventRepository.GetByTargetCodeAsync(targetCode, token) ?? throw new EntityNotFoundException("Entities not found.");

        var firstEntity = entities.OrderBy(e => e.Id).First();

        var firstEntityActualCofinanceRate = firstEntity.ActualCofinanceRate;

        var entitiesToUpdate = entities.OrderBy(e => e.Id).Skip(1);

        foreach (var entity in entitiesToUpdate)
            UpdateEntity(entity, firstEntityActualCofinanceRate);

        token.ThrowIfCancellationRequested();

        await _actualEventRepository.UpdateEntitiesAsync(entities, token);
    }
    #endregion

    private ActualEventEntity PrepareEntity(PlannedEventEntity plannedEvent, ActualEventEntity entity)
    {
        entity.PlannedEvent = plannedEvent;

        var totalBudget = entity.PlannedEvent.TotalBudget;
        var plannedCofinanceRate = entity.PlannedEvent.PlannedCofinanceRate;
        var plannedRegionalBudget = entity.PlannedEvent.PlannedRegionalBudget;
        var completedWorksBudget = entity.CompletedWorksBudget;

        var (actualCofinanceRate, actualRegionalBudget) = _budgetCalculator.CalculateActualCofinanceRateAndRegionalBudget(totalBudget, plannedCofinanceRate, plannedRegionalBudget, completedWorksBudget);

        entity.ActualCofinanceRate = actualCofinanceRate;
        entity.ActualRegionalBudget = actualRegionalBudget;
        entity.ActualLocalBudget = _budgetCalculator.CalculateLocalBudget(completedWorksBudget, actualCofinanceRate);

        return entity;
    }

    private ActualEventEntity UpdateEntity(ActualEventEntity entity, decimal actualCofinanceRate)
    {
        var completedWorksBudget = entity.CompletedWorksBudget;

        entity.ActualCofinanceRate = actualCofinanceRate;
        entity.ActualLocalBudget = _budgetCalculator.CalculateLocalBudget(completedWorksBudget, actualCofinanceRate);
        entity.ActualRegionalBudget = _budgetCalculator.CalculateRegionalBudget(completedWorksBudget, actualCofinanceRate);

        return entity;
    }

    private ActualEventResponceDto CreateOutputDtoFromEntity(ActualEventEntity entity)
    {
        return _mapper.Map<ActualEventResponceDto>(entity);
    }

    private ActualEventEntity CreateEntityFromInputDto(ActualEventRequestDto dto)
    {
        return _mapper.Map<ActualEventEntity>(dto);
    }

    private IEnumerable<ActualEventResponceDto> CreateOutputDtosCollection(IEnumerable<ActualEventEntity> entities)
    {
        return _mapper.Map<IEnumerable<ActualEventResponceDto>>(entities);
    }
}
