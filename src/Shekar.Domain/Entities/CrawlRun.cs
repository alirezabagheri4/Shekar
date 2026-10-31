using Shekar.Domain.Enums;

namespace Shekar.Domain.Entities;

/// <summary>لاگ هر پاس کراول؛ برای پایش سلامت و عیب‌یابی خط لوله.</summary>
public class CrawlRun
{
    public long Id { get; set; }

    /// <summary>منبع کراول‌شده.</summary>
    public ListingSource Source { get; set; }

    /// <summary>شهر هدف؛ NULL یعنی همه‌ی شهرها.</summary>
    public int? CityId { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset? FinishedAt { get; set; }

    /// <summary>وضعیت: 1=در حال اجرا، 2=موفق، 3=خطا.</summary>
    public CrawlStatus Status { get; set; } = CrawlStatus.Running;

    /// <summary>تعداد آگهیِ دریافت‌شده از منبع.</summary>
    public int FetchedCount { get; set; }

    /// <summary>تعداد آگهیِ تازه‌ی ذخیره‌شده.</summary>
    public int SavedCount { get; set; }

    /// <summary>تعداد ردشده (تکراری/بدون‌عکس/قدیمی/بلاک‌شده/…).</summary>
    public int SkippedCount { get; set; }

    /// <summary>تعداد آگهیِ حذف‌شده‌ی تشخیص‌داده‌شده.</summary>
    public int RemovedCount { get; set; }

    /// <summary>تعداد خطا حین پردازش.</summary>
    public int ErrorCount { get; set; }

    /// <summary>پیام خطا در صورت شکست.</summary>
    public string? Error { get; set; }
}
