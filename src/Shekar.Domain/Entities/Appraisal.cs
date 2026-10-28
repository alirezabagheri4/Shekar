namespace Shekar.Domain.Entities;

/// <summary>یک کارشناسی قیمت که کاربر با دادن لینک آگهی انجام داده است.</summary>
public class Appraisal
{
    public int Id { get; set; }

    public int UserId { get; set; }

    /// <summary>لینک آگهی خودرو که کارشناسی شده.</summary>
    public string AdUrl { get; set; } = string.Empty;

    /// <summary>عنوان خودرو، مثل «پژو ۲۰۷ پانوراما ۱۴۰۲».</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>امتیاز نتیجه، ۰ تا ۱۰۰.</summary>
    public int Score { get; set; }

    /// <summary>برچسب نتیجه، مثل «فرصت طلایی»، «منصفانه»، «گران».</summary>
    public string Verdict { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}
