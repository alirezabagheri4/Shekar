namespace Shekar.Domain.Entities;

/// <summary>
/// آگهی‌دهنده (بر اساس شماره موبایل). حجم آگهی‌گذاری را دنبال می‌کنیم تا دلال‌ها و
/// اسپمرها را تشخیص داده و در صورت نیاز بلاک کنیم (<see cref="BlockedAdvertiser"/>).
/// </summary>
public class Advertiser
{
    public int Id { get; set; }

    /// <summary>شماره موبایل آگهی‌دهنده؛ شناسه‌ی یکتا.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>نام نمایشی/مستعار آگهی‌دهنده (در صورت وجود).</summary>
    public string? DisplayName { get; set; }

    /// <summary>اولین باری که آگهی از این شماره دیده شد.</summary>
    public DateTimeOffset FirstSeenAt { get; set; }

    /// <summary>آخرین باری که آگهی از این شماره ثبت شد.</summary>
    public DateTimeOffset? LastAdAt { get; set; }

    /// <summary>مجموع کل آگهی‌های ثبت‌شده با این شماره.</summary>
    public int TotalAdsPosted { get; set; }

    /// <summary>تعداد آگهی در ۳۰ روز اخیر؛ مبنای تشخیص آگهی‌گذاری بیش از حد.</summary>
    public int AdsPostedLast30Days { get; set; }

    /// <summary>آیا این آگهی‌دهنده معتبر/تأییدشده است (از فیلترها مستثنا می‌شود).</summary>
    public bool IsTrusted { get; set; }
}
