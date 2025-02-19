using Microsoft.AspNetCore.Mvc;
using Romb.Application.Dtos;
using Romb.Application.Services;

namespace Romb.Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActualEventController : ControllerBase
{
    private readonly IActualEventService _actualEventService;
    private readonly ILogger<ActualEventController> _logger;

    private readonly string _separator = new string('-', 30);

    private const string ControllerName = nameof(ActualEventController);

    public ActualEventController(IActualEventService actualEventService, ILogger<ActualEventController> logger)
    {
        _actualEventService = actualEventService;
        _logger = logger;
    }

    #region [GET]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActualEventResponceDto>>> GetAsync(CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to get all events.", ControllerName);

        var dtos = await _actualEventService.GetAsync(token);

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<ActualEventResponceDto>> GetByIdAsync(long id, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to get event with ID: {Id}.", ControllerName, id);

        var dto = await _actualEventService.GetByIdAsync(id, token);

        _logger.LogInformation("[{NameOfController}]: Receipt request was successfully completed for event with ID: {Id}.", ControllerName, id);

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ActualEventResponceDto>>> GetByTargetCode([FromQuery] string targetCode, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to get events with target code: {TargetCode}.", ControllerName, targetCode);

        var dto = await _actualEventService.GetByTargetCodeAsync(targetCode, token);

        _logger.LogInformation("[{NameOfController}]: Receipt request was successfully completed for events with target code: {Id}.", ControllerName, targetCode);

        return Ok(dto);
    }
    #endregion

    #region [POST]
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] ActualEventRequestDto dto, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to add event.", ControllerName);

        var outputDto = await _actualEventService.AddAsync(dto, token);

        _logger.LogInformation("[{NameOfController}]: Event has been successfuly added with ID: {Id}.", ControllerName, outputDto.Id);

        return CreatedAtAction(nameof(GetByIdAsync), new {id = outputDto.Id}, outputDto);
    }
    #endregion

    [HttpPut]
    public async Task<IActionResult> UpdateAsyncByTargetCode([FromQuery] string targetCode, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to update events with target code: {TargetCode}.", ControllerName, targetCode);

        await _actualEventService.UpdateByTargetCodeAsync(targetCode, token);

        _logger.LogInformation("[{NameOfController}]: Receipt request was successfully completed for events with target code: {Id}.", ControllerName, targetCode);

        return NoContent();
    }
}
