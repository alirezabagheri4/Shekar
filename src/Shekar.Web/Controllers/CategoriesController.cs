using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Web.Infrastructure;
using Shekar.Web.Models;

namespace Shekar.Web.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly IHuntService _hunts;
    private readonly ISubscriptionService _subscriptions;
    private readonly IDeviceContext _device;

    public CategoriesController(IHuntService hunts, ISubscriptionService subscriptions, IDeviceContext device)
    {
        _hunts = hunts;
        _subscriptions = subscriptions;
        _device = device;
    }

    [HttpGet("/categories")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // صفحه‌ی دسته‌ها فقط موبایل است؛ روی دسکتاپ به /hunts هدایت شود.
        if (!_device.IsMobile)
        {
            return Redirect("/hunts");
        }

        IReadOnlyList<CategorySummaryDto> categories = await _hunts.GetCategoriesAsync(ct);
        SubscriptionStatusDto status = await _subscriptions.GetStatusAsync(ct);
        int total = categories.Sum(c => c.TodayCount);

        return View(new CategoriesViewModel(categories, total, status));
    }
}
