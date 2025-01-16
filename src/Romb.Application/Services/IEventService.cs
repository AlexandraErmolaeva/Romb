using Romb.Application.Dtos;
using System.Collections;

namespace Romb.Application.Services;

public interface IEventService
{
    Task<EventOutputDto> GetEventByIdAsync(long id);
    Task<IEnumerable<EventOutputDto>> GetAllEventsAsync();
    Task<EventOutputDto> AddEventAsync(EventInputDto eventDto);
    Task DeleteEventByIdAsync(long id);
    Task DeleteAllEventAsync();
    Task UpdateEventByIdAsync(long id, EventInputDto dto);
}
