using Microsoft.EntityFrameworkCore;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Services;

namespace Romb.Application.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _dbContext;

    private const string ServiceName = nameof(EventRepository);

    public EventRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<EventEntity>> GetAsync(CancellationToken token = default)
    {
        return await _dbContext.Events.AsNoTracking().ToListAsync(token);
    }

    public async Task<EventEntity> GetByIdAsync(long id, CancellationToken token = default)
    {
        return await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, token);
    }

    public async Task AddAsync(EventEntity entity, CancellationToken token = default)
    {
        await _dbContext.Events.AddAsync(entity, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task UpdateAsync(EventEntity entity, CancellationToken token = default)
    {
        _dbContext.Events.Update(entity);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(EventEntity entity, CancellationToken token = default)
    {
        _dbContext.Events.Remove(entity);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(CancellationToken token = default)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Events", token);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken token = default)
    {
        return await _dbContext.Events.AnyAsync(e => e.Id == id, token);
    }
}
