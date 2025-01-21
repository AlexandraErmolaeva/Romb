using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Romb.Application.Entities;

namespace Romb.Application;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<EventEntity> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventEntity>(entity =>
        {
            entity.ToTable("events");

            entity.Property(e => e.Id)
                  .HasColumnType("id"); 

            entity.Property(e => e.Name)
                  .HasColumnType("name");

            entity.Property(e => e.TotalBudget)
                  .HasColumnType("total_budget"); 

            entity.Property(e => e.CofinanceRate)
                  .HasColumnType("cofinance_rate");

            entity.Property(e => e.LocalBudget)
                  .HasColumnType("local_budget");

            entity.Property(e => e.RegionalBudget)
                  .HasColumnType("regional_budget");

            entity.Property(e => e.CreatedAt)
                  .HasColumnType("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnType("updated_at");
        }
        );
    }
}
