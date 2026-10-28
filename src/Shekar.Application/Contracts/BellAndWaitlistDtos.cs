using System.ComponentModel.DataAnnotations;
using Shekar.Domain.Enums;

namespace Shekar.Application.Contracts;

/// <summary>درخواست روشن/خاموش‌کردن زنگوله‌ی «خبرم کن» (R6).</summary>
public sealed class BellToggleRequestDto
{
    [Required]
    public PriceCategory Category { get; set; }
}

/// <summary>نتیجه‌ی تغییر وضعیت زنگوله.</summary>
public sealed record BellToggleResultDto
{
    public PriceCategory Category { get; init; }
    public bool IsOn { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>برچسب زمان باقی‌مانده، مثل «۲۱ ساعت مونده».</summary>
    public string? RemainingLabel { get; init; }
}

/// <summary>درخواست ثبت در لیست انتظار شهرِ پرظرفیت (R1).</summary>
public sealed class WaitlistJoinRequestDto
{
    [Range(1, int.MaxValue)]
    public int CityId { get; set; }

    [Required(ErrorMessage = "شماره موبایل لازمه.")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل معتبر نیست.")]
    public string Mobile { get; set; } = string.Empty;
}

/// <summary>نوبت کاربر در لیست انتظار.</summary>
public sealed record WaitlistPositionDto
{
    public int Position { get; init; }
    public string CityName { get; init; } = string.Empty;

    /// <summary>برچسب نمایشی، مثل «نفر ۳۴ام صف تهرانی».</summary>
    public string Label { get; init; } = string.Empty;
}
