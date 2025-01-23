namespace Romb.Application.Helpers;

public interface IBudgetCalculator
{
    (decimal regionalBudget, decimal localBudget) CalculateRegionalAndLocalBudgets(decimal totalBudget, decimal cofinanceRate);
}
