using Shekar.Domain.Enums;

namespace Shekar.Domain.Entities;

/// <summary>
/// شماره‌ی بلاک‌شده: آگهی‌های این شماره از لیست پیشنهادهای روزانه (شکارها) حذف می‌شوند.
/// برای مسدودکردن دلال‌ها، اسپمرها و آگهی‌های با اطلاعات نادرست.
/// </summary>
public class BlockedAdvertiser
{
    public int Id { get; set; }

    /// <summary>شماره موبایل بلاک‌شده.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>دلیل بلاک.</summary>
    public BlockReason Reason { get; set; }

    /// <summary>توضیح تکمیلی (مثلاً «۳۲ آگهی در یک هفته»).</summary>
    public string? Note { get; set; }

    /// <summary>زمان بلاک‌شدن.</summary>
    public DateTimeOffset BlockedAt { get; set; }

    /// <summary>شناسه‌ی ادمینی که بلاک کرده؛ <c>null</c> یعنی سیستمی/خودکار.</summary>
    public int? BlockedByUserId { get; set; }

    /// <summary>انقضای بلاک؛ <c>null</c> یعنی دائمی.</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>آیا بلاک هم‌اکنون فعال است.</summary>
    public bool IsActive { get; set; } = true;
}
