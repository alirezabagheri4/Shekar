namespace Shekar.Application.Abstractions;

/// <summary>ارسال پیامک (کد OTP و اعلان شکار تازه). در Mock فقط لاگ می‌شود.</summary>
public interface ISmsSender
{
    Task SendAsync(string mobile, string message, CancellationToken cancellationToken = default);
}
