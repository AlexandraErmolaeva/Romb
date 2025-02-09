namespace Romb.Application.Dtos;

public class PlannedEventOutputDto
{
    public long Id { get; init; }
    public string TargetCode { get; init; }
    public string Name { get; init; }
    public decimal TotalBudget { get; init; }
    public decimal PlannedCofinanceRate { get; init; }
    public decimal PlannedLocalBudget { get; init; }
    public decimal PlannedRegionalBudget { get; init; }
}
