using System.Security.Claims;
using Shekar.Application.Abstractions;
using Shekar.Application.Common;

namespace Shekar.Web.Infrastructure;

/// <summary>کاربر جاری از روی Claimهای Cookie.</summary>
public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public int? UserId
    {
        get
        {
            string? value = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out int id) ? id : null;
        }
    }

    public bool IsAuthenticated => UserId.HasValue;

    public int RequireUserId() => UserId ?? throw new NotAuthenticatedException();
}
