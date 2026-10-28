namespace Shekar.Domain.Enums;

/// <summary>وضعیت آگهیِ شکار.</summary>
public enum HuntState
{
    /// <summary>فعال؛ در لیست «شکار فعال» شمرده می‌شود و جزئیات دارد.</summary>
    Active = 1,

    /// <summary>آگهی حذف شده؛ لینک و جزئیات ندارد، کارت خاکستری نمایش داده می‌شود.</summary>
    Removed = 2
}
