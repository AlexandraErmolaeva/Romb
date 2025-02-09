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

        var result = _budgetCalculator.CalculatePlannedRegionalBudget(totalBudget, cofinanceRate);

        Assert.Equal(200M, result);
    }

    [Fact]
    public void CalculatePlannedRegionalBudget_ReturnThrowException()
    {
        var totalBudget = -1000M;
        var cofinanceRate = 0.2M;

        Assert.Throws<CalculatingBudgetException>(() =>
            _budgetCalculator.CalculatePlannedRegionalBudget(totalBudget, cofinanceRate));
    }

    [Fact]
    public void CalculatePlannedRegionalBudget_ReturnZero()
    {
        var totalBudget = 1000M;
        var cofinanceRate = 0M; 

        var result = _budgetCalculator.CalculatePlannedRegionalBudget(totalBudget, cofinanceRate);

        Assert.Equal(0M, result);
    }

    [Fact]
    public void CalculatePlannedRegionalBudget_ReturnCorrectValue_WithMaxDecimalValue()
    {
        var totalBudget = decimal.MaxValue;
        var cofinanceRate = 50M;

        var result = _budgetCalculator.CalculatePlannedRegionalBudget(totalBudget, cofinanceRate);

        Assert.Equal(totalBudget / 2, result);
    }
    #endregion

    #region [CalculateLocalBudget]
    [Fact]
    public void CalculatePlannedLocalBudget_ReturnCorrectValue()
    {
        var totalBudget = 1000M;
        var cofinanceRate = 20M;

        var result = _budgetCalculator.CalculatePlannedLocalBudget(totalBudget, cofinanceRate);

        Assert.Equal(800M, result);
    }

    [Fact]
    public void CalculatePlannedLocalBudget_ReturnThrowException()
    {
        var totalBudget = -1000M;
        var cofinanceRate = 0.2M;

        Assert.Throws<CalculatingBudgetException>(() =>
            _budgetCalculator.CalculatePlannedLocalBudget(totalBudget, cofinanceRate));
    }

    [Fact]
    public void CalculatePlannedLocalBudget_ReturnZero()
    {
        var totalBudget = 1000M;
        var cofinanceRate = 100M;

        var result = _budgetCalculator.CalculatePlannedLocalBudget(totalBudget, cofinanceRate);

        Assert.Equal(0M, result);
    }

    [Fact]
    public void CalculatePlannedLocalBudget_ReturnCorrectValue_WithMaxDecimalValue()
    {
        var totalBudget = decimal.MaxValue;
        var cofinanceRate = 50M;

        var result = _budgetCalculator.CalculatePlannedLocalBudget(totalBudget, cofinanceRate);

        Assert.Equal(totalBudget / 2, result);
    }
    #endregion
}
