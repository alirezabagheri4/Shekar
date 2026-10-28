namespace Shekar.Domain.Entities;

/// <summary>شهر تحت پوشش سرویس. فعلاً فقط تهران فعال است.</summary>
public class City
{
    public int Id { get; set; }

    /// <summary>نام نمایشی، مثل «تهران».</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>اسلاگ انگلیسی برای مسیرها، مثل «tehran».</summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>آیا شهر فعال است یا «به‌زودی».</summary>
    public bool IsActive { get; set; }

    /// <summary>ظرفیت اشتراک فعال شهر (پیش‌فرض ۱۰۰).</summary>
    public int Capacity { get; set; } = DomainConstants.CityCapacity;
}
