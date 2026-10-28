using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Web.Models;

namespace Shekar.Web.Controllers;

public class PlansController : Controller
{
    private const int TehranCityId = 1;

    private readonly IPlanService _plans;
    private readonly ICapacityService _capacity;

    public PlansController(IPlanService plans, ICapacityService capacity)
    {
        _plans = plans;
        _capacity = capacity;
    }

    [HttpGet("/plans")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        IReadOnlyList<PlanDto> plans = await _plans.ListAsync(ct);
        CityCapacityDto capacity = await _capacity.GetAsync(TehranCityId, ct);
        var vm = new PlansViewModel(plans, capacity);

        // R1 — ظرفیت تکمیل: نمایش حالت «ظرفیت تهران تکمیل شد» و لیست انتظار.
        return View(capacity.IsFull ? "Full" : "Index", vm);
    }
}
