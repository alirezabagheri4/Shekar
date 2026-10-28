using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Domain;

namespace Shekar.Application.Services;

/// <summary>ساعت پشتیبانی ۰۹:۰۰ تا ۱۸:۰۰ به وقت Asia/Tehran (R12).</summary>
public sealed class SupportService : ISupportService
{
    private readonly IClock _clock;

    public SupportService(IClock clock)
    {
        _clock = clock;
    }

    public bool IsOpenNow()
    {
        TimeOnly now = TimeOnly.FromTimeSpan(_clock.TehranNow.TimeOfDay);
        return now >= DomainConstants.SupportStart && now < DomainConstants.SupportEnd;
    }

    public string HoursLabel => "۹ صبح تا ۶ عصر";

    public string StatusLabel
    {
        get
        {
            string time = PersianFormat.ToPersianDigits(_clock.TehranNow.ToString("HH:mm"));
            return IsOpenNow()
                ? $"الان ساعت {time} به وقت تهرانه · آنلاینیم"
                : "بیرون از ساعت پشتیبانی هم پیام بذار؛ فردا از ۹ صبح اول به تو جواب می‌دیم.";
        }
    }
}
