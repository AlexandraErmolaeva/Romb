using Romb.Application.Exceptions;

namespace Romb.Application.Helpers;

public class BudgetCalculator : IBudgetCalculator
{
    private const decimal totalRate = 100M; // 100%.

    public decimal CalculateRegionalBudget(decimal totalValue, decimal cofinanceRate)
    {
        var regionalBudget = (totalValue / totalRate) * cofinanceRate;

        if (regionalBudget < 0)
            throw new CalculatingBudgetException("Value of the regional budget is incorrect.");

        return Math.Round(regionalBudget, 2, MidpointRounding.AwayFromZero);
    }

    public decimal CalculateLocalBudget(decimal totalValue, decimal cofinanceRate)
    {
        var localBudget = totalValue - (totalValue / totalRate * cofinanceRate);

        if (localBudget < 0)
            throw new CalculatingBudgetException("Value of the local budget is incorrect.");

        return Math.Round(localBudget, 2, MidpointRounding.AwayFromZero);
    }

    public (decimal actualCofinanceRate, decimal actualRegionalBudget) CalculateActualCofinanceRateAndRegionalBudget(decimal totalBudget,
                                                                                                                     decimal plannedCofinanceRate,
                                                                                                                     decimal plannedRegionalBudget,
                                                                                                                     decimal completedWorksBudget)
    {
        var actualRegionalBudget = CalculateActualRegionalBudget(totalBudget, plannedRegionalBudget, completedWorksBudget);

        var actualCofinanceRate = actualRegionalBudget / completedWorksBudget * totalRate;

        if (actualCofinanceRate <= plannedCofinanceRate)
            return (actualCofinanceRate, actualRegionalBudget);

        var decimalPlaceForCalculation = 0.01M;

        while (actualCofinanceRate > plannedCofinanceRate)
        {
            actualRegionalBudget -= decimalPlaceForCalculation;

            actualCofinanceRate = actualRegionalBudget / completedWorksBudget * totalRate;
        }

        return (actualCofinanceRate, actualRegionalBudget);
    }

    private decimal CalculateActualRegionalBudget(decimal totalBudget, decimal plannedRegionalBudget, decimal completedWorksBudget)
    {
        var actualRegionalBudget = plannedRegionalBudget / totalBudget * completedWorksBudget;

        if (actualRegionalBudget < 0)
            throw new CalculatingBudgetException("Value of the regional budget is incorrect.");

        return Math.Round(actualRegionalBudget, 2, MidpointRounding.AwayFromZero);
    }
}

