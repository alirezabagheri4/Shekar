using System.Globalization;
using Shekar.Application.Common;

namespace Shekar.Web.Infrastructure;

/// <summary>قالب‌بندی نمایشی فارسی برای Viewها (قیمت، زمان نسبی، ارقام).</summary>
public static class ViewFormat
{
    public static string Digits(string input) => PersianFormat.ToPersianDigits(input);

    public static string Number(long value) => PersianFormat.ToPersianNumber(value);

    /// <summary>قیمت را به «میلیارد/میلیون» فارسی تبدیل می‌کند، مثل «۱٫۳۴ میلیارد».</summary>
    public static string Price(long toman)
    {
        if (toman >= 1_000_000_000)
        {
            double b = toman / 1_000_000_000d;
            return $"{Decimal(b)} میلیارد";
        }

        double m = toman / 1_000_000d;
        return $"{Decimal(m)} میلیون";
    }

    /// <summary>قیمت با واحد، مثل «۱٫۳۴ میلیارد تومان».</summary>
    public static string PriceToman(long toman) => $"{Price(toman)} تومان";

    /// <summary>سود تخمینی، مثل «سود ~۱۹۵ میلیون».</summary>
    public static string Profit(long toman) => $"~{Price(toman)}";

    /// <summary>زمان نسبی فارسی، مثل «۳۵ دقیقه پیش» یا «۲ ساعت پیش».</summary>
    public static string RelativeTime(DateTimeOffset publishedAt, DateTimeOffset now)
    {
        TimeSpan diff = now - publishedAt;
        if (diff < TimeSpan.Zero)
        {
            diff = TimeSpan.Zero;
        }

        if (diff.TotalMinutes < 60)
        {
            int minutes = Math.Max(1, (int)Math.Round(diff.TotalMinutes));
            return $"{Digits(minutes.ToString())} دقیقه پیش";
        }

        if (diff.TotalHours < 24)
        {
            int hours = (int)Math.Floor(diff.TotalHours);
            return $"{Digits(hours.ToString())} ساعت پیش";
        }

        int days = (int)Math.Floor(diff.TotalDays);
        return $"{Digits(days.ToString())} روز پیش";
    }

    private static string Decimal(double value)
    {
        // تا دو رقم اعشار، با حذف صفرهای انتهایی و جداکننده‌ی فارسی «٫».
        string s = Math.Round(value, 2).ToString("0.##", CultureInfo.InvariantCulture);
        return Digits(s).Replace('.', '٫');
    }
}
