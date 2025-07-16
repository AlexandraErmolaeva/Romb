using Romb.Application.Entities;

namespace Romb.Application.Repositories
{
    public interface IPlannedEventRepository
    {
        Task AddAsync(PlannedEventEntity entity, CancellationToken token = default);
        Task DeleteAsync(CancellationToken token = default);
        Task DeleteAsync(PlannedEventEntity entity, CancellationToken token = default);
        Task<bool> ExistsAsync(long id, CancellationToken token = default);
        Task<IEnumerable<PlannedEventEntity>> GetAsync(CancellationToken token = default);
        Task<PlannedEventEntity> GetByIdAsync(long id, CancellationToken token = default);
        Task UpdateAsync(PlannedEventEntity entity, CancellationToken token = default);
    }
}