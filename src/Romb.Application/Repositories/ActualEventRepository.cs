using Microsoft.EntityFrameworkCore;
using Romb.Application.Entities;

namespace Romb.Application.Repositories;

public class ActualEventRepository : IActualEventRepository
{
    private readonly AppDbContext _dbContext;

    public ActualEventRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ActualEventEntity>> GetAsync(CancellationToken token = default)
    {
        return await _dbContext.ActualEvents.AsNoTracking().ToListAsync(token);
    }

    public async Task<ActualEventEntity> GetByIdAsync(long id, CancellationToken token = default)
    {
        return await _dbContext.ActualEvents.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, token);
    }

    public async Task AddAsync(ActualEventEntity entity, CancellationToken token = default)
    {
        _dbContext.Attach(entity.PlannedEvent);

        await _dbContext.ActualEvents.AddAsync(entity, token);
        await _dbContext.SaveChangesAsync(token);
    }

}
