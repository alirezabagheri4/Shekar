using System.ComponentModel.DataAnnotations;
using Shekar.Domain.Enums;

namespace Shekar.Application.Contracts;

/// <summary>پلن قابل‌نمایش.</summary>
public sealed record PlanDto
{
    public PlanType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public long MonthlyPriceToman { get; init; }
    public IReadOnlyList<string> Features { get; init; } = Array.Empty<string>();
    public bool HasBellAlerts { get; init; }
    public bool IsRecommended { get; init; }
}

/// <summary>ظرفیت اشتراک یک شهر (R1).</summary>
public sealed record CityCapacityDto
{
    public int CityId { get; init; }
    public string CityName { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public int Used { get; init; }
    public int Remaining { get; init; }
    public bool IsFull { get; init; }
}

/// <summary>وضعیت اشتراک کاربر جاری.</summary>
public sealed record SubscriptionStatusDto
{
    public bool IsSubscriber { get; init; }
    public PlanType? PlanType { get; init; }
    public SubscriptionStatus? Status { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public bool HasUsedTrial { get; init; }
}

/// <summary>درخواست شروع اشتراک (آزمایشی یا خرید).</summary>
public sealed class StartSubscriptionRequestDto
{
    [Required]
    public PlanType PlanType { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "شهر نامعتبر است.")]
    public int CityId { get; set; }
}

/// <summary>نتیجه‌ی خرید/پرداخت.</summary>
public sealed record PurchaseResultDto
{
    public int SubscriptionId { get; init; }
    public PlanType PlanType { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public string ExpiresLabel { get; init; } = string.Empty;

    /// <summary>کد پیگیری پرداخت (نمایشی).</summary>
    public string TrackingCode { get; init; } = string.Empty;
}
