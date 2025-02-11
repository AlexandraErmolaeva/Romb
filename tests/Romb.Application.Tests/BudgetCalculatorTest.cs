using Romb.Application.Exceptions;
using Romb.Application.Helpers;

namespace Romb.Application.Tests;

public class BudgetCalculatorTest
{
    private readonly IBudgetCalculator _budgetCalculator;

    public BudgetCalculatorTest()
    {
        _budgetCalculator = new BudgetCalculator();
    }

    #region [CalculatePlannedRegionalBudget]
    [Fact]
    public void CalculatePlannedRegionalBudget_ReturnCorrectValue()
    {
        var totalBudget = 1000M;
        var cofinanceRate = 20M;

        var result = _budgetCalculator.CalculateRegionalBudget(totalBudget, cofinanceRate);

        Assert.Equal(200M, result);
    }

    [Fact]
    public void CalculatePlannedRegionalBudget_ReturnThrowException()
    {
        var totalBudget = -1000M;
        var cofinanceRate = 0.2M;

        Assert.Throws<CalculatingBudgetException>(() =>
            _budgetCalculator.CalculateRegionalBudget(totalBudget, cofinanceRate));
    }

    [Fact]
    public void CalculatePlannedRegionalBudget_ReturnZero()
    {
        var totalBudget = 1000M;
        var cofinanceRate = 0M;

        var result = _budgetCalculator.CalculateRegionalBudget(totalBudget, cofinanceRate);

        Assert.Equal(0M, result);
    }

    [Fact]
    public void CalculatePlannedRegionalBudget_ReturnCorrectValue_WithMaxDecimalValue()
    {
        var totalBudget = decimal.MaxValue;
        var cofinanceRate = 50M;

        var result = _budgetCalculator.CalculateRegionalBudget(totalBudget, cofinanceRate);

        Assert.Equal(totalBudget / 2, result);
    }
    #endregion

    #region [CalculateLocalBudget]
    [Fact]
    public void CalculatePlannedLocalBudget_ReturnCorrectValue()
    {
        var totalBudget = 1000M;
        var cofinanceRate = 20M;

        var result = _budgetCalculator.CalculateLocalBudget(totalBudget, cofinanceRate);

        Assert.Equal(800M, result);
    }

    [Fact]
    public void CalculatePlannedLocalBudget_ReturnThrowException()
    {
        var totalBudget = -1000M;
        var cofinanceRate = 0.2M;

        Assert.Throws<CalculatingBudgetException>(() =>
            _budgetCalculator.CalculateLocalBudget(totalBudget, cofinanceRate));
    }

    [Fact]
    public void CalculatePlannedLocalBudget_ReturnZero()
    {
        var totalBudget = 1000M;
        var cofinanceRate = 100M;

        var result = _budgetCalculator.CalculateLocalBudget(totalBudget, cofinanceRate);

        Assert.Equal(0M, result);
    }

    [Fact]
    public void CalculatePlannedLocalBudget_ReturnCorrectValue_WithMaxDecimalValue()
    {
        var totalBudget = decimal.MaxValue;
        var cofinanceRate = 50M;

        var result = _budgetCalculator.CalculateLocalBudget(totalBudget, cofinanceRate);
        var expectedResult = totalBudget / 2;

        Assert.Equal(expectedResult, result);
    }
    #endregion

    [Fact]
    public void CalculateActualCofinanceRateAndRegionalBudget_ReturnCorrectValue()
    {
        var totalBudget = 2250000M;
        var plannedCofinanceRate = 89M;
        var plannedRegionalBudget = 2000000M;
        var completedWorksBudget = 1992107M; 

        var (actualCofinanceRate, actualRegionalBudget) = _budgetCalculator.CalculateActualCofinanceRateAndRegionalBudget(totalBudget, plannedCofinanceRate, plannedRegionalBudget, completedWorksBudget);

        Assert.Equal(1770761.78M, actualRegionalBudget);
    }
}
