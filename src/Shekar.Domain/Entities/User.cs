namespace Shekar.Domain.Entities;

/// <summary>کاربر ثبت‌نام‌شده (ورود با شماره موبایل + OTP).</summary>
public class User
{
    public int Id { get; set; }

    /// <summary>شماره موبایل، شناسه‌ی یکتای ورود.</summary>
    public string Mobile { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    /// <summary>شهر انتخاب‌شده در پروفایل.</summary>
    public int? CityId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>R3 — آیا کاربر پیش‌تر از ۲۴ ساعت آزمایشی رایگان استفاده کرده است.</summary>
    public bool HasUsedTrial { get; set; }

    /// <summary>آیا پروفایل (نام/شهر) تکمیل شده؛ فقط در ورود اول پرسیده می‌شود.</summary>
    public bool ProfileCompleted { get; set; }

    /// <summary>نام کامل نمایشی.</summary>
    public string FullName => string.Join(' ', new[] { FirstName, LastName }
        .Where(p => !string.IsNullOrWhiteSpace(p)));
}
