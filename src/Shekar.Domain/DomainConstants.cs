using Shekar.Domain.Enums;
using Shekar.Domain.ValueObjects;

namespace Shekar.Domain;

/// <summary>ثابت‌ها و مرزهای دامنه‌ی کسب‌وکار شکار (مطابق بخش «قوانین»).</summary>
public static class DomainConstants
{
    /// <summary>یک میلیون تومان.</summary>
    public const long Million = 1_000_000L;

    /// <summary>یک میلیارد تومان.</summary>
    public const long Billion = 1_000_000_000L;

    /// <summary>R1 — ظرفیت هر شهر: ۱۰۰ اشتراک فعال (Trial + Active).</summary>
    public const int CityCapacity = 100;

    /// <summary>R3 — مدت اشتراک آزمایشی رایگان: ۲۴ ساعت، یک‌بار برای هر کاربر تازه.</summary>
    public static readonly TimeSpan TrialDuration = TimeSpan.FromHours(24);

    /// <summary>R5 — سهمیه‌ی کارشناسی رایگان ماهانه برای کاربر بدون اشتراک.</summary>
    public const int FreeAppraisalsPerMonth = 3;

    /// <summary>R6 — مدت فعال‌بودن زنگوله‌ی «خبرم کن» پیش از خاموش‌شدن خودکار: ۲۴ ساعت.</summary>
    public static readonly TimeSpan BellAlertDuration = TimeSpan.FromHours(24);

    /// <summary>R8 — حداقل امتیاز برای «فرصت طلایی».</summary>
    public const int GoldenScoreThreshold = 85;

    /// <summary>R9 — کف هزینه‌ی نقل‌وانتقال و فروش (۱۵ میلیون تومان).</summary>
    public const long TransactionCostFloorToman = 15 * Million;

    /// <summary>R9 — نرخ هزینه‌ی نقل‌وانتقال نسبت به قیمت آگهی (۲٪).</summary>
    public const double TransactionCostRate = 0.02;

    /// <summary>R12 — ساعت شروع پشتیبانی به وقت Asia/Tehran.</summary>
    public static readonly TimeOnly SupportStart = new(9, 0);

    /// <summary>R12 — ساعت پایان پشتیبانی به وقت Asia/Tehran.</summary>
    public static readonly TimeOnly SupportEnd = new(18, 0);

    /// <summary>شناسه‌ی منطقه‌ی زمانی تهران.</summary>
    public const string TehranTimeZoneId = "Asia/Tehran";

    /// <summary>پانویس مشترک همه‌ی صفحات.</summary>
    public const string Disclaimer =
        "قیمت و سود تخمینیه و تضمینی نیست؛ قبل از خرید حتماً کارشناسی کن.";

    /// <summary>R10 — هشت دسته‌ی قیمتی به ترتیب، با مرزها (تومان) و برچسب فارسی.</summary>
    public static readonly IReadOnlyList<PriceCategoryBand> PriceCategoryBands = new[]
    {
        new PriceCategoryBand(PriceCategory.Under1B,     0,              1000L * Million, "زیر ۱ میلیارد"),
        new PriceCategoryBand(PriceCategory.From1To1_5B, 1000L * Million, 1500L * Million, "۱ تا ۱٫۵ میلیارد"),
        new PriceCategoryBand(PriceCategory.From1_5To2B, 1500L * Million, 2000L * Million, "۱٫۵ تا ۲ میلیارد"),
        new PriceCategoryBand(PriceCategory.From2To3B,   2000L * Million, 3000L * Million, "۲ تا ۳ میلیارد"),
        new PriceCategoryBand(PriceCategory.From3To4B,   3000L * Million, 4000L * Million, "۳ تا ۴ میلیارد"),
        new PriceCategoryBand(PriceCategory.From4To8B,   4000L * Million, 8000L * Million, "۴ تا ۸ میلیارد"),
        new PriceCategoryBand(PriceCategory.From8To16B,  8000L * Million, 16000L * Million, "۸ تا ۱۶ میلیارد"),
        new PriceCategoryBand(PriceCategory.Above16B,    16000L * Million, null,            "بالای ۱۶ میلیارد"),
    };

    /// <summary>دسته‌ی قیمتی متناظر با قیمت داده‌شده (تومان) را برمی‌گرداند.</summary>
    public static PriceCategory ResolveCategory(long priceToman)
    {
        foreach (PriceCategoryBand band in PriceCategoryBands)
        {
            if (band.Contains(priceToman))
            {
                return band.Category;
            }
        }

        // قیمت‌های بسیار بالا در آخرین دسته قرار می‌گیرند.
        return PriceCategory.Above16B;
    }

    /// <summary>آستانه‌ی قیمتی که بالای آن «رنگ» در سگمنت‌بندی قیمت دخیل می‌شود (۴ میلیارد).</summary>
    public const long ColorMattersAboveToman = 4_000L * Million;

    /// <summary>باکت کارکرد (کیلومتر) را برای دسته‌بندی هوشمند قیمت برمی‌گرداند.</summary>
    public static MileageBucket ResolveMileageBucket(int? mileageKm)
    {
        int km = mileageKm ?? 0;
        return km switch
        {
            <= 0 => MileageBucket.Zero,
            <= 20_000 => MileageBucket.To20k,
            <= 50_000 => MileageBucket.To50k,
            <= 100_000 => MileageBucket.To100k,
            <= 150_000 => MileageBucket.To150k,
            _ => MileageBucket.Above150k
        };
    }
}
