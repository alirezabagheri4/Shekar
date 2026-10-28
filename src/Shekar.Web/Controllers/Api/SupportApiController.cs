using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Services;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/support")]
[Produces("application/json")]
public sealed class SupportApiController : ControllerBase
{
    private readonly ISupportService _support;

    public SupportApiController(ISupportService support) => _support = support;

    /// <summary>وضعیت و ساعت پشتیبانی (Asia/Tehran).</summary>
    [HttpGet]
    public ActionResult GetStatus() => Ok(new
    {
        isOpen = _support.IsOpenNow(),
        hours = _support.HoursLabel,
        status = _support.StatusLabel
    });
}
