using Romb.Application.Exceptions;

namespace Romb.Application.Calculators;

public class BudgetCalculator : IBudgetCalculator
{
    private const decimal totalRate = 100M; // 100%.

    public (decimal regionalBudget, decimal localBudget) CalculateRegionalAndLocalBudgets(decimal totalBudget, decimal cofinanceRate)
    {
        return (CalculateRegionalBudget(totalBudget, cofinanceRate), CalculateLocalBudget(totalBudget, cofinanceRate));
    }

    private decimal CalculateRegionalBudget(decimal totalBudget, decimal cofinanceRate)
    {
        var regionalBudget = totalBudget / totalRate * cofinanceRate;

        if (regionalBudget < 0)
            throw new EventCalculatingBudgetException("Value of the regional budget is incorrect.");

        return regionalBudget;
    }

    private decimal CalculateLocalBudget(decimal totalBudget, decimal cofinanceRate)
    {
        var localBudget = totalBudget - (totalBudget / totalRate * cofinanceRate);

        if (localBudget < 0)
            throw new EventCalculatingBudgetException("Value of the local budget is incorrect.");

        return localBudget;
    }
}

