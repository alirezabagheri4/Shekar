using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shekar.Web.Models;

namespace Shekar.Web.Controllers;

[Authorize]
public class PayController : Controller
{
    [HttpGet("/pay")]
    public IActionResult Index() => View();

    [HttpGet("/pay/success")]
    public IActionResult Success()
    {
        var vm = new PaySuccessViewModel(
            PlanName: TempData["PayPlan"] as string ?? "شکارچی حرفه‌ای",
            Amount: TempData["PayAmount"] as string ?? string.Empty,
            ExpiresLabel: TempData["PayExpires"] as string ?? string.Empty,
            TrackingCode: TempData["PayTracking"] as string ?? string.Empty,
            IsTrial: (TempData["PayTrial"] as string) == "1");
        return View(vm);
    }
}
