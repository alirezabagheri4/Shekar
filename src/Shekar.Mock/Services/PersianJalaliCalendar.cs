using System.Globalization;
using Shekar.Application.Abstractions;
using Shekar.Application.Common;

namespace Shekar.Mock.Services;

/// <summary>تقویم شمسی مبتنی بر <see cref="PersianCalendar"/> دات‌نت.</summary>
public sealed class PersianJalaliCalendar : IJalaliCalendar
{
    private static readonly PersianCalendar Calendar = new();

    private static readonly string[] MonthNames =
    {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };

    public int GetYear(DateTimeOffset tehranTime) => Calendar.GetYear(tehranTime.DateTime);

    public int GetMonth(DateTimeOffset tehranTime) => Calendar.GetMonth(tehranTime.DateTime);

    public (DateTimeOffset StartUtc, DateTimeOffset NextStartUtc) GetMonthRangeUtc(DateTimeOffset tehranTime)
    {
        int year = Calendar.GetYear(tehranTime.DateTime);
        int month = Calendar.GetMonth(tehranTime.DateTime);

        DateTime startLocal = Calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);

        int nextYear = month == 12 ? year + 1 : year;
        int nextMonth = month == 12 ? 1 : month + 1;
        DateTime nextLocal = Calendar.ToDateTime(nextYear, nextMonth, 1, 0, 0, 0, 0);

        return (ToUtc(startLocal), ToUtc(nextLocal));
    }

    public string GetMonthName(int month) => MonthNames[Math.Clamp(month, 1, 12) - 1];

    public string FormatDate(DateTimeOffset tehranTime)
    {
        DateTime d = tehranTime.DateTime;
        int day = Calendar.GetDayOfMonth(d);
        int month = Calendar.GetMonth(d);
        int year = Calendar.GetYear(d);
        return PersianFormat.ToPersianDigits($"{day} {GetMonthName(month)} {year}");
    }

    private static DateTimeOffset ToUtc(DateTime tehranLocal)
    {
        DateTime unspecified = DateTime.SpecifyKind(tehranLocal, DateTimeKind.Unspecified);
        TimeSpan offset = TehranTime.Zone.GetUtcOffset(unspecified);
        return new DateTimeOffset(unspecified, offset).ToUniversalTime();
    }
}
