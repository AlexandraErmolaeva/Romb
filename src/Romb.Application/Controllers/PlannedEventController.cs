using Microsoft.AspNetCore.Mvc;
using Romb.Application.Dtos;
using Romb.Application.Services;

namespace Romb.Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlannedEventController : ControllerBase
{
    private readonly IPlannedEventService _plannedEventService;
    private readonly ILogger<PlannedEventController> _logger;

    private readonly string _separator = new string('-', 30);

    private const string ControllerName = nameof(PlannedEventController);

    public PlannedEventController(IPlannedEventService plannedEventService, ILogger<PlannedEventController> logger)
    {
        _plannedEventService = plannedEventService;
        _logger = logger;
    }

    #region [GET]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlannedEventOutputDto>>> GetAsync(CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to get all events.", ControllerName);

        var dtos = await _plannedEventService.GetAsync(token);

        _logger.LogInformation("[{NameOfController}]: Receipt request was successfully completed for all events.", ControllerName);

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<PlannedEventOutputDto>> GetByIdAsync(long id, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to get event with ID: {Id}.", ControllerName, id);

        var dto = await _plannedEventService.GetByIdAsync(id, token);

        _logger.LogInformation("[{NameOfController}]: Receipt request was successfully completed for event with ID: {Id}.", ControllerName, id);

        return Ok(dto);
    }
    #endregion

    #region [POST]
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] PlannedEventInputDto dto, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to add event.", ControllerName);

        if (!ModelState.IsValid)
        {
            var validationErrors = ModelState
                .Where(x => x.Value?.Errors != null && x.Value.Errors.Any())
                .ToDictionary(k => k.Value, v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            _logger.LogWarning("[{NameOfController}]: Validation failed for event.", ControllerName);

            return BadRequest(new { message = "Validation failed.", errors = validationErrors });
        }

        var outputDto = await _plannedEventService.AddAsync(dto, token);

        _logger.LogInformation("[{NameOfController}]: Event has been successfuly added with ID: {Id}.", ControllerName, outputDto.Id);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = outputDto.Id }, outputDto);
    }
    #endregion

    #region [PUT]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateByIdAsync(long id, [FromBody] PlannedEventInputDto dto, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to update event with ID: {Id}.", ControllerName, id);

        await _plannedEventService.UpdateByIdAsync(id, dto, token);

        _logger.LogInformation("[{NameOfController}]: Update request has been successfuly completed for event with ID: {Id}.", ControllerName, id);

        return NoContent();
    }
    #endregion

    #region [DELETE]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to delete all events.", ControllerName);

        await _plannedEventService.DeleteAsync(token);

        _logger.LogInformation("[{NameOfController}]: Deletion request has been successfully completed for all events.", ControllerName);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(long id, CancellationToken token)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to delete event with ID: {Id}.", ControllerName, id);

        await _plannedEventService.DeleteByIdAsync(id, token);

        _logger.LogInformation("[{NameOfController}]: Deletion request has been successfully completed for event with ID: {Id}.", ControllerName, id);

        return NoContent();
    }
    #endregion
}

