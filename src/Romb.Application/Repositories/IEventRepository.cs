using Romb.Application.Entities;

namespace Romb.Application.Repositories
{
    public interface IEventRepository
    {
        Task AddAsync(EventEntity entity, CancellationToken token = default);
        Task DeleteAsync(CancellationToken token = default);
        Task DeleteAsync(EventEntity entity, CancellationToken token = default);
        Task<bool> ExistsAsync(long id, CancellationToken token = default);
        Task<IEnumerable<EventEntity>> GetAsync(CancellationToken token = default);
        Task<EventEntity> GetByIdAsync(long id, CancellationToken token = default);
        Task UpdateAsync(EventEntity entity, CancellationToken token = default);
    }
}