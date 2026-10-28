using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/plans")]
[Produces("application/json")]
public sealed class PlansController : ControllerBase
{
    private readonly IPlanService _plans;

    public PlansController(IPlanService plans) => _plans = plans;

    /// <summary>فهرست پلن‌ها.</summary>
    [HttpGet]
    public Task<IReadOnlyList<PlanDto>> GetAll(CancellationToken ct) => _plans.ListAsync(ct);
}
