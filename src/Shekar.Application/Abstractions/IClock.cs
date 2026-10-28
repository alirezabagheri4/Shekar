namespace Shekar.Application.Abstractions;

/// <summary>ساعت سیستم. برای تست‌پذیری به‌جای <c>DateTimeOffset.UtcNow</c> استفاده می‌شود.</summary>
public interface IClock
{
    /// <summary>زمان فعلی UTC.</summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>زمان فعلی به وقت Asia/Tehran (R12 و محاسبات ماه شمسی).</summary>
    DateTimeOffset TehranNow { get; }
}
