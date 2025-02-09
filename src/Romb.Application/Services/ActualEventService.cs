using AutoMapper;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Exceptions;
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
    public async Task<IEnumerable<ActualEventOutputDto>> GetAsync(CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting event from the database...", ServiceName);

        token.ThrowIfCancellationRequested();

        var entities = await _actualEventRepository.GetAsync(token);

        var outputDtos = CreateOutputDtosCollection(entities);

        return outputDtos;
    }

    public async Task<ActualEventOutputDto> GetByIdAsync(long id, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Getting event from the database with ID: {Id}...", ServiceName, id);

        token.ThrowIfCancellationRequested();

        var entity = await _actualEventRepository.GetByIdAsync(id, token) ?? throw new EntityNotFoundException("Entity not found.");

        var outputDto = CreateOutputDtoFromEntity(entity);

        return outputDto;
    }
    #endregion

    #region [Adding events]
    public async Task<ActualEventOutputDto> AddAsync(ActualEventInputDto dto, CancellationToken token = default)
    {
        _logger.LogInformation("[{ServiceName}]: Adding event to the database...", ServiceName);

        token.ThrowIfCancellationRequested();

        var plannedEvent = await _plannedEventRepository.GetByIdAsync(dto.PlannedEventId, token) ?? throw new EntityNotFoundException("Entity not found.");

        var entity = PrepareEntity(plannedEvent, CreateEntityFromInputDto(dto));

        await _actualEventRepository.AddAsync(entity, token);

        var outputDto = CreateOutputDtoFromEntity(entity);

        return outputDto;
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
        entity.ActualLocalBudget = _budgetCalculator.CalculateActualLocalBudget(completedWorksBudget, actualRegionalBudget);

        return entity;
    }

    private ActualEventOutputDto CreateOutputDtoFromEntity(ActualEventEntity entity)
    {
        return _mapper.Map<ActualEventOutputDto>(entity);
    }

    private ActualEventEntity CreateEntityFromInputDto(ActualEventInputDto dto)
    {
        return _mapper.Map<ActualEventEntity>(dto);
    }

    private IEnumerable<ActualEventOutputDto> CreateOutputDtosCollection(IEnumerable<ActualEventEntity> entities)
    {
        return _mapper.Map<IEnumerable<ActualEventOutputDto>>(entities);
    }
}
