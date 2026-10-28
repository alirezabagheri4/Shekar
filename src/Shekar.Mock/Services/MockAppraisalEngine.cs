using Shekar.Application.Abstractions;
using Shekar.Application.Contracts;
using Shekar.Application.Services;

namespace Shekar.Mock.Services;

/// <summary>موتور کارشناسی ماک: نتیجه‌ی ثابتِ نمونه (پژو ۲۰۷ — analyze-result) را برمی‌گرداند.</summary>
public sealed class MockAppraisalEngine : IAppraisalEngine
{
    public AppraisalResultDto Analyze(string adUrl)
    {
        const long price = 1_340_000_000;
        const long market = 1_560_000_000;

        var card = new HuntCardDto
        {
            Id = 0,
            Title = "پژو ۲۰۷ پانوراما ۱۴۰۲",
            ModelYear = "۱۴۰۲",
            MileageKm = 28_000,
            Location = "تهرانپارس",
            PriceToman = price,
            MarketMedianToman = market,
            BelowMarketPercent = 14,
            Score = 87,
            IsGolden = HuntCalculations.IsGolden(87),
            Liquidity = Domain.Enums.Liquidity.High,
            EstimatedProfitToman = HuntCalculations.EstimatedNetProfit(market, price),
            Category = Domain.DomainConstants.ResolveCategory(price)
        };

        var detail = new HuntDetailDto
        {
            Card = card,
            ComparableCount = 42,
            Confidence = "بالا",
            FairRangeMinToman = 1_520_000_000,
            FairRangeMaxToman = 1_600_000_000,
            Summary = "قیمتش واقعاً خوبه و ماشین پرطرفداره. چون رنگ‌شدگی ذکر شده، حتماً کارشناسی بدنه بگیر.",
            Factors = new[]
            {
                new AnalysisFactorDto("قیمت", "عالی", "۱۴٪ زیر میانه‌ی بازار"),
                new AnalysisFactorDto("وضعیت فنی", "خیلی خوب", "طبق آگهی: یه لکه رنگ گلگیر جلو"),
                new AnalysisFactorDto("کارکرد", "خیلی خوب", "۲۸ هزار کیلومتر؛ کمتر از میانگین این مدل"),
                new AnalysisFactorDto("نقدشوندگی", "عالی", "۲۰۷های مشابه معمولاً ۲ تا ۴ روزه فروش می‌رن"),
                new AnalysisFactorDto("ریسک", "کم", "۲ نکته قبل از خرید چک کن")
            },
            Risks = new[] { "رنگ‌شدگی ذکر شده", "بیمه‌ی معتبر", "نقدشوندگی بالا" },
            TransactionCostToman = HuntCalculations.TransactionCost(price)
        };

        return new AppraisalResultDto { Detail = detail, Verdict = "فرصت طلایی" };
    }
}
