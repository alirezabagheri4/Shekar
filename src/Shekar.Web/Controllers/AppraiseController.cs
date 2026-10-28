using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Web.Models;

namespace Shekar.Web.Controllers;

[Authorize]
public class AppraiseController : Controller
{
    private readonly IAppraisalService _appraisals;

    public AppraiseController(IAppraisalService appraisals) => _appraisals = appraisals;

    [HttpGet("/appraise")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        AppraisalQuotaDto quota = await _appraisals.GetQuotaAsync(ct);
        var vm = new AppraiseViewModel(quota);

        // R5 — سهمیه‌ی رایگان تمام‌شده برای کاربر بدون اشتراک.
        bool exhausted = !quota.IsSubscriber && quota.Remaining == 0;
        return View(exhausted ? "QuotaExhausted" : "Index", vm);
    }

    // ثبت لینک و اجرای کارشناسی (R5)، سپس نمایش انیمیشن تحلیل.
    [HttpPost("/appraise")]
    public async Task<IActionResult> Submit(AppraisalRequestDto request, CancellationToken ct)
    {
        try
        {
            await _appraisals.AppraiseAsync(request, ct);
            return Redirect("/appraise/analyze");
        }
        catch (AppValidationException ex)
        {
            // R5 — لینک نامعتبر است.
            TempData["AppraiseError"] = ex.Message;
            return Redirect("/appraise");
        }
        catch (ForbiddenException)
        {
            // R5 — سهمیه تمام شد.
            return Redirect("/appraise");
        }
    }

    [HttpGet("/appraise/analyze")]
    public IActionResult Analyze() => View();

    [HttpGet("/appraise/result")]
    public IActionResult Result() => View();
}
