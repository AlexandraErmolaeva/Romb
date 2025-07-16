using Romb.Application.Dtos;

namespace Romb.Application.Services
{
    public interface IActualEventService
    {
        Task<ActualEventResponceDto> AddAsync(ActualEventRequestDto dto, CancellationToken token = default);
        Task<IEnumerable<ActualEventResponceDto>> GetAsync(CancellationToken token = default);
        Task<ActualEventResponceDto> GetByIdAsync(long id, CancellationToken token = default);
        Task<IEnumerable<ActualEventResponceDto>> GetByTargetCodeAsync(string targetCode, CancellationToken token = default);
        Task UpdateByTargetCodeAsync(string targetCode, CancellationToken token = default);
    }
}