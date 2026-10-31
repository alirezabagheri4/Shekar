namespace Shekar.Domain.Enums;

/// <summary>وضعیت بدنه‌ی خودرو (یکی از ابعاد مهم قیمت‌گذاری/سگمنت‌بندی).</summary>
public enum BodyCondition
{
    /// <summary>نامشخص (در آگهی ذکر نشده).</summary>
    Unknown = 0,

    /// <summary>سالم و بی‌رنگ.</summary>
    Healthy = 1,

    /// <summary>رنگ‌شدگی جزئی.</summary>
    Painted = 2,

    /// <summary>تعویض قطعه‌ی بدنه.</summary>
    Replaced = 3,

    /// <summary>تصادفی / دور رنگ.</summary>
    Accident = 4
}
