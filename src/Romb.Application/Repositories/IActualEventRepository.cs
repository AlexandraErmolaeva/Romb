using Romb.Application.Entities;

namespace Romb.Application.Repositories
{
    public interface IActualEventRepository
    {
        Task AddAsync(ActualEventEntity entity, CancellationToken token = default);
        Task<IEnumerable<ActualEventEntity>> GetAsync(CancellationToken token = default);
        Task<ActualEventEntity> GetByIdAsync(long id, CancellationToken token = default);
        Task<IEnumerable<ActualEventEntity>> GetByTargetCodeAsync(string targetCode, CancellationToken token = default);
        Task UpdateCollectionAsync(IEnumerable<ActualEventEntity> entities, CancellationToken token = default);
    }
}