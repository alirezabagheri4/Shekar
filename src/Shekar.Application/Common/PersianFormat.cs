using System.Globalization;

namespace Shekar.Application.Common;

/// <summary>کمک‌تابع‌های تبدیل ارقام به فارسی برای برچسب‌های متنی سرویس‌ها.</summary>
public static class PersianFormat
{
    private static readonly char[] PersianDigits = { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };

    /// <summary>ارقام لاتین یک رشته را به فارسی تبدیل می‌کند.</summary>
    public static string ToPersianDigits(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        Span<char> buffer = stackalloc char[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            buffer[i] = c is >= '0' and <= '9' ? PersianDigits[c - '0'] : c;
        }

        return new string(buffer);
    }

    /// <summary>عدد را با جداکننده‌ی هزارگان و ارقام فارسی برمی‌گرداند.</summary>
    public static string ToPersianNumber(long value) =>
        ToPersianDigits(value.ToString("#,0", CultureInfo.InvariantCulture));
}
