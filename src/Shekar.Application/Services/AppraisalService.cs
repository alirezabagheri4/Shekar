using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Domain;
using Shekar.Domain.Entities;

namespace Shekar.Application.Services;

/// <summary>کارشناسی قیمت (R5): ورود لازم، سهمیه‌ی ماهانه‌ی شمسی، لینک معتبر، مشترک نامحدود.</summary>
public sealed class AppraisalService : IAppraisalService
{
    private readonly IAppraisalRepository _appraisals;
    private readonly IAppraisalEngine _engine;
    private readonly IAdLinkValidator _linkValidator;
    private readonly ISubscriptionService _subscriptions;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly IJalaliCalendar _jalali;

    public AppraisalService(
        IAppraisalRepository appraisals,
        IAppraisalEngine engine,
        IAdLinkValidator linkValidator,
        ISubscriptionService subscriptions,
        ICurrentUser currentUser,
        IClock clock,
        IJalaliCalendar jalali)
    {
        _appraisals = appraisals;
        _engine = engine;
        _linkValidator = linkValidator;
        _subscriptions = subscriptions;
        _currentUser = currentUser;
        _clock = clock;
        _jalali = jalali;
    }

    public async Task<AppraisalQuotaDto> GetQuotaAsync(CancellationToken ct = default)
    {
        int userId = _currentUser.RequireUserId();
        SubscriptionStatusDto status = await _subscriptions.GetStatusForUserAsync(userId, ct);
        int used = await CountThisMonthAsync(userId, ct);

        int remaining = Math.Max(0, DomainConstants.FreeAppraisalsPerMonth - used);
        return new AppraisalQuotaDto
        {
            IsSubscriber = status.IsSubscriber,
            Limit = DomainConstants.FreeAppraisalsPerMonth,
            Used = used,
            Remaining = remaining,
            NextResetLabel = BuildNextResetLabel()
        };
    }

    public async Task<AppraisalResultDto> AppraiseAsync(AppraisalRequestDto request, CancellationToken ct = default)
    {
        int userId = _currentUser.RequireUserId();

        // R5 — لینک باید URL معتبر آگهی خودرو باشد.
        if (!_linkValidator.IsValidCarAdUrl(request.AdUrl))
        {
            throw new AppValidationException("لینک نامعتبر است!");
        }

        SubscriptionStatusDto status = await _subscriptions.GetStatusForUserAsync(userId, ct);

        // R5 — کاربر بدون اشتراک ماهی ۳ کارشناسی رایگان؛ چهارمی ممنوع.
        if (!status.IsSubscriber)
        {
            int used = await CountThisMonthAsync(userId, ct);
            if (used >= DomainConstants.FreeAppraisalsPerMonth)
            {
                throw new ForbiddenException(
                    ErrorCodes.AppraisalQuotaExhausted,
                    "سهمیه‌ی رایگان این ماه تموم شد");
            }
        }

        AppraisalResultDto result = _engine.Analyze(request.AdUrl);

        await _appraisals.AddAsync(new Appraisal
        {
            UserId = userId,
            AdUrl = request.AdUrl,
            Title = result.Detail.Card.Title,
            Score = result.Detail.Card.Score,
            Verdict = result.Verdict,
            CreatedAt = _clock.UtcNow
        }, ct);

        return result;
    }

    public async Task<IReadOnlyList<AppraisalHistoryItemDto>> GetHistoryAsync(CancellationToken ct = default)
    {
        int userId = _currentUser.RequireUserId();
        IReadOnlyList<Appraisal> items = await _appraisals.ListForUserAsync(userId, ct);

        return items
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AppraisalHistoryItemDto
            {
                Title = a.Title,
                Score = a.Score,
                Verdict = a.Verdict,
                DateLabel = _jalali.FormatDate(ToTehran(a.CreatedAt)),
                IsGolden = HuntCalculations.IsGolden(a.Score)
            })
            .ToList();
    }

    private async Task<int> CountThisMonthAsync(int userId, CancellationToken ct)
    {
        (DateTimeOffset startUtc, DateTimeOffset nextStartUtc) = _jalali.GetMonthRangeUtc(_clock.TehranNow);
        return await _appraisals.CountForUserInRangeAsync(userId, startUtc, nextStartUtc, ct);
    }

    private string BuildNextResetLabel()
    {
        int month = _jalali.GetMonth(_clock.TehranNow);
        int next = month == 12 ? 1 : month + 1;
        return $"اول {_jalali.GetMonthName(next)}";
    }

    private DateTimeOffset ToTehran(DateTimeOffset utc) => utc.ToOffset(_clock.TehranNow.Offset);
}
