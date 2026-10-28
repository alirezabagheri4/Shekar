namespace Shekar.Application.Abstractions;

/// <summary>تقویم شمسی برای شمارش سهمیه‌ی ماهانه (R5) و نمایش تاریخ.</summary>
public interface IJalaliCalendar
{
    /// <summary>سال شمسی یک لحظه.</summary>
    int GetYear(DateTimeOffset tehranTime);

    /// <summary>ماه شمسی (۱ تا ۱۲) یک لحظه.</summary>
    int GetMonth(DateTimeOffset tehranTime);

    /// <summary>بازه‌ی [شروع، شروعِ ماه بعد) ماهِ شمسیِ شاملِ <paramref name="tehranTime"/> به‌صورت UTC.</summary>
    (DateTimeOffset StartUtc, DateTimeOffset NextStartUtc) GetMonthRangeUtc(DateTimeOffset tehranTime);

    /// <summary>نام ماه شمسی، مثل «آذر».</summary>
    string GetMonthName(int month);

    /// <summary>تاریخ شمسی قابل‌نمایش، مثل «۳۰ آبان ۱۴۰۵».</summary>
    string FormatDate(DateTimeOffset tehranTime);
}
