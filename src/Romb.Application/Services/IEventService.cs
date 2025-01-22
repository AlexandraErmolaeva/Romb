using Romb.Application.Dtos;
using System.Collections;

namespace Romb.Application.Services;

public interface IEventService
{
    Task<EventOutputDto> GetByIdAsync(long id);
    Task<IEnumerable<EventOutputDto>> GetAsync();
    Task<EventOutputDto> AddAsync(EventInputDto eventDto);
    Task DeleteByIdAsync(long id);
    Task DeleteAsync();
    Task UpdateByIdAsync(long id, EventInputDto dto);
}
