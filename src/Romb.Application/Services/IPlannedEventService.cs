using Romb.Application.Dtos;

namespace Romb.Application.Services
{
    public interface IPlannedEventService
    {
        Task<PlannedEventOutputDto> AddAsync(PlannedEventInputDto dto, CancellationToken token = default);
        Task DeleteAsync(CancellationToken token = default);
        Task DeleteByIdAsync(long id, CancellationToken token = default);
        Task<IEnumerable<PlannedEventOutputDto>> GetAsync(CancellationToken token = default);
        Task<PlannedEventOutputDto> GetByIdAsync(long id, CancellationToken token = default);
        Task UpdateByIdAsync(long id, PlannedEventInputDto dto, CancellationToken token = default);
    }
}