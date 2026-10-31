namespace Shekar.Domain.Entities;

/// <summary>
/// تنظیمات و محدودیت‌های سراسری کسب‌وکار به‌صورت کلید/مقدار (قابل تغییر بدون انتشار مجدد):
/// ظرفیت پیش‌فرض شهر، آستانه‌ی فرصت طلایی، کف و نرخ هزینه‌ی نقل‌وانتقال، ساعت پشتیبانی،
/// سهمیه‌ی کارشناسی رایگان مهمان و … .
/// </summary>
public class SystemSetting
{
    /// <summary>کلید یکتا، مثل «GoldenScoreThreshold».</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>مقدار به‌صورت رشته (طبق <see cref="ValueType"/> تفسیر می‌شود).</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>نوع مقدار: int | decimal | bool | time | string.</summary>
    public string ValueType { get; set; } = "string";

    /// <summary>توضیح کاربردی این تنظیم.</summary>
    public string? Description { get; set; }

    /// <summary>زمان آخرین تغییر.</summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
