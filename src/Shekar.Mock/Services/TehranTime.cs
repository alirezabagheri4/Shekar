using Shekar.Domain;

namespace Shekar.Mock.Services;

/// <summary>منطقه‌ی زمانی تهران و تبدیل‌های مشترک.</summary>
internal static class TehranTime
{
    public static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTimeOffset ToTehran(DateTimeOffset utc) =>
        TimeZoneInfo.ConvertTime(utc, Zone);

    private static TimeZoneInfo ResolveZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(DomainConstants.TehranTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            // جایگزین ثابت +۳:۳۰ اگر پایگاه‌داده‌ی منطقه در دسترس نبود.
            return TimeZoneInfo.CreateCustomTimeZone("Tehran", TimeSpan.FromHours(3.5), "Tehran", "Tehran");
        }
    }
}
