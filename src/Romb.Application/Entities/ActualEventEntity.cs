namespace Romb.Application.Entities;

public class ActualEventEntity
{
    public long Id { get; set; } 
    public long PlannedEventId { get; set; } 
    public decimal CompletedWorksBudget { get; set; }
    public decimal ActualCofinanceRate { get; set; }
    public decimal ActualLocalBudget { get; set; }
    public decimal ActualRegionalBudget { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PlannedEventEntity PlannedEvent { get; set; }
}
