namespace Romb.Application.Dtos;

public class ActualEventResponceDto
{
    public long Id { get; init; }
    public long PlannedEventId { get; init; }
    public string TargetCode { get; init; }
    public decimal CompletedWorksBudget { get; init; }
    public decimal ActualCofinanceRate { get; init; }
    public decimal ActualLocalBudget { get; init; }
    public decimal ActualRegionalBudget { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
