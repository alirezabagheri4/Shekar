using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Domain;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;

namespace Shekar.Application.Services;

/// <summary>زنگوله‌ی «خبرم کن» (R6): فقط پلن حرفه‌ای؛ ۲۴ ساعت فعال، سپس خاموشِ خودکار.</summary>
public sealed class BellService : IBellService
{
    private readonly IBellAlertRepository _bells;
    private readonly ISubscriptionService _subscriptions;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    public BellService(
        IBellAlertRepository bells,
        ISubscriptionService subscriptions,
        ICurrentUser currentUser,
        IClock clock)
    {
        _bells = bells;
        _subscriptions = subscriptions;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<IReadOnlyList<BellToggleResultDto>> ListAsync(CancellationToken ct = default)
    {
        int userId = _currentUser.RequireUserId();
        DateTimeOffset now = _clock.UtcNow;
        IReadOnlyList<BellAlert> stored = await _bells.ListForUserAsync(userId, ct);
        Dictionary<PriceCategory, BellAlert> byCategory = stored.ToDictionary(b => b.Category);

        return DomainConstants.PriceCategoryBands
            .Select(band => Map(band.Category, byCategory.GetValueOrDefault(band.Category), now))
            .ToList();
    }

    public async Task<BellToggleResultDto> ToggleAsync(BellToggleRequestDto request, CancellationToken ct = default)
    {
        int userId = _currentUser.RequireUserId();
        SubscriptionStatusDto status = await _subscriptions.GetStatusForUserAsync(userId, ct);

        // R6 — «خبرم کن» فقط در پلن حرفه‌ای؛ پلن پایه مودال ارتقا را می‌بیند.
        if (!status.IsSubscriber || status.PlanType != PlanType.HunterPro)
        {
            throw new ForbiddenException(
                ErrorCodes.BellProOnly,
                "زنگوله‌ی «خبرم کن» فقط تو پلن حرفه‌ایه.");
        }

        DateTimeOffset now = _clock.UtcNow;
        BellAlert? existing = await _bells.GetAsync(userId, request.Category, ct);
        bool currentlyOn = existing?.ExpiresAt is { } exp && exp > now;

        BellAlert alert = existing ?? new BellAlert { UserId = userId, Category = request.Category };
        if (currentlyOn)
        {
            // خاموش‌کردن دستی.
            alert.ActivatedAt = null;
            alert.ExpiresAt = null;
        }
        else
        {
            // روشن‌کردن: ۲۴ ساعت فعال.
            alert.ActivatedAt = now;
            alert.ExpiresAt = now + DomainConstants.BellAlertDuration;
        }

        await _bells.UpsertAsync(alert, ct);
        return Map(request.Category, alert, now);
    }

    private static BellToggleResultDto Map(PriceCategory category, BellAlert? alert, DateTimeOffset now)
    {
        bool on = alert?.ExpiresAt is { } exp && exp > now;
        string? remaining = null;
        if (on)
        {
            int hours = (int)Math.Ceiling((alert!.ExpiresAt!.Value - now).TotalHours);
            remaining = $"{PersianFormat.ToPersianDigits(hours.ToString())} ساعت مونده";
        }

        return new BellToggleResultDto
        {
            Category = category,
            IsOn = on,
            ExpiresAt = on ? alert!.ExpiresAt : null,
            RemainingLabel = remaining
        };
    }
}
