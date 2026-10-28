namespace Shekar.Application.Common;

/// <summary>
/// پایه‌ی خطاهای کسب‌وکار. لایه‌ی Web هر نوع را به Status Code و ProblemDetails نگاشت می‌کند.
/// <see cref="Code"/> یک شناسه‌ی پایدار برای تشخیص حالت در UI است.
/// </summary>
public abstract class AppException : Exception
{
    protected AppException(string code, string message) : base(message)
    {
        Code = code;
    }

    /// <summary>شناسه‌ی ماشین‌خوان خطا، مثل «city_capacity_full».</summary>
    public string Code { get; }
}

/// <summary>کاربر لاگین‌نشده است → ۴۰۱.</summary>
public sealed class NotAuthenticatedException : AppException
{
    public NotAuthenticatedException(string message = "برای این کار باید وارد بشی.")
        : base("not_authenticated", message) { }
}

/// <summary>دسترسی ممنوع (قفل/سهمیه/فقط حرفه‌ای) → ۴۰۳.</summary>
public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string code, string message) : base(code, message) { }
}

/// <summary>منبع پیدا نشد یا حذف شده → ۴۰۴.</summary>
public sealed class NotFoundException : AppException
{
    public NotFoundException(string message = "پیدا نشد.")
        : base("not_found", message) { }
}

/// <summary>تعارض وضعیت (ظرفیت تکمیل) → ۴۰۹.</summary>
public sealed class ConflictException : AppException
{
    public ConflictException(string code, string message) : base(code, message) { }
}

/// <summary>ورودی نامعتبر که با DataAnnotations پوشش داده نشده → ۴۰۰.</summary>
public sealed class AppValidationException : AppException
{
    public AppValidationException(string message)
        : base("validation_error", message) { }
}
