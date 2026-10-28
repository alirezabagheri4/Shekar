using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Domain.Entities;

namespace Shekar.Application.Services;

/// <summary>لیست انتظار شهرِ پرظرفیت (R1).</summary>
public sealed class WaitlistService : IWaitlistService
{
    private readonly IWaitlistRepository _waitlist;
    private readonly ICityRepository _cities;
    private readonly IClock _clock;

    public WaitlistService(IWaitlistRepository waitlist, ICityRepository cities, IClock clock)
    {
        _waitlist = waitlist;
        _cities = cities;
        _clock = clock;
    }

    public async Task<WaitlistPositionDto> JoinAsync(WaitlistJoinRequestDto request, CancellationToken ct = default)
    {
        City city = await _cities.GetAsync(request.CityId, ct)
            ?? throw new NotFoundException("شهر پیدا نشد.");

        // اگر قبلاً ثبت شده، همان نوبت را برگردان (idempotent).
        WaitlistEntry? existing = await _waitlist.GetAsync(request.CityId, request.Mobile, ct);
        if (existing is not null)
        {
            return BuildPosition(existing.Position, city.Name);
        }

        int count = await _waitlist.CountForCityAsync(request.CityId, ct);
        int position = count + 1;

        await _waitlist.AddAsync(new WaitlistEntry
        {
            CityId = request.CityId,
            Mobile = request.Mobile,
            CreatedAt = _clock.UtcNow,
            Position = position
        }, ct);

        return BuildPosition(position, city.Name);
    }

    private static WaitlistPositionDto BuildPosition(int position, string cityName) => new()
    {
        Position = position,
        CityName = cityName,
        Label = $"نفر {PersianFormat.ToPersianDigits(position.ToString())}ام صف {cityName}ی"
    };
}
