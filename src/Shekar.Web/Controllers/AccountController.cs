using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Abstractions;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Web.Models;

namespace Shekar.Web.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ISubscriptionService _subscriptions;
    private readonly IBellService _bells;
    private readonly IAppraisalService _appraisals;
    private readonly IHuntService _hunts;
    private readonly IUserRepository _users;
    private readonly ICityRepository _cities;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly IJalaliCalendar _jalali;

    public AccountController(
        ISubscriptionService subscriptions, IBellService bells, IAppraisalService appraisals,
        IHuntService hunts, IUserRepository users, ICityRepository cities,
        ICurrentUser currentUser, IClock clock, IJalaliCalendar jalali)
    {
        _subscriptions = subscriptions;
        _bells = bells;
        _appraisals = appraisals;
        _hunts = hunts;
        _users = users;
        _cities = cities;
        _currentUser = currentUser;
        _clock = clock;
        _jalali = jalali;
    }

    [HttpGet("/account")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        int userId = _currentUser.RequireUserId();
        var user = await _users.GetAsync(userId, ct);
        SubscriptionStatusDto status = await _subscriptions.GetStatusAsync(ct);
        IReadOnlyList<BellToggleResultDto> bells = await _bells.ListAsync(ct);
        IReadOnlyList<AppraisalHistoryItemDto> appraisals = await _appraisals.GetHistoryAsync(ct);
        IReadOnlyList<CategorySummaryDto> categories = await _hunts.GetCategoriesAsync(ct);

        string cityName = user?.CityId is { } cid
            ? (await _cities.GetAsync(cid, ct))?.Name ?? string.Empty
            : string.Empty;

        string expiresLabel = string.Empty;
        int daysRemaining = 0;
        if (status.ExpiresAt is { } exp)
        {
            expiresLabel = _jalali.FormatDate(exp.ToOffset(_clock.TehranNow.Offset));
            daysRemaining = Math.Max(0, (int)Math.Ceiling((exp - _clock.UtcNow).TotalDays));
        }

        var vm = new AccountViewModel(
            status,
            user?.FullName ?? string.Empty,
            user?.Mobile ?? string.Empty,
            cityName,
            expiresLabel,
            daysRemaining,
            bells,
            appraisals,
            categories);

        return View(vm);
    }
}
