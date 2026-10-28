using System.ComponentModel.DataAnnotations;

namespace Shekar.Application.Contracts;

/// <summary>مرحله ۱ ورود: ارسال کد به شماره موبایل.</summary>
public sealed class RequestOtpDto
{
    [Required(ErrorMessage = "شماره موبایل لازمه.")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل معتبر نیست.")]
    public string Mobile { get; set; } = string.Empty;
}

/// <summary>مرحله ۲ ورود: تأیید کد ۵ رقمی.</summary>
public sealed class VerifyOtpDto
{
    [Required(ErrorMessage = "شماره موبایل لازمه.")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل معتبر نیست.")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "کد رو وارد کن.")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "کد ۵ رقمیه.")]
    public string Code { get; set; } = string.Empty;
}

/// <summary>مرحله ۳ ورود (فقط ورود اول): تکمیل پروفایل.</summary>
public sealed class CompleteProfileDto
{
    [Required(ErrorMessage = "نام لازمه.")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام خانوادگی لازمه.")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "شهر رو انتخاب کن.")]
    public int CityId { get; set; }
}

/// <summary>نتیجه‌ی تأیید کد.</summary>
public sealed record AuthResultDto
{
    public int UserId { get; init; }

    /// <summary>آیا باید مرحله‌ی تکمیل پروفایل نمایش داده شود (ورود اول).</summary>
    public bool RequiresProfile { get; init; }
}
