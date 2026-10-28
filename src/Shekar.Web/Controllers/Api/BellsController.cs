using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/bells")]
[Produces("application/json")]
public sealed class BellsController : ControllerBase
{
    private readonly IBellService _bells;

    public BellsController(IBellService bells) => _bells = bells;

    /// <summary>وضعیت زنگوله‌ی همه‌ی دسته‌ها.</summary>
    [HttpGet]
    public Task<IReadOnlyList<BellToggleResultDto>> GetAll(CancellationToken ct) => _bells.ListAsync(ct);

    /// <summary>روشن/خاموش‌کردن زنگوله‌ی یک دسته.</summary>
    [HttpPost("toggle")]
    public async Task<ActionResult<BellToggleResultDto>> Toggle(
        [FromBody] BellToggleRequestDto request, CancellationToken ct)
    {
        BellToggleResultDto result = await _bells.ToggleAsync(request, ct);
        return Ok(result);
    }
}
