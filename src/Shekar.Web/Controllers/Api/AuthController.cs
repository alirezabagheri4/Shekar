using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Abstractions;
using Shekar.Application.Contracts;
using Shekar.Application.Services;

namespace Shekar.Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;

    public AuthController(IAuthService auth, ICurrentUser currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    /// <summary>مرحله ۱: ارسال کد ۵ رقمی به شماره موبایل.</summary>
    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto request, CancellationToken ct)
    {
        await _auth.RequestOtpAsync(request, ct);
        return NoContent();
    }

    /// <summary>مرحله ۲: تأیید کد و ورود (ثبت Cookie).</summary>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<AuthResultDto>> VerifyOtp([FromBody] VerifyOtpDto request, CancellationToken ct)
    {
        AuthResultDto result = await _auth.VerifyOtpAsync(request, ct);
        await SignInAsync(result.UserId);
        return Ok(result);
    }

    /// <summary>مرحله ۳ (ورود اول): تکمیل پروفایل.</summary>
    [HttpPost("complete-profile")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileDto request, CancellationToken ct)
    {
        await _auth.CompleteProfileAsync(_currentUser.RequireUserId(), request, ct);
        return NoContent();
    }

    /// <summary>خروج.</summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    private async Task SignInAsync(int userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });
    }
}
