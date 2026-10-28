using Shekar.Domain.Enums;

namespace Shekar.Domain.ValueObjects;

/// <summary>
/// تعریف یک باند دسته‌ی قیمتی: مرزهای پایین/بالا بر حسب تومان و برچسب فارسی.
/// <paramref name="MinToman"/> شامل است و <paramref name="MaxTomanExclusive"/> ناشامل؛
/// برای آخرین دسته (بالای ۱۶۰۰۰) مقدار بالا <c>null</c> است.
/// </summary>
public sealed record PriceCategoryBand(
    PriceCategory Category,
    long MinToman,
    long? MaxTomanExclusive,
    string Label)
{
    /// <summary>آیا قیمت داده‌شده در این باند قرار می‌گیرد.</summary>
    public bool Contains(long priceToman) =>
        priceToman >= MinToman && (MaxTomanExclusive is null || priceToman < MaxTomanExclusive);
}
