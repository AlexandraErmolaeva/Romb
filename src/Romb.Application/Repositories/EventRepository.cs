using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<EventEntity>> GetAsync()
    {
        return await _dbContext.Events.AsNoTracking().ToListAsync();
    }

    public async Task<EventEntity> GetByIdAsync(long id)
    {
        return await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task AddAsync(EventEntity entity)
    {
        await _dbContext.Events.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(EventEntity entity)
    {
        _dbContext.Events.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(EventEntity entity)
    {
        _dbContext.Events.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Events");
    }

    // TODO: нужен ли этот метод?
    public async Task<bool> ExistsAsync(long id)
    {
        return await _dbContext.Events.AnyAsync(e => e.Id == id);
    }
}
