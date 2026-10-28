using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Domain.Enums;
using Shekar.Web.Infrastructure;
using Shekar.Web.Models;

namespace Shekar.Web.Controllers;

[Authorize]
public class HuntsController : Controller
{
    private readonly IHuntService _hunts;
    private readonly ISubscriptionService _subscriptions;
    private readonly IDeviceContext _device;
    private readonly IClock _clock;

    public HuntsController(IHuntService hunts, ISubscriptionService subscriptions, IDeviceContext device, IClock clock)
    {
        _hunts = hunts;
        _subscriptions = subscriptions;
        _device = device;
        _clock = clock;
    }

    [HttpGet("/hunts")]
    public async Task<IActionResult> Index(
        PriceCategory? category, HuntDay day = HuntDay.Today, HuntSortOrder sort = HuntSortOrder.TopScore,
        CancellationToken ct = default)
    {
        // روی موبایل، لیست همیشه در بستر یک دسته دیده می‌شود؛ بدون دسته به صفحه‌ی دسته‌ها برگرد.
        if (_device.IsMobile && category is null)
        {
            return Redirect("/categories");
        }

        HuntListDto list = await _hunts.GetListAsync(category, day, sort, ct);
        IReadOnlyList<CategorySummaryDto> categories = await _hunts.GetCategoriesAsync(ct);
        SubscriptionStatusDto status = await _subscriptions.GetStatusAsync(ct);

        var vm = new HuntsViewModel(list, categories, status, _clock.TehranNow);
        return View(list.IsLocked ? "Locked" : "Index", vm);
    }

    [HttpGet("/hunts/{id:int}")]
    public async Task<IActionResult> Detail(int id, CancellationToken ct)
    {
        try
        {
            HuntDetailDto detail = await _hunts.GetDetailAsync(id, ct);
            return View(new HuntDetailViewModel(detail, _clock.TehranNow));
        }
        catch (ForbiddenException)
        {
            // R4 — قفل: به صفحه‌ی پلن‌ها هدایت شود.
            return Redirect("/plans");
        }
        catch (NotFoundException)
        {
            // R7 — آگهی حذف‌شده.
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("Removed");
        }
    }
}
