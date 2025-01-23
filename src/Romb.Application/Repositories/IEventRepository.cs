using Romb.Application.Dtos;
using Romb.Application.Entities;

namespace Romb.Application.Repositories
{
    public interface IEventRepository
    {
        Task AddAsync(EventEntity entity);
        Task DeleteAsync();
        Task DeleteAsync(EventEntity entity);
        Task<bool> ExistsAsync(long id);
        Task<IEnumerable<EventEntity>> GetAsync();
        Task<EventEntity> GetByIdAsync(long id);
        Task UpdateAsync(EventEntity entity);
    }
}