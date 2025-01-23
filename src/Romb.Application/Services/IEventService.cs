using Romb.Application.Dtos;

namespace Romb.Application.Services
{
    public interface IEventService
    {
        Task<EventOutputDto> AddAsync(EventInputDto dto, CancellationToken token = default);
        Task DeleteAsync(CancellationToken token = default);
        Task DeleteByIdAsync(long id, CancellationToken token = default);
        Task<IEnumerable<EventOutputDto>> GetAsync(CancellationToken token = default);
        Task<EventOutputDto> GetByIdAsync(long id, CancellationToken token = default);
        Task UpdateByIdAsync(long id, EventInputDto dto, CancellationToken token = default);
    }
}