namespace Shekar.Domain.Enums;

/// <summary>وضعیت اشتراک کاربر. Trial و Active هر دو «فعال» حساب می‌شوند و در ظرفیت شهر شمرده می‌شوند.</summary>
public enum SubscriptionStatus
{
    /// <summary>۲۴ ساعت آزمایشی رایگان.</summary>
    Trial = 1,

    /// <summary>اشتراک پولی فعال.</summary>
    Active = 2,

    /// <summary>منقضی‌شده؛ در ظرفیت شمرده نمی‌شود.</summary>
    Expired = 3
}
