using Shekar.Application.Services;
using Shekar.Domain;
using Xunit;

namespace Shekar.Application.Tests;

public class HuntCalculationsTests
{
    [Theory]
    [InlineData(84, false)]
    [InlineData(85, true)]  // R8 — مرز دقیق فرصت طلایی.
    [InlineData(86, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    public void IsGolden_UsesThreshold85(int score, bool expected)
    {
        Assert.Equal(expected, HuntCalculations.IsGolden(score));
    }

    [Fact]
    public void TransactionCost_AppliesFloorWhenTwoPercentIsSmaller()
    {
        // قیمت ۵۰۰م → ۲٪ = ۱۰م < ۱۵م ⇒ کف ۱۵م اعمال می‌شود.
        long cost = HuntCalculations.TransactionCost(500 * DomainConstants.Million);
        Assert.Equal(15 * DomainConstants.Million, cost);
    }

    [Fact]
    public void TransactionCost_UsesTwoPercentWhenLargerThanFloor()
    {
        // قیمت ۱٫۳۴ میلیارد → ۲٪ بیشتر از کف ۱۵م است.
        long cost = HuntCalculations.TransactionCost(1_340_000_000);
        Assert.Equal(26_800_000, cost);
    }

    [Fact]
    public void EstimatedNetProfit_MatchesFormula()
    {
        // R9 — میانه − قیمت − max(۱۵م، ۲٪ قیمت). نمونه‌ی پژو ۲۰۷.
        long profit = HuntCalculations.EstimatedNetProfit(
            marketMedianToman: 1_560_000_000,
            priceToman: 1_340_000_000);

        Assert.Equal(1_560_000_000 - 1_340_000_000 - 26_800_000, profit);
        Assert.Equal(193_200_000, profit);
    }
}
