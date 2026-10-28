using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Contracts;
using Shekar.Application.Services;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/cities")]
[Produces("application/json")]
public sealed class CitiesController : ControllerBase
{
    private readonly ICapacityService _capacity;

    public CitiesController(ICapacityService capacity) => _capacity = capacity;

    /// <summary>ظرفیت اشتراک فعال یک شهر.</summary>
    [HttpGet("{id:int}/capacity")]
    public Task<CityCapacityDto> GetCapacity(int id, CancellationToken ct) => _capacity.GetAsync(id, ct);
}
