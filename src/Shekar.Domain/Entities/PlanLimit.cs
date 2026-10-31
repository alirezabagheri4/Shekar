namespace Shekar.Domain.Entities;

/// <summary>
/// محدودیت‌ها و امکانات هر پلن (یک‌به‌یک با <see cref="Plan"/>). قوانین پلن را
/// داده‌محور می‌کند تا بدون تغییر کد قابل تنظیم باشد.
/// </summary>
public class PlanLimit
{
    /// <summary>کلید اصلی و کلید خارجی به <see cref="Plan"/>.</summary>
    public int PlanId { get; set; }

    /// <summary>آیا کارشناسی نامحدود است (مشترک‌ها).</summary>
    public bool UnlimitedAppraisals { get; set; }

    /// <summary>سقف کارشناسی ماهانه وقتی نامحدود نیست؛ <c>null</c> یعنی نامحدود.</summary>
    public int? MonthlyAppraisalLimit { get; set; }

    /// <summary>R6 — آیا قابلیت «خبرم کن» فعال است (فقط پلن حرفه‌ای).</summary>
    public bool AllowBellAlerts { get; set; }

    /// <summary>مدت فعال‌بودن هر زنگوله به ساعت.</summary>
    public int BellAlertDurationHours { get; set; } = 24;

    /// <summary>بیشینه‌ی زنگوله‌های فعالِ هم‌زمان؛ <c>null</c> یعنی نامحدود.</summary>
    public int? MaxActiveBells { get; set; }

    /// <summary>R3 — آیا پلن اشتراک آزمایشی رایگان دارد.</summary>
    public bool TrialAllowed { get; set; }

    /// <summary>مدت اشتراک آزمایشی به ساعت.</summary>
    public int TrialHours { get; set; } = 24;

    /// <summary>آیا شکارها در لحظه نمایش داده می‌شوند.</summary>
    public bool RealtimeHunts { get; set; } = true;

    /// <summary>آیا پشتیبانی اولویت‌دار دارد.</summary>
    public bool PrioritySupport { get; set; }

    public Plan Plan { get; set; } = null!;
}
