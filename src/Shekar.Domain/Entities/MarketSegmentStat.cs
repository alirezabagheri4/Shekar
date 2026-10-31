using Shekar.Domain.Enums;

namespace Shekar.Domain.Entities;

/// <summary>
/// آمار یک «سگمنت» بازار که Job ساعتی محاسبه می‌کند. مرجع تشخیص زیرقیمت‌بودن در اسکورینگ.
/// کلید سگمنت: مدل + سال + باکتِ‌کارکرد + وضعیت‌بدنه [+ رنگ اگر خودرو گران باشد].
/// </summary>
public class MarketSegmentStat
{
    public int Id { get; set; }

    /// <summary>کلید متنیِ یکتای سگمنت (برای upsert سریع).</summary>
    public string SegmentKey { get; set; } = string.Empty;

    public int VehicleModelId { get; set; }

    public int ModelYear { get; set; }

    public MileageBucket MileageBucket { get; set; }

    public BodyCondition BodyCondition { get; set; }

    /// <summary>کلید رنگ؛ NULL یعنی رنگ در این سگمنت دخیل نیست (خودروی غیرگران).</summary>
    public string? ColorKey { get; set; }

    /// <summary>تعداد نمونه‌ی این سگمنت در پنجره‌ی زمانی.</summary>
    public int SampleCount { get; set; }

    /// <summary>میانه‌ی قیمت (تومان) — مرجع اصلی (مقاوم در برابر آگهی‌های پرت).</summary>
    public long MedianPriceToman { get; set; }

    /// <summary>میانگین قیمت (تومان).</summary>
    public long AveragePriceToman { get; set; }

    /// <summary>چارک اول (۲۵٪) قیمت.</summary>
    public long P25PriceToman { get; set; }

    /// <summary>چارک سوم (۷۵٪) قیمت.</summary>
    public long P75PriceToman { get; set; }

    /// <summary>طول پنجره‌ی زمانی نمونه‌گیری (روز).</summary>
    public int WindowDays { get; set; }

    /// <summary>زمان آخرین محاسبه.</summary>
    public DateTimeOffset ComputedAt { get; set; }
}
