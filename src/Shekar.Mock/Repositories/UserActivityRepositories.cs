using Shekar.Application.Abstractions;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;

namespace Shekar.Mock.Repositories;

public sealed class MockAppraisalRepository : IAppraisalRepository
{
    private readonly MockDataStore _store;
    public MockAppraisalRepository(MockDataStore store) => _store = store;

    public Task<int> CountForUserInRangeAsync(int userId, DateTimeOffset fromUtc, DateTimeOffset toUtc, CancellationToken ct = default) =>
        Task.FromResult(_store.Appraisals.Count(a =>
            a.UserId == userId && a.CreatedAt >= fromUtc && a.CreatedAt < toUtc));

    public Task<IReadOnlyList<Appraisal>> ListForUserAsync(int userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Appraisal>>(_store.Appraisals.Where(a => a.UserId == userId).ToList());

    public Task<Appraisal> AddAsync(Appraisal appraisal, CancellationToken ct = default)
    {
        lock (_store.Gate)
        {
            appraisal.Id = _store.NextId(_store.Appraisals, a => a.Id);
            _store.Appraisals.Add(appraisal);
        }
        return Task.FromResult(appraisal);
    }
}

public sealed class MockBellAlertRepository : IBellAlertRepository
{
    private readonly MockDataStore _store;
    public MockBellAlertRepository(MockDataStore store) => _store = store;

    public Task<IReadOnlyList<BellAlert>> ListForUserAsync(int userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<BellAlert>>(_store.BellAlerts.Where(b => b.UserId == userId).ToList());

    public Task<BellAlert?> GetAsync(int userId, PriceCategory category, CancellationToken ct = default) =>
        Task.FromResult(_store.BellAlerts.FirstOrDefault(b => b.UserId == userId && b.Category == category));

    public Task UpsertAsync(BellAlert alert, CancellationToken ct = default)
    {
        lock (_store.Gate)
        {
            if (alert.Id == 0)
            {
                alert.Id = _store.NextId(_store.BellAlerts, b => b.Id);
                _store.BellAlerts.Add(alert);
            }
            // در غیر این صورت همان نمونه‌ی مرجع به‌روز شده است.
        }
        return Task.CompletedTask;
    }
}

public sealed class MockWaitlistRepository : IWaitlistRepository
{
    private readonly MockDataStore _store;
    public MockWaitlistRepository(MockDataStore store) => _store = store;

    public Task<WaitlistEntry?> GetAsync(int cityId, string mobile, CancellationToken ct = default) =>
        Task.FromResult(_store.Waitlist.FirstOrDefault(w => w.CityId == cityId && w.Mobile == mobile));

    public Task<int> CountForCityAsync(int cityId, CancellationToken ct = default) =>
        Task.FromResult(_store.Waitlist.Count(w => w.CityId == cityId));

    public Task<WaitlistEntry> AddAsync(WaitlistEntry entry, CancellationToken ct = default)
    {
        lock (_store.Gate)
        {
            entry.Id = _store.NextId(_store.Waitlist, w => w.Id);
            _store.Waitlist.Add(entry);
        }
        return Task.FromResult(entry);
    }
}

public sealed class MockOtpRepository : IOtpRepository
{
    private readonly MockDataStore _store;
    public MockOtpRepository(MockDataStore store) => _store = store;

    public Task<OtpCode?> GetLatestAsync(string mobile, CancellationToken ct = default) =>
        Task.FromResult(_store.Otps.Where(o => o.Mobile == mobile).OrderByDescending(o => o.CreatedAt).FirstOrDefault());

    public Task AddAsync(OtpCode code, CancellationToken ct = default)
    {
        lock (_store.Gate)
        {
            code.Id = _store.NextId(_store.Otps, o => o.Id);
            _store.Otps.Add(code);
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(OtpCode code, CancellationToken ct = default) => Task.CompletedTask;
}
