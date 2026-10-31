namespace Shekar.Domain.Enums;

/// <summary>وضعیت آگهی خام در طول چرخه‌ی کراول.</summary>
public enum ListingState
{
    /// <summary>هنوز روی منبع فعال است.</summary>
    Active = 1,

    /// <summary>دیگر در منبع دیده نشد (حذف/فروخته‌شده).</summary>
    Removed = 2
}
