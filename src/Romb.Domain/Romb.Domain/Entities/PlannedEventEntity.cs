namespace Romb.Application.Entities;

public class PlannedEventEntity
{
    public long Id { get; set; }
    public string TargetCode { get; set; }
    public string Name { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal PlannedCofinanceRate { get; set; }
    public decimal PlannedLocalBudget { get; set; }
    public decimal PlannedRegionalBudget { get; set; }
    public bool IsActualCalculated { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 
}
