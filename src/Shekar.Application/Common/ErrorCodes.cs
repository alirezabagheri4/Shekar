namespace Shekar.Application.Common;

/// <summary>شناسه‌های پایدار خطاهای کسب‌وکار (برای نگاشت به حالت‌های UI).</summary>
public static class ErrorCodes
{
    /// <summary>R1 — ظرفیت شهر تکمیل است.</summary>
    public const string CityCapacityFull = "city_capacity_full";

    /// <summary>R3 — کاربر پیش‌تر از اشتراک آزمایشی استفاده کرده.</summary>
    public const string TrialAlreadyUsed = "trial_already_used";

    /// <summary>R4 — لیست شکار برای کاربر بدون اشتراک قفل است.</summary>
    public const string HuntLocked = "hunt_locked";

    /// <summary>R5 — سهمیه‌ی کارشناسی رایگان این ماه تمام شده.</summary>
    public const string AppraisalQuotaExhausted = "appraisal_quota_exhausted";

    /// <summary>R5 — لینک نامعتبر است.</summary>
    public const string InvalidAdLink = "invalid_ad_link";

    /// <summary>R6 — «خبرم کن» فقط در پلن حرفه‌ای.</summary>
    public const string BellProOnly = "bell_pro_only";

    /// <summary>R7 — آگهی حذف شده.</summary>
    public const string HuntRemoved = "hunt_removed";
}
