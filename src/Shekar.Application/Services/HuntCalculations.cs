using Shekar.Domain;

namespace Shekar.Application.Services;

/// <summary>محاسبات خالص قوانین کسب‌وکار مربوط به شکار (R8، R9). بدون حالت و قابل تست مستقیم.</summary>
public static class HuntCalculations
{
    /// <summary>R8 — امتیاز ≥ ۸۵ یعنی «فرصت طلایی».</summary>
    public static bool IsGolden(int score) => score >= DomainConstants.GoldenScoreThreshold;

    /// <summary>R9 — هزینه‌ی نقل‌وانتقال و فروش = max(۱۵ میلیون، ۲٪ قیمت).</summary>
    public static long TransactionCost(long priceToman)
    {
        long twoPercent = (long)Math.Round(priceToman * DomainConstants.TransactionCostRate);
        return Math.Max(DomainConstants.TransactionCostFloorToman, twoPercent);
    }

    /// <summary>R9 — سود خالص تخمینی = میانه‌ی بازار − قیمت آگهی − هزینه‌ی نقل‌وانتقال.</summary>
    public static long EstimatedNetProfit(long marketMedianToman, long priceToman) =>
        marketMedianToman - priceToman - TransactionCost(priceToman);
}
