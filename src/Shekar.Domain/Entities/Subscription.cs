using Shekar.Domain.Enums;

namespace Shekar.Domain.Entities;

/// <summary>اشتراک کاربر در یک شهر. Trial و Active هر دو در ظرفیت شهر شمرده می‌شوند.</summary>
public class Subscription
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CityId { get; set; }

    public PlanType PlanType { get; set; }

    public SubscriptionStatus Status { get; set; }

    public DateTimeOffset StartsAt { get; set; }

    /// <summary>پایان اعتبار (برای Trial: شروع + ۲۴ ساعت؛ برای Active: ماهانه).</summary>
    public DateTimeOffset ExpiresAt { get; set; }
}
