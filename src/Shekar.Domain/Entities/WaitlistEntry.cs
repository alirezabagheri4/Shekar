namespace Shekar.Domain.Entities;

/// <summary>ثبت‌نام در لیست انتظار یک شهر وقتی ظرفیت تکمیل است (R1).</summary>
public class WaitlistEntry
{
    public int Id { get; set; }

    public int CityId { get; set; }

    /// <summary>شماره موبایل متقاضی.</summary>
    public string Mobile { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>نوبت نمایش‌داده‌شده در صف (مثلاً «نفر ۳۴ام صف تهرانی»).</summary>
    public int Position { get; set; }
}
