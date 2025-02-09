using Microsoft.EntityFrameworkCore;
using Romb.Application.Dtos;
using Romb.Application.Entities;
using Romb.Application.Services;

namespace Romb.Application.Repositories;

public class PlannedEventRepository : IPlannedEventRepository
{
    private readonly AppDbContext _dbContext;

    public PlannedEventRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<PlannedEventEntity>> GetAsync(CancellationToken token = default)
    {
        return await _dbContext.PlannedEvents.AsNoTracking().ToListAsync(token);
    }

    public async Task<PlannedEventEntity> GetByIdAsync(long id, CancellationToken token = default)
    {
        return await _dbContext.PlannedEvents.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, token);
    }

    public async Task AddAsync(PlannedEventEntity entity, CancellationToken token = default)
    {
        await _dbContext.PlannedEvents.AddAsync(entity, token);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task UpdateAsync(PlannedEventEntity entity, CancellationToken token = default)
    {
        _dbContext.PlannedEvents.Update(entity);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(PlannedEventEntity entity, CancellationToken token = default)
    {
        _dbContext.PlannedEvents.Remove(entity);
        await _dbContext.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(CancellationToken token = default)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Events", token);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken token = default)
    {
        return await _dbContext.PlannedEvents.AnyAsync(e => e.Id == id, token);
    }
}
