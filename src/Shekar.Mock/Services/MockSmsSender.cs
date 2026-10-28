using System.Collections.Concurrent;
using Shekar.Application.Abstractions;

namespace Shekar.Mock.Services;

/// <summary>ارسال‌کننده‌ی پیامک ماک: فقط در کنسول لاگ می‌شود و در حافظه نگه داشته می‌شود.</summary>
public sealed class MockSmsSender : ISmsSender
{
    /// <summary>پیامک‌های ارسال‌شده (برای دیباگ/نمایش).</summary>
    public ConcurrentQueue<(string Mobile, string Message, DateTimeOffset At)> Sent { get; } = new();

    public Task SendAsync(string mobile, string message, CancellationToken cancellationToken = default)
    {
        Sent.Enqueue((mobile, message, DateTimeOffset.UtcNow));
        Console.WriteLine($"[SMS → {mobile}] {message}");
        return Task.CompletedTask;
    }
}
