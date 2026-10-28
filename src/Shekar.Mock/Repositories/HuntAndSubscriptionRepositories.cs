using Shekar.Application.Abstractions;
using Shekar.Domain;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;
using Shekar.Mock.Services;

namespace Shekar.Mock.Repositories;

public sealed class MockSubscriptionRepository : ISubscriptionRepository
{
    private readonly MockDataStore _store;
    public MockSubscriptionRepository(MockDataStore store) => _store = store;

    public Task<Subscription?> GetActiveForUserAsync(int userId, DateTimeOffset now, CancellationToken ct = default) =>
        Task.FromResult(_store.Subscriptions
            .Where(s => s.UserId == userId && s.Status != SubscriptionStatus.Expired && s.ExpiresAt > now)
            .OrderByDescending(s => s.ExpiresAt)
            .FirstOrDefault());

    public Task<int> CountActiveInCityAsync(int cityId, DateTimeOffset now, CancellationToken ct = default) =>
        Task.FromResult(_store.Subscriptions.Count(s =>
            s.CityId == cityId
            && (s.Status == SubscriptionStatus.Trial || s.Status == SubscriptionStatus.Active)
            && s.ExpiresAt > now));

    public Task<Subscription> AddAsync(Subscription subscription, CancellationToken ct = default)
    {
        lock (_store.Gate)
        {
            subscription.Id = _store.NextId(_store.Subscriptions, s => s.Id);
            _store.Subscriptions.Add(subscription);
        }
        return Task.FromResult(subscription);
    }
}

public sealed class MockHuntRepository : IHuntRepository
{
    private readonly MockDataStore _store;
    private readonly IClock _clock;

    public MockHuntRepository(MockDataStore store, IClock clock)
    {
        _store = store;
        _clock = clock;
    }

    public Task<IReadOnlyList<Hunt>> QueryAsync(HuntQuery query, CancellationToken ct = default)
    {
        DateTime target = _clock.TehranNow.Date.AddDays(-(int)query.Day);

        IEnumerable<Hunt> result = _store.Hunts.Where(h =>
            h.CityId == query.CityId
            && TehranTime.ToTehran(h.PublishedAt).Date == target);

        if (query.Category is { } category)
        {
            result = result.Where(h => DomainConstants.ResolveCategory(h.PriceToman) == category);
        }

        return Task.FromResult<IReadOnlyList<Hunt>>(result.ToList());
    }

    public Task<IReadOnlyList<Hunt>> ListTodayAsync(int cityId, CancellationToken ct = default)
    {
        DateTime today = _clock.TehranNow.Date;
        return Task.FromResult<IReadOnlyList<Hunt>>(_store.Hunts
            .Where(h => h.CityId == cityId && TehranTime.ToTehran(h.PublishedAt).Date == today)
            .ToList());
    }

    public Task<Hunt?> GetAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_store.Hunts.FirstOrDefault(h => h.Id == id));
}
