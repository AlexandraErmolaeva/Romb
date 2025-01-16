namespace Romb.Application.Dtos;

public class EventOutputDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public decimal TotalBudget { get; init; }
    public decimal CofinanceRate { get; init; }
    public decimal LocalBudget { get; init; }
    public decimal RegionalBudget { get; init; }
}
