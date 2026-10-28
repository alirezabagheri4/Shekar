using Shekar.Application.Abstractions;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;

namespace Shekar.Mock.Repositories;

public sealed class MockCityRepository : ICityRepository
{
    private readonly MockDataStore _store;
    public MockCityRepository(MockDataStore store) => _store = store;

    public Task<City?> GetAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_store.Cities.FirstOrDefault(c => c.Id == id));

    public Task<City?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        Task.FromResult(_store.Cities.FirstOrDefault(c => c.Slug == slug));

    public Task<IReadOnlyList<City>> ListAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<City>>(_store.Cities.ToList());
}

public sealed class MockPlanRepository : IPlanRepository
{
    private readonly MockDataStore _store;
    public MockPlanRepository(MockDataStore store) => _store = store;

    public Task<IReadOnlyList<Plan>> ListAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Plan>>(_store.Plans.ToList());

    public Task<Plan?> GetAsync(PlanType type, CancellationToken ct = default) =>
        Task.FromResult(_store.Plans.FirstOrDefault(p => p.Type == type));
}

public sealed class MockUserRepository : IUserRepository
{
    private readonly MockDataStore _store;
    public MockUserRepository(MockDataStore store) => _store = store;

    public Task<User?> GetAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_store.Users.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByMobileAsync(string mobile, CancellationToken ct = default) =>
        Task.FromResult(_store.Users.FirstOrDefault(u => u.Mobile == mobile));

    public Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        lock (_store.Gate)
        {
            user.Id = _store.NextId(_store.Users, u => u.Id);
            _store.Users.Add(user);
        }
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        // موجودیت‌ها مرجع‌اند؛ تغییرات روی نمونه‌ی درون لیست اعمال شده است.
        return Task.CompletedTask;
    }
}
