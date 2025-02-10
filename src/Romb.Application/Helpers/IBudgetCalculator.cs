namespace Romb.Application.Helpers
{
    public interface IBudgetCalculator
    {
        (decimal actualCofinanceRate, decimal actualRegionalBudget) CalculateActualCofinanceRateAndRegionalBudget(decimal totalBudget, decimal plannedCofinanceRate, decimal plannedRegionalBudget, decimal completedWorksBudget);
        decimal CalculateActualLocalBudget(decimal completedWorksBudget, decimal actualRegionalBudget);
        decimal CalculatePlannedLocalBudget(decimal totalBudget, decimal cofinanceRate);
        decimal CalculatePlannedRegionalBudget(decimal totalBudget, decimal cofinanceRate);
        decimal ForciblyCalculateActualLocalBudgetBudgetWithCofinanceRate(decimal completedWorksBudget, decimal cofinanceRate);
        decimal ForciblyCalculateActualRegionalBudgetWithCofinanceRate(decimal completedWorksBudget, decimal cofinanceRate);
    }
}