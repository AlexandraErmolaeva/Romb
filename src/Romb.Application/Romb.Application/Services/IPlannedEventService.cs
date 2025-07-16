using Romb.Application.Dtos;

namespace Romb.Application.Services
{
    public interface IPlannedEventService
    {
        Task<PlannedEventResponceDto> AddAsync(PlannedEventRequestDto dto, CancellationToken token = default);
        Task DeleteAsync(CancellationToken token = default);
        Task DeleteByIdAsync(long id, CancellationToken token = default);
        Task<IEnumerable<PlannedEventResponceDto>> GetAsync(CancellationToken token = default);
        Task<PlannedEventResponceDto> GetByIdAsync(long id, CancellationToken token = default);
        Task UpdateByIdAsync(long id, PlannedEventRequestDto dto, CancellationToken token = default);
    }
}