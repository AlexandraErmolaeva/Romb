using Microsoft.EntityFrameworkCore;
using Romb.Application.Entities;

namespace Romb.Application;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<PlannedEventEntity> PlannedEvents { get; set; }
    public virtual DbSet<ActualEventEntity> ActualEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlannedEventEntity>(entity =>
        {
            entity.ToTable("planned_events");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.TargetCode)
                  .HasColumnName("target_code");

            entity.Property(e => e.Name)
                  .HasColumnName("name");

            entity.Property(e => e.TotalBudget)
                  .HasColumnName("total_budget")
                  .HasColumnType("decimal(50,15)");

            entity.Property(e => e.PlannedCofinanceRate)
                  .HasColumnName("planned_cofinance_rate")
                  .HasColumnType("decimal(19,15)");

            entity.Property(e => e.PlannedLocalBudget)
                  .HasColumnName("planned_local_budget")
                  .HasColumnType("decimal(50,15)");

            entity.Property(e => e.PlannedRegionalBudget)
                  .HasColumnName("planned_regional_budget")
                  .HasColumnType("decimal(50,15)");

            entity.Property(e => e.IsActualCalculated)
                  .HasColumnName("is_actual_calculated");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
        );

        modelBuilder.Entity<ActualEventEntity>(entity =>
        {
            entity.ToTable("actual_events");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");
                  
            entity.Property(e => e.PlannedEventId)
                  .HasColumnName("planned_event_id");

            entity.Property(e => e.CompletedWorksBudget)
                  .HasColumnName("completed_works_budget")
                  .HasColumnType("decimal(50,15)");

            entity.Property(e => e.ActualCofinanceRate)
                  .HasColumnName("actual_cofinance_rate")
                  .HasColumnType("decimal(19,15)");

            entity.Property(e => e.ActualLocalBudget)
                  .HasColumnName("actual_local_budget")
                  .HasColumnType("decimal(50,15)");

            entity.Property(e => e.ActualRegionalBudget)
                  .HasColumnName("actual_regional_budget")
                  .HasColumnType("decimal(50,15)");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(b => b.PlannedEvent)
                  .WithOne()
                  .HasForeignKey<ActualEventEntity>(b => b.PlannedEventId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
        );
    }
}
