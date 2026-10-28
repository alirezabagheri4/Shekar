using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/subscriptions")]
[Produces("application/json")]
public sealed class SubscriptionsApiController : ControllerBase
{
    private readonly ISubscriptionService _subscriptions;

    public SubscriptionsApiController(ISubscriptionService subscriptions) => _subscriptions = subscriptions;

    /// <summary>وضعیت اشتراک کاربر جاری.</summary>
    [HttpGet("me")]
    public Task<SubscriptionStatusDto> GetMine(CancellationToken ct) => _subscriptions.GetStatusAsync(ct);

    /// <summary>شروع ۲۴ ساعت اشتراک آزمایشی.</summary>
    [HttpPost("trial")]
    public async Task<ActionResult<PurchaseResultDto>> StartTrial(
        [FromBody] StartSubscriptionRequestDto request, CancellationToken ct)
    {
        PurchaseResultDto result = await _subscriptions.StartTrialAsync(request, ct);
        return Ok(result);
    }

    /// <summary>خرید اشتراک ماهانه.</summary>
    [HttpPost("purchase")]
    public async Task<ActionResult<PurchaseResultDto>> Purchase(
        [FromBody] StartSubscriptionRequestDto request, CancellationToken ct)
    {
        PurchaseResultDto result = await _subscriptions.PurchaseAsync(request, ct);
        return Ok(result);
    }
}
