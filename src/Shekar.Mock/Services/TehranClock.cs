using Shekar.Application.Abstractions;

namespace Shekar.Mock.Services;

/// <summary>ساعت واقعی سیستم با زمان محلی Asia/Tehran.</summary>
public sealed class TehranClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public DateTimeOffset TehranNow => TehranTime.ToTehran(DateTimeOffset.UtcNow);
}
