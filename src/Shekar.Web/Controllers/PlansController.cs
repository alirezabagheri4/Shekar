using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Domain.Enums;
using Shekar.Web.Models;

namespace Shekar.Web.Controllers;

public class PlansController : Controller
{
    private const int TehranCityId = 1;

    private readonly IPlanService _plans;
    private readonly ICapacityService _capacity;
    private readonly ISubscriptionService _subscriptions;
    private readonly ICurrentUser _currentUser;

    public PlansController(
        IPlanService plans, ICapacityService capacity,
        ISubscriptionService subscriptions, ICurrentUser currentUser)
    {
        _plans = plans;
        _capacity = capacity;
        _subscriptions = subscriptions;
        _currentUser = currentUser;
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

    // خرید اشتراک (ماک): پرداخت موفق فرض می‌شود و اشتراک فعال می‌شود.
    [HttpPost("/plans/subscribe")]
    public async Task<IActionResult> Subscribe(PlanType plan, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Redirect("/login?returnUrl=/plans");
        }

        try
        {
            PurchaseResultDto result = await _subscriptions.PurchaseAsync(
                new StartSubscriptionRequestDto { PlanType = plan, CityId = TehranCityId }, ct);
            await StoreReceiptAsync(plan, result, isTrial: false, ct);
            return Redirect("/pay/success");
        }
        catch (ConflictException)
        {
            // R1 — ظرفیت تکمیل.
            return Redirect("/plans");
        }
    }

    // R3 — ۲۴ ساعت رایگان، فقط برای پلن شکارچی.
    [HttpPost("/plans/trial")]
    public async Task<IActionResult> Trial(CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Redirect("/login?returnUrl=/plans");
        }

        try
        {
            PurchaseResultDto result = await _subscriptions.StartTrialAsync(
                new StartSubscriptionRequestDto { PlanType = PlanType.Hunter, CityId = TehranCityId }, ct);
            await StoreReceiptAsync(PlanType.Hunter, result, isTrial: true, ct);
            return Redirect("/pay/success");
        }
        catch (ForbiddenException ex)
        {
            TempData["PlansMsg"] = ex.Message;
            return Redirect("/plans");
        }
        catch (ConflictException)
        {
            return Redirect("/plans");
        }
    }

    private async Task StoreReceiptAsync(PlanType plan, PurchaseResultDto result, bool isTrial, CancellationToken ct)
    {
        IReadOnlyList<PlanDto> plans = await _plans.ListAsync(ct);
        PlanDto? p = plans.FirstOrDefault(x => x.Type == plan);

        TempData["PayPlan"] = plan == PlanType.HunterPro ? "شکارچی حرفه‌ای" : "شکارچی";
        TempData["PayAmount"] = isTrial
            ? "۲۴ ساعت رایگان"
            : $"{PersianFormat.ToPersianNumber(p?.MonthlyPriceToman ?? 0)} تومان";
        TempData["PayExpires"] = result.ExpiresLabel;
        TempData["PayTracking"] = PersianFormat.ToPersianDigits(result.TrackingCode);
        TempData["PayTrial"] = isTrial ? "1" : "0";
    }
}
