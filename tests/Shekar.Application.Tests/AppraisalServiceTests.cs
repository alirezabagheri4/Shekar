using Moq;
using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Domain;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;
using Xunit;

namespace Shekar.Application.Tests;

public class AppraisalServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 28, 6, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset MonthStart = new(2026, 10, 23, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset NextMonthStart = new(2026, 11, 22, 0, 0, 0, TimeSpan.Zero);
    private const int UserId = 7;

    private readonly Mock<IAppraisalRepository> _appraisals = new();
    private readonly Mock<IAppraisalEngine> _engine = new();
    private readonly Mock<IAdLinkValidator> _link = new();
    private readonly Mock<ISubscriptionService> _subscriptions = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<IJalaliCalendar> _jalali = new();

    public AppraisalServiceTests()
    {
        _currentUser.Setup(c => c.RequireUserId()).Returns(UserId);
        _link.Setup(l => l.IsValidCarAdUrl(It.IsAny<string>())).Returns(true);
        _jalali.Setup(j => j.GetMonthRangeUtc(It.IsAny<DateTimeOffset>())).Returns((MonthStart, NextMonthStart));
        _jalali.Setup(j => j.GetMonth(It.IsAny<DateTimeOffset>())).Returns(8);
        _jalali.Setup(j => j.GetMonthName(9)).Returns("آذر");
        _engine.Setup(e => e.Analyze(It.IsAny<string>())).Returns(SampleResult());
    }

    private AppraisalService Build() => new(
        _appraisals.Object, _engine.Object, _link.Object, _subscriptions.Object,
        _currentUser.Object, new TestClock(Now), _jalali.Object);

    private void SetSubscriber(bool isSubscriber) =>
        _subscriptions.Setup(s => s.GetStatusForUserAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionStatusDto { IsSubscriber = isSubscriber });

    private void SetUsedThisMonth(int used) =>
        _appraisals.Setup(a => a.CountForUserInRangeAsync(UserId, MonthStart, NextMonthStart, It.IsAny<CancellationToken>()))
            .ReturnsAsync(used);

    [Fact]
    public async Task Appraise_InvalidLink_ThrowsValidation()
    {
        // R5 — لینک نامعتبر.
        _link.Setup(l => l.IsValidCarAdUrl(It.IsAny<string>())).Returns(false);
        SetSubscriber(false);

        var ex = await Assert.ThrowsAsync<AppValidationException>(() =>
            Build().AppraiseAsync(new AppraisalRequestDto { AdUrl = "not-a-link" }));

        Assert.Equal("لینک نامعتبر است!", ex.Message);
    }

    [Fact]
    public async Task Appraise_FourthFreeThisMonth_ThrowsForbidden()
    {
        // R5 — سهمیه‌ی ۳تایی تمام؛ چهارمی ممنوع.
        SetSubscriber(false);
        SetUsedThisMonth(DomainConstants.FreeAppraisalsPerMonth);

        var ex = await Assert.ThrowsAsync<ForbiddenException>(() =>
            Build().AppraiseAsync(new AppraisalRequestDto { AdUrl = "https://example.com/ad/1" }));

        Assert.Equal(ErrorCodes.AppraisalQuotaExhausted, ex.Code);
        _appraisals.Verify(a => a.AddAsync(It.IsAny<Appraisal>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Appraise_ThirdFreeThisMonth_Succeeds()
    {
        // R5 — تا سومی مجاز (۲ استفاده‌شده ⇒ سومی).
        SetSubscriber(false);
        SetUsedThisMonth(2);
        _appraisals.Setup(a => a.AddAsync(It.IsAny<Appraisal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appraisal a, CancellationToken _) => a);

        AppraisalResultDto result = await Build()
            .AppraiseAsync(new AppraisalRequestDto { AdUrl = "https://example.com/ad/1" });

        Assert.Equal(87, result.Detail.Card.Score);
        _appraisals.Verify(a => a.AddAsync(It.IsAny<Appraisal>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Appraise_Subscriber_UnlimitedEvenPastQuota()
    {
        // R5 — مشترک نامحدود؛ حتی با ۱۰ کارشناسی در ماه مجاز است.
        SetSubscriber(true);
        SetUsedThisMonth(10);
        _appraisals.Setup(a => a.AddAsync(It.IsAny<Appraisal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appraisal a, CancellationToken _) => a);

        AppraisalResultDto result = await Build()
            .AppraiseAsync(new AppraisalRequestDto { AdUrl = "https://example.com/ad/1" });

        Assert.NotNull(result);
        _appraisals.Verify(a => a.AddAsync(It.IsAny<Appraisal>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static AppraisalResultDto SampleResult() => new()
    {
        Verdict = "فرصت طلایی",
        Detail = new HuntDetailDto
        {
            Card = new HuntCardDto { Title = "پژو ۲۰۷ پانوراما ۱۴۰۲", Score = 87, IsGolden = true }
        }
    };
}
