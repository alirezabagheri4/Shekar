using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/waitlist")]
[Produces("application/json")]
public sealed class WaitlistController : ControllerBase
{
    private readonly IWaitlistService _waitlist;

    public WaitlistController(IWaitlistService waitlist) => _waitlist = waitlist;

    /// <summary>ثبت در لیست انتظار شهرِ پرظرفیت.</summary>
    [HttpPost]
    public async Task<ActionResult<WaitlistPositionDto>> Join(
        [FromBody] WaitlistJoinRequestDto request, CancellationToken ct)
    {
        WaitlistPositionDto result = await _waitlist.JoinAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
