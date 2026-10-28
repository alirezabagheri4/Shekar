using Shekar.Domain.Enums;

namespace Shekar.Domain.Entities;

/// <summary>پلن اشتراک. R2 — فقط پرداخت ماهانه.</summary>
public class Plan
{
    public int Id { get; set; }

    public PlanType Type { get; set; }

    /// <summary>نام نمایشی، مثل «شکارچی» یا «شکارچی حرفه‌ای».</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>قیمت ماهانه به تومان.</summary>
    public long MonthlyPriceToman { get; set; }

    /// <summary>ویژگی‌های نمایشی پلن (متن‌ها عیناً از نمونه).</summary>
    public IReadOnlyList<string> Features { get; set; } = Array.Empty<string>();

    /// <summary>R6 — آیا پلن قابلیت «خبرم کن» دارد (فقط پلن حرفه‌ای).</summary>
    public bool HasBellAlerts => Type == PlanType.HunterPro;
}
