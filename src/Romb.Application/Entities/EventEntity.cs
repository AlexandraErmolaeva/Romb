using System.ComponentModel.DataAnnotations.Schema;

namespace Romb.Application.Entities;

public class EventEntity
{
    [Column("id")]
    public long Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("total_budget")]
    public decimal TotalBudget { get; set; }
    [Column("cofinance_rate")]
    public decimal CofinanceRate { get; set; }
    [Column("local_budget")]
    public decimal LocalBudget { get; set; }
    [Column("regional_budget")]
    public decimal RegionalBudget { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
