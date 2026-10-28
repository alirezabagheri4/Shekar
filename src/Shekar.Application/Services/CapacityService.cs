using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Domain.Entities;

namespace Shekar.Application.Services;

/// <summary>R1 — ظرفیت اشتراک فعال هر شهر (۱۰۰).</summary>
public sealed class CapacityService : ICapacityService
{
    private readonly ICityRepository _cities;
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IClock _clock;

    public CapacityService(ICityRepository cities, ISubscriptionRepository subscriptions, IClock clock)
    {
        _cities = cities;
        _subscriptions = subscriptions;
        _clock = clock;
    }

    public async Task<CityCapacityDto> GetAsync(int cityId, CancellationToken ct = default)
    {
        City city = await _cities.GetAsync(cityId, ct)
            ?? throw new NotFoundException("شهر پیدا نشد.");

        int used = await _subscriptions.CountActiveInCityAsync(cityId, _clock.UtcNow, ct);
        int remaining = Math.Max(0, city.Capacity - used);

        return new CityCapacityDto
        {
            CityId = city.Id,
            CityName = city.Name,
            Capacity = city.Capacity,
            Used = used,
            Remaining = remaining,
            IsFull = used >= city.Capacity
        };
    }
}
