namespace Romb.Application.Helpers
{
    public interface IBudgetCalculator
    {
        (decimal actualCofinanceRate, decimal actualRegionalBudget) CalculateActualCofinanceRateAndRegionalBudget(decimal totalBudget, decimal plannedCofinanceRate, decimal plannedRegionalBudget, decimal completedWorksBudget);
        decimal CalculateLocalBudget(decimal totalValue, decimal cofinanceRate);
        decimal CalculateRegionalBudget(decimal totalValue, decimal cofinanceRate);
    }
}