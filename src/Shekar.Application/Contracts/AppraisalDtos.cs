using System.ComponentModel.DataAnnotations;

namespace Shekar.Application.Contracts;

/// <summary>درخواست کارشناسی قیمت با لینک آگهی.</summary>
public sealed class AppraisalRequestDto
{
    [Required(ErrorMessage = "لینک نامعتبر است!")]
    [Url(ErrorMessage = "لینک نامعتبر است!")]
    public string AdUrl { get; set; } = string.Empty;
}

/// <summary>وضعیت سهمیه‌ی کارشناسی رایگان (R5).</summary>
public sealed record AppraisalQuotaDto
{
    public bool IsSubscriber { get; init; }
    public int Limit { get; init; }
    public int Used { get; init; }
    public int Remaining { get; init; }

    /// <summary>برچسب شروع ماه بعد، مثل «اول آذر».</summary>
    public string NextResetLabel { get; init; } = string.Empty;
}

/// <summary>نتیجه‌ی کارشناسی.</summary>
public sealed record AppraisalResultDto
{
    public HuntDetailDto Detail { get; init; } = default!;
    public string Verdict { get; init; } = string.Empty;
}

/// <summary>یک ردیف تاریخچه‌ی کارشناسی در صفحه‌ی حساب.</summary>
public sealed record AppraisalHistoryItemDto
{
    public string Title { get; init; } = string.Empty;
    public int Score { get; init; }
    public string Verdict { get; init; } = string.Empty;
    public string DateLabel { get; init; } = string.Empty;
    public bool IsGolden { get; init; }
}
