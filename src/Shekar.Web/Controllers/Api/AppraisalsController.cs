using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/appraisals")]
[Produces("application/json")]
public sealed class AppraisalsController : ControllerBase
{
    private readonly IAppraisalService _appraisals;

    public AppraisalsController(IAppraisalService appraisals) => _appraisals = appraisals;

    /// <summary>وضعیت سهمیه‌ی کارشناسی رایگان.</summary>
    [HttpGet("quota")]
    public Task<AppraisalQuotaDto> GetQuota(CancellationToken ct) => _appraisals.GetQuotaAsync(ct);

    /// <summary>تاریخچه‌ی کارشناسی‌های کاربر.</summary>
    [HttpGet("history")]
    public Task<IReadOnlyList<AppraisalHistoryItemDto>> GetHistory(CancellationToken ct) =>
        _appraisals.GetHistoryAsync(ct);

    /// <summary>ثبت یک کارشناسی جدید با لینک آگهی.</summary>
    [HttpPost]
    public async Task<ActionResult<AppraisalResultDto>> Create(
        [FromBody] AppraisalRequestDto request, CancellationToken ct)
    {
        AppraisalResultDto result = await _appraisals.AppraiseAsync(request, ct);
        return Ok(result);
    }
}
