using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Domain;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;

namespace Shekar.Application.Services;

/// <summary>اشتراک‌ها: وضعیت، شروع آزمایشی (R3)، خرید (R2) — همگی با ظرفیت شهر (R1).</summary>
public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IUserRepository _users;
    private readonly ICityRepository _cities;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly IJalaliCalendar _jalali;

    public SubscriptionService(
        ISubscriptionRepository subscriptions,
        IUserRepository users,
        ICityRepository cities,
        ICurrentUser currentUser,
        IClock clock,
        IJalaliCalendar jalali)
    {
        _subscriptions = subscriptions;
        _users = users;
        _cities = cities;
        _currentUser = currentUser;
        _clock = clock;
        _jalali = jalali;
    }

    public Task<SubscriptionStatusDto> GetStatusAsync(CancellationToken ct = default) =>
        GetStatusForUserAsync(_currentUser.RequireUserId(), ct);

    public async Task<SubscriptionStatusDto> GetStatusForUserAsync(int userId, CancellationToken ct = default)
    {
        User? user = await _users.GetAsync(userId, ct);
        Subscription? active = await _subscriptions.GetActiveForUserAsync(userId, _clock.UtcNow, ct);

        return new SubscriptionStatusDto
        {
            IsSubscriber = active is not null,
            PlanType = active?.PlanType,
            Status = active?.Status,
            ExpiresAt = active?.ExpiresAt,
            HasUsedTrial = user?.HasUsedTrial ?? false
        };
    }

    public async Task<PurchaseResultDto> StartTrialAsync(StartSubscriptionRequestDto request, CancellationToken ct = default)
    {
        int userId = _currentUser.RequireUserId();
        User user = await _users.GetAsync(userId, ct)
            ?? throw new NotAuthenticatedException();

        // R3 — اشتراک آزمایشی رایگان فقط یک بار برای هر کاربر.
        if (user.HasUsedTrial)
        {
            throw new ForbiddenException(
                ErrorCodes.TrialAlreadyUsed,
                "از اشتراک آزمایشی رایگانت قبلاً استفاده کردی.");
        }

        await EnsureCapacityAsync(request.CityId, ct);

        DateTimeOffset now = _clock.UtcNow;
        var subscription = await _subscriptions.AddAsync(new Subscription
        {
            UserId = userId,
            CityId = request.CityId,
            PlanType = request.PlanType,
            Status = SubscriptionStatus.Trial,
            StartsAt = now,
            ExpiresAt = now + DomainConstants.TrialDuration
        }, ct);

        user.HasUsedTrial = true;
        await _users.UpdateAsync(user, ct);

        return BuildResult(subscription);
    }

    public async Task<PurchaseResultDto> PurchaseAsync(StartSubscriptionRequestDto request, CancellationToken ct = default)
    {
        int userId = _currentUser.RequireUserId();

        await EnsureCapacityAsync(request.CityId, ct);

        DateTimeOffset now = _clock.UtcNow;
        // R2 — فقط پرداخت ماهانه.
        var subscription = await _subscriptions.AddAsync(new Subscription
        {
            UserId = userId,
            CityId = request.CityId,
            PlanType = request.PlanType,
            Status = SubscriptionStatus.Active,
            StartsAt = now,
            ExpiresAt = now.AddMonths(1)
        }, ct);

        return BuildResult(subscription);
    }

    /// <summary>R1 — اگر ظرفیت شهر تکمیل باشد، خرید/آزمایشی ممنوع است (۴۰۹).</summary>
    private async Task EnsureCapacityAsync(int cityId, CancellationToken ct)
    {
        City city = await _cities.GetAsync(cityId, ct)
            ?? throw new NotFoundException("شهر پیدا نشد.");

        int used = await _subscriptions.CountActiveInCityAsync(cityId, _clock.UtcNow, ct);
        if (used >= city.Capacity)
        {
            throw new ConflictException(
                ErrorCodes.CityCapacityFull,
                $"ظرفیت {city.Name} تکمیل شده؛ تو لیست انتظار ثبت‌نام کن.");
        }
    }

    private PurchaseResultDto BuildResult(Subscription subscription)
    {
        DateTimeOffset expiresTehran = ToTehran(subscription.ExpiresAt);
        return new PurchaseResultDto
        {
            SubscriptionId = subscription.Id,
            PlanType = subscription.PlanType,
            ExpiresAt = subscription.ExpiresAt,
            ExpiresLabel = _jalali.FormatDate(expiresTehran),
            TrackingCode = GenerateTrackingCode()
        };
    }

    private DateTimeOffset ToTehran(DateTimeOffset utc)
    {
        // نسبت TehranNow به UtcNow همان آفست منطقه است.
        TimeSpan offset = _clock.TehranNow.Offset;
        return utc.ToOffset(offset);
    }

    private static string GenerateTrackingCode() =>
        Random.Shared.Next(10_000_000, 99_999_999).ToString();
}
