using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Domain.Enums;

namespace Shekar.Web.Controllers;

[Authorize]
public class BellController : Controller
{
    private readonly IBellService _bells;

    public BellController(IBellService bells) => _bells = bells;

    /// <summary>R6 — روشن/خاموش‌کردن زنگوله و بازگشت به صفحه؛ پلن پایه → ارتقا.</summary>
    [HttpPost("/bells/toggle")]
    public async Task<IActionResult> Toggle(PriceCategory category, string? returnUrl, CancellationToken ct)
    {
        try
        {
            await _bells.ToggleAsync(new BellToggleRequestDto { Category = category }, ct);
        }
        catch (ForbiddenException)
        {
            return Redirect("/plans");
        }

        return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/hunts" : returnUrl);
    }
}
