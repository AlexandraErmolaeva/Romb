using Romb.Application.Exceptions;

namespace Romb.Application.Helpers;

public class BudgetCalculator : IBudgetCalculator
{
    private const decimal totalRate = 100M; // 100%.

    public decimal CalculatePlannedRegionalBudget(decimal totalBudget, decimal cofinanceRate)
    {
        var regionalBudget = (totalBudget / totalRate) * cofinanceRate;

        if (regionalBudget < 0)
            throw new CalculatingBudgetException("Value of the regional budget is incorrect.");

        return Math.Round(regionalBudget, 2, MidpointRounding.AwayFromZero);
    }

    public decimal CalculatePlannedLocalBudget(decimal totalBudget, decimal cofinanceRate)
    {
        var localBudget = totalBudget - (totalBudget / totalRate * cofinanceRate);

        if (localBudget < 0)
            throw new CalculatingBudgetException("Value of the local budget is incorrect.");

        return Math.Round(localBudget, 2, MidpointRounding.AwayFromZero);
    }

    public (decimal actualCofinanceRate, decimal actualRegionalBudget) CalculateActualCofinanceRateAndRegionalBudget(decimal totalBudget,
                                                                                                                     decimal plannedCofinanceRate,
                                                                                                                     decimal plannedRegionalBudget,
                                                                                                                     decimal completedWorksBudget)
    {
        var actualRegionalBudget = CalculateActualLRegionalBudget(totalBudget, plannedRegionalBudget, completedWorksBudget);

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

    public decimal CalculateActualLocalBudget(decimal completedWorksBudget, decimal actualRegionalBudget)
    {
        var actualLocalBudget = completedWorksBudget - actualRegionalBudget;

        if (actualLocalBudget < 0)
            throw new CalculatingBudgetException("Value of the local budget is incorrect.");

        return Math.Round(actualLocalBudget, 2, MidpointRounding.AwayFromZero);
    }

    public decimal ForciblyCalculateActualRegionalBudgetWithCofinanceRate(decimal completedWorksBudget,
                                                                          decimal cofinanceRate)
    {
        var actualRegionalBudget = completedWorksBudget * cofinanceRate / totalRate;

        return Math.Round(actualRegionalBudget, 2, MidpointRounding.AwayFromZero);
    }

    public decimal ForciblyCalculateActualLocalBudgetBudgetWithCofinanceRate(decimal completedWorksBudget,
                                                                             decimal cofinanceRate)
    {
        var actualLocalBudget = completedWorksBudget - completedWorksBudget * cofinanceRate / totalRate;

        return Math.Round(actualLocalBudget, 2, MidpointRounding.AwayFromZero);
    }


    private decimal CalculateActualLRegionalBudget(decimal totalBudget, decimal plannedRegionalBudget, decimal completedWorksBudget)
    {
        var actualRegionalBudget = plannedRegionalBudget / totalBudget * completedWorksBudget;

        if (actualRegionalBudget < 0)
            throw new CalculatingBudgetException("Value of the regional budget is incorrect.");

        return Math.Round(actualRegionalBudget, 2, MidpointRounding.AwayFromZero);
    }
}

