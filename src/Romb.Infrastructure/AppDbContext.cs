using Microsoft.EntityFrameworkCore;
using Romb.Application.Entities;

namespace Romb.Infrastructure;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<EventEntity> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventEntity>(entity =>
        {
            entity.ToTable("events");

            entity.Property(e => e.Id)
                  .HasColumnName("id"); 

            entity.Property(e => e.Name)
                  .HasColumnName("name");

            entity.Property(e => e.TotalBudget)
                  .HasColumnName("total_budget")
                  .HasColumnType("decimal(18,25)");

            entity.Property(e => e.CofinanceRate)
                  .HasColumnName("cofinance_rate")
                  .HasColumnType("decimal(18,25)");

            entity.Property(e => e.LocalBudget)
                  .HasColumnName("local_budget")
                  .HasColumnType("decimal(18,25)");

            entity.Property(e => e.RegionalBudget)
                  .HasColumnName("regional_budget")
                  .HasColumnType("decimal(18,25)");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at");
        }
        );
    }
}
