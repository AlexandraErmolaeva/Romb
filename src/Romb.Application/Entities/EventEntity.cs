using System.ComponentModel.DataAnnotations.Schema;

namespace Romb.Application.Entities;

public class EventEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal CofinanceRate { get; set; }
    public decimal LocalBudget { get; set; }
    public decimal RegionalBudget { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
