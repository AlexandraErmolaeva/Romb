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

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<ActualEventOutputDto>> GetByIdAsync(long id, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to get event with ID: {Id}.", ControllerName, id);

        var dto = await _actualEventService.GetByIdAsync(id, token);

        _logger.LogInformation("[{NameOfController}]: Receipt request was successfully completed for event with ID: {Id}.", ControllerName, id);

        return Ok(dto);
    }

    #region [POST]
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] ActualEventInputDto dto, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to add event.", ControllerName);

        var outputDto = await _actualEventService.AddAsync(dto, token);

        _logger.LogInformation("[{NameOfController}]: Event has been successfuly added with ID: {Id}.", ControllerName, outputDto.Id);

        return CreatedAtAction(nameof(GetByIdAsync), new {id = outputDto.Id}, outputDto);
    }
    #endregion
}
