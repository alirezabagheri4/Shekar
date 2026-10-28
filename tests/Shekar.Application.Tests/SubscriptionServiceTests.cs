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

public class SubscriptionServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 28, 6, 0, 0, TimeSpan.Zero);
    private const int UserId = 7;
    private const int CityId = 1;

    private readonly Mock<ISubscriptionRepository> _subs = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ICityRepository> _cities = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<IJalaliCalendar> _jalali = new();

    public SubscriptionServiceTests()
    {
        _currentUser.Setup(c => c.RequireUserId()).Returns(UserId);
        _cities.Setup(c => c.GetAsync(CityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new City { Id = CityId, Name = "تهران", Capacity = DomainConstants.CityCapacity });
        _jalali.Setup(j => j.FormatDate(It.IsAny<DateTimeOffset>())).Returns("۳۰ آبان ۱۴۰۵");
    }

    private SubscriptionService Build() => new(
        _subs.Object, _users.Object, _cities.Object, _currentUser.Object,
        new TestClock(Now), _jalali.Object);

    [Fact]
    public async Task StartTrial_WhenCapacityFull_ThrowsConflict()
    {
        // R1 — ظرفیت پر (۱۰۰/۱۰۰).
        _users.Setup(u => u.GetAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = UserId, HasUsedTrial = false });
        _subs.Setup(s => s.CountActiveInCityAsync(CityId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DomainConstants.CityCapacity);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            Build().StartTrialAsync(new StartSubscriptionRequestDto { PlanType = PlanType.Hunter, CityId = CityId }));

        Assert.Equal(ErrorCodes.CityCapacityFull, ex.Code);
    }

    [Fact]
    public async Task StartTrial_WhenAlreadyUsed_ThrowsForbidden()
    {
        // R3 — اشتراک آزمایشی فقط یک بار.
        _users.Setup(u => u.GetAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = UserId, HasUsedTrial = true });

        var ex = await Assert.ThrowsAsync<ForbiddenException>(() =>
            Build().StartTrialAsync(new StartSubscriptionRequestDto { PlanType = PlanType.Hunter, CityId = CityId }));

        Assert.Equal(ErrorCodes.TrialAlreadyUsed, ex.Code);
        _subs.Verify(s => s.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartTrial_WhenEligible_CreatesTrialAndMarksUser()
    {
        var user = new User { Id = UserId, HasUsedTrial = false };
        _users.Setup(u => u.GetAsync(UserId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _subs.Setup(s => s.CountActiveInCityAsync(CityId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);
        _subs.Setup(s => s.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subscription s, CancellationToken _) => { s.Id = 99; return s; });

        PurchaseResultDto result = await Build()
            .StartTrialAsync(new StartSubscriptionRequestDto { PlanType = PlanType.HunterPro, CityId = CityId });

        Assert.Equal(99, result.SubscriptionId);
        Assert.True(user.HasUsedTrial);
        _subs.Verify(s => s.AddAsync(
            It.Is<Subscription>(x => x.Status == SubscriptionStatus.Trial
                && x.ExpiresAt == Now + DomainConstants.TrialDuration),
            It.IsAny<CancellationToken>()), Times.Once);
        _users.Verify(u => u.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Purchase_WhenCapacityFull_ThrowsConflict()
    {
        // R1 — خرید در ظرفیت پر ممنوع.
        _subs.Setup(s => s.CountActiveInCityAsync(CityId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DomainConstants.CityCapacity);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            Build().PurchaseAsync(new StartSubscriptionRequestDto { PlanType = PlanType.HunterPro, CityId = CityId }));

        Assert.Equal(ErrorCodes.CityCapacityFull, ex.Code);
    }

    [Fact]
    public async Task Purchase_SetsMonthlyActiveSubscription()
    {
        // R2 — فقط ماهانه: پایان اعتبار یک ماه بعد.
        _subs.Setup(s => s.CountActiveInCityAsync(CityId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);
        _subs.Setup(s => s.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subscription s, CancellationToken _) => { s.Id = 5; return s; });

        await Build().PurchaseAsync(new StartSubscriptionRequestDto { PlanType = PlanType.HunterPro, CityId = CityId });

        _subs.Verify(s => s.AddAsync(
            It.Is<Subscription>(x => x.Status == SubscriptionStatus.Active
                && x.ExpiresAt == Now.AddMonths(1)),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
