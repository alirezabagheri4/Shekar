namespace Shekar.Domain.Entities;

/// <summary>کد یک‌بارمصرف ۵ رقمی برای ورود. در Mock همیشه «۱۲۳۴۵».</summary>
public class OtpCode
{
    public int Id { get; set; }

    public string Mobile { get; set; } = string.Empty;

    /// <summary>کد ۵ رقمی.</summary>
    public string Code { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>آیا کد قبلاً استفاده/تأیید شده است.</summary>
    public bool Consumed { get; set; }
}
