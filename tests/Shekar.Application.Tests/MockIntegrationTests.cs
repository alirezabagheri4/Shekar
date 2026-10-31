using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Services;
using Shekar.Domain.Enums;
using Shekar.Mock;
using Shekar.Mock.Repositories;
using Shekar.Mock.Services;
using Xunit;

namespace Shekar.Application.Tests;

/// <summary>تست یکپارچه‌ی سرویس‌های Application روی مخزن و زیرساخت Mock.</summary>
public class MockIntegrationTests
{
    private readonly MockDataStore _store;
    private readonly IClock _clock = new TehranClock();
    private readonly PersianJalaliCalendar _jalali = new();

    public MockIntegrationTests()
    {
        _store = new MockDataStore(_clock);
    }

    private HuntService BuildHuntService(int? userId)
    {
        var currentUser = new StubCurrentUser(userId);
        var subs = new SubscriptionService(
            new MockSubscriptionRepository(_store), new MockUserRepository(_store),
            new MockCityRepository(_store), currentUser, _clock, _jalali);
        return new HuntService(
            new MockHuntRepository(_store, _clock), new MockBellAlertRepository(_store),
            subs, currentUser, _clock);
    }

    [Fact]
    public async Task Subscriber_SeesUnlockedListWith14ActiveHunts()
    {
        // کامران (مشترک حرفه‌ای) → قفل نیست، ۱۴ شکار فعال (۱۵ منهای ۱ حذف‌شده).
        var service = BuildHuntService(MockDataStore.DemoUserId);
        var list = await service.GetListAsync(null, HuntDay.Today, HuntSortOrder.TopScore);

        Assert.False(list.IsLocked);
        Assert.Equal(14, list.ActiveCount);
        Assert.Equal(15, list.Cards.Count);
        Assert.Null(list.LockedBanner);
        // مرتب‌سازی بیشترین امتیاز: اولین کارت فعال امتیاز ۸۷.
        Assert.Equal(87, list.Cards[0].Score);
    }

    [Fact]
    public async Task NonSubscriber_SeesLockedListWithBanner()
    {
        // کاربر جدید بدون اشتراک → قفل + بنر آمار.
        var currentUser = new StubCurrentUser(999);
        _store.Users.Add(new Domain.Entities.User { Id = 999, Mobile = "09120000000", ProfileCompleted = true });

        var service = BuildHuntService(999);
        var list = await service.GetListAsync(null, HuntDay.Today, HuntSortOrder.TopScore);

        Assert.True(list.IsLocked);
        Assert.NotNull(list.LockedBanner);
        Assert.Equal(14, list.LockedBanner!.TodayHuntCount);
        Assert.Equal(2, list.LockedBanner.GoldenCount); // دو شکار ۸۷ → فرصت طلایی
    }

    [Fact]
    public async Task RemovedHunt_DetailThrowsNotFound_ForSubscriber()
    {
        var service = BuildHuntService(MockDataStore.DemoUserId);
        int removedId = _store.Hunts.First(h => h.State == HuntState.Removed).Id;

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetDetailAsync(removedId));
    }

    [Fact]
    public async Task Guest_ListThrowsNotAuthenticated()
    {
        var service = BuildHuntService(null);
        await Assert.ThrowsAsync<NotAuthenticatedException>(() =>
            service.GetListAsync(null, HuntDay.Today, HuntSortOrder.TopScore));
    }

    [Fact]
    public async Task Categories_SumToFifteenToday()
    {
        var service = BuildHuntService(MockDataStore.DemoUserId);
        var categories = await service.GetCategoriesAsync();

        Assert.Equal(8, categories.Count);
        Assert.Equal(15, categories.Sum(c => c.TodayCount));
        // زنگوله‌ی ۱–۱٫۵ب روشن است (داده‌ی حساب نمونه).
        Assert.True(categories.Single(c => c.Category == PriceCategory.From1To1_5B).BellOn);
    }

    [Fact]
    public async Task BlockedAdvertiser_ExcludedFromDailyHunts()
    {
        // یک شکار فعال با شماره‌ی آگهی‌دهنده برمی‌داریم و آن شماره را بلاک می‌کنیم.
        var hunt = _store.Hunts.First(h => h.State == HuntState.Active && h.AdvertiserPhone != null);
        var repo = new MockHuntRepository(_store, _clock);
        var before = await repo.QueryAsync(new Shekar.Application.Abstractions.HuntQuery(1, null, HuntDay.Today, HuntSortOrder.TopScore));
        Assert.Contains(before, h => h.Id == hunt.Id);

        _store.BlockedAdvertisers.Add(new Shekar.Domain.Entities.BlockedAdvertiser
        {
            Id = 99, Phone = hunt.AdvertiserPhone!, Reason = Shekar.Domain.Enums.BlockReason.TooManyAds,
            BlockedAt = _clock.UtcNow, IsActive = true
        });

        var after = await repo.QueryAsync(new Shekar.Application.Abstractions.HuntQuery(1, null, HuntDay.Today, HuntSortOrder.TopScore));
        Assert.DoesNotContain(after, h => h.Id == hunt.Id);
        Assert.Equal(before.Count - 1, after.Count);
    }

    [Fact]
    public void Jalali_MonthRangeContainsNow()
    {
        var (startUtc, nextUtc) = _jalali.GetMonthRangeUtc(_clock.TehranNow);
        Assert.True(startUtc <= _clock.UtcNow && _clock.UtcNow < nextUtc);
    }

    private sealed class StubCurrentUser : ICurrentUser
    {
        public StubCurrentUser(int? userId) => UserId = userId;
        public int? UserId { get; }
        public bool IsAuthenticated => UserId.HasValue;
        public int RequireUserId() => UserId ?? throw new NotAuthenticatedException();
    }
}
