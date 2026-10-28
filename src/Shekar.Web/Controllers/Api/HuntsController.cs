using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Domain.Enums;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/hunts")]
[Produces("application/json")]
public sealed class HuntsController : ControllerBase
{
    private readonly IHuntService _hunts;

    public HuntsController(IHuntService hunts) => _hunts = hunts;

    /// <summary>لیست شکارها با فیلتر دسته/بازه و مرتب‌سازی.</summary>
    [HttpGet]
    public Task<HuntListDto> GetList(
        [FromQuery] PriceCategory? category,
        [FromQuery] HuntDay day = HuntDay.Today,
        [FromQuery] HuntSortOrder sort = HuntSortOrder.TopScore,
        CancellationToken ct = default) =>
        _hunts.GetListAsync(category, day, sort, ct);

    /// <summary>خلاصه‌ی دسته‌های قیمتی.</summary>
    [HttpGet("categories")]
    public Task<IReadOnlyList<CategorySummaryDto>> GetCategories(CancellationToken ct) =>
        _hunts.GetCategoriesAsync(ct);

    /// <summary>جزئیات یک شکار.</summary>
    [HttpGet("{id:int}")]
    public Task<HuntDetailDto> GetDetail(int id, CancellationToken ct) =>
        _hunts.GetDetailAsync(id, ct);
}
