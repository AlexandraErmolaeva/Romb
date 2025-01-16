namespace Romb.Application.Calculators;

public interface IBudgetCalculator
{
    (decimal regionalBudget, decimal localBudget) CalculateRegionalAndLocalBudgets(decimal totalBudget, decimal cofinanceRate);
}
