using Microsoft.EntityFrameworkCore;
using Romb.Application.Entities;

namespace Romb.Application;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<EventEntity> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventEntity>().ToTable("events");
    }
}
