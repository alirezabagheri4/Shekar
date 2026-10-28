namespace Shekar.Application.Abstractions;

/// <summary>کاربر جاری درخواست. لایه‌ی Web آن را از Cookie/Claims پر می‌کند.</summary>
public interface ICurrentUser
{
    /// <summary>شناسه‌ی کاربر لاگین‌شده؛ <c>null</c> یعنی مهمان.</summary>
    int? UserId { get; }

    /// <summary>آیا کاربر وارد شده است.</summary>
    bool IsAuthenticated { get; }

    /// <summary>شناسه‌ی کاربر لاگین‌شده یا پرتاب <see cref="Common.NotAuthenticatedException"/>.</summary>
    int RequireUserId();
}
