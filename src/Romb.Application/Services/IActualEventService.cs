using Romb.Application.Dtos;

namespace Romb.Application.Services
{
    public interface IActualEventService
    {
        Task<ActualEventOutputDto> AddAsync(ActualEventInputDto dto, CancellationToken token = default);
        Task<ActualEventOutputDto> GetByIdAsync(long id, CancellationToken token = default);
    }
}