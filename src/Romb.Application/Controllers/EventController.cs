using Microsoft.AspNetCore.Mvc;
using Romb.Application.Dtos;
using Romb.Application.Exceptions;
using Romb.Application.Services;

namespace Romb.Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IRedisService _redisService;
    private readonly ILogger<EventController> _logger;

    private readonly string _separator = new string('-', 30);

    private const string ControllerName = nameof(EventController);

    public EventController(IEventService eventService, ILogger<EventController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    #region [GET]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventOutputDto>>> GetAsync()
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to get all events.", ControllerName);

        var dtos = await _eventService.GetAsync();

        _logger.LogInformation("[{NameOfController}]: Receipt request was successfully completed for all events.", ControllerName);
        LoggingRequestCompletion();

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<EventOutputDto>> GetByIdAsync(long id)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to get event with ID: {Id}.", ControllerName, id);

        var dto = await _eventService.GetByIdAsync(id);

        if (dto is null)
        {
            _logger.LogError("[{NameOfController}]: Event not found with ID: {Id}.", ControllerName, id);
            LoggingRequestCompletion();

            return NotFound();
        }

        _logger.LogInformation("[{NameOfController}]: Receipt request was successfully completed for event with ID: {Id}.", ControllerName, id);
        LoggingRequestCompletion();

        return Ok(dto);
    }
    #endregion

    #region [POST]
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] EventInputDto dto)
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to add event.", ControllerName);

        if (!ModelState.IsValid)
        {
            var validationErrors = ModelState
                .Where(x => x.Value?.Errors != null && x.Value.Errors.Any())
                .ToDictionary(k => k.Value, v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            _logger.LogWarning("[{NameOfController}]: Validation failed for event.", ControllerName);
            LoggingRequestCompletion();

            return BadRequest(new { message = "Validation failed.", errors = validationErrors });
        }

        var outputDto = await _eventService.AddAsync(dto);

        _logger.LogInformation("[{NameOfController}]: Event has been successfuly added with ID: {Id}.", ControllerName, outputDto.Id);
        LoggingRequestCompletion();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = outputDto.Id }, outputDto);
    }
    #endregion

    #region [PUT]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateByIdAsync(long id, [FromBody] EventInputDto dto)
    {
        try
        {
            _logger.LogInformation("[{NameOfController}]: Recieved a request to update event with ID: {Id}.", ControllerName, id);

            await _eventService.UpdateByIdAsync(id, dto);

            _logger.LogInformation("[{NameOfController}]: Update request has been successfuly completed for event with ID: {Id}.", ControllerName, id);
            LoggingRequestCompletion();

            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(ex, "[{NameOfController}]: Event not found with ID: {Id}.", ControllerName, id);
            LoggingRequestCompletion();

            return NotFound();
        }
    }
    #endregion

    #region [DELETE]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync()
    {
        _logger.LogInformation("[{NameOfController}]: Recieved a request to delete all events.", ControllerName);

        await _eventService.DeleteAsync();

        _logger.LogInformation("[{NameOfController}]: Deletion request has been successfully completed for all events.", ControllerName);
        LoggingRequestCompletion();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync(long id)
    {
        try
        {
            _logger.LogInformation("[{NameOfController}]: Recieved a request to delete event with ID: {Id}.", ControllerName, id);

            await _eventService.DeleteByIdAsync(id);

            _logger.LogInformation("[{NameOfController}]: Deletion request has been successfully completed for event with ID: {Id}.", ControllerName, id);
            LoggingRequestCompletion();

            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(ex, "[{NameOfController}]: Event not found with ID: {Id}.", ControllerName, id);
            LoggingRequestCompletion();

            return NotFound();
        }
    }
    #endregion

    private void LoggingRequestCompletion()
    {
        _logger.LogInformation("[Request is completion]");
        _logger.LogInformation(_separator);
    }
}

