using Shekar.Domain.Enums;

namespace Shekar.Domain.Entities;

/// <summary>
/// زنگوله‌ی «خبرم کن» برای یک دسته‌ی قیمتی (R6). فقط پلن حرفه‌ای.
/// پس از فعال‌سازی تا ۲۴ ساعت شکارهای تازه پیامک می‌شود، سپس خودکار خاموش می‌شود.
/// </summary>
public class BellAlert
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public PriceCategory Category { get; set; }

    /// <summary>زمان روشن‌شدن؛ <c>null</c> یعنی خاموش.</summary>
    public DateTimeOffset? ActivatedAt { get; set; }

    /// <summary>زمان خاموش‌شدن خودکار (ActivatedAt + ۲۴ ساعت).</summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}
