using Shekar.Application.Abstractions;

namespace Shekar.Application.Tests;

/// <summary>ساعت ثابت برای تست؛ تهران = UTC+3:30.</summary>
public sealed class TestClock : IClock
{
    public TestClock(DateTimeOffset utcNow)
    {
        UtcNow = utcNow;
    }

    public DateTimeOffset UtcNow { get; }

    public DateTimeOffset TehranNow => UtcNow.ToOffset(TimeSpan.FromHours(3.5));
}
