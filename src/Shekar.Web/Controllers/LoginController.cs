using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shekar.Application.Abstractions;
using Shekar.Application.Common;
using Shekar.Application.Contracts;
using Shekar.Application.Services;
using Shekar.Web.Models;

namespace Shekar.Web.Controllers;

public class LoginController : Controller
{
    private readonly IAuthService _auth;
    private readonly ICityRepository _cities;
    private readonly ICurrentUser _currentUser;

    public LoginController(IAuthService auth, ICityRepository cities, ICurrentUser currentUser)
    {
        _auth = auth;
        _cities = cities;
        _currentUser = currentUser;
    }

    // مرحله ۱ — شماره موبایل.
    [HttpGet("/login")]
    public IActionResult Index(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Index(RequestOtpDto request, string? returnUrl, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(request);
        }

        await _auth.RequestOtpAsync(request, ct);
        return RedirectToAction(nameof(Otp), new { mobile = request.Mobile, returnUrl });
    }

    // مرحله ۲ — کد ۵ رقمی.
    [HttpGet("/login/otp")]
    public IActionResult Otp(string mobile, string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginOtpViewModel(mobile));
    }

    [HttpPost("/login/otp")]
    public async Task<IActionResult> Otp(string mobile, string? returnUrl, string? d1, string? d2, string? d3, string? d4, string? d5, CancellationToken ct = default)
    {
        string code = string.Concat(d1, d2, d3, d4, d5).Trim();
        try
        {
            AuthResultDto result = await _auth.VerifyOtpAsync(new VerifyOtpDto { Mobile = mobile, Code = code }, ct);
            await SignInAsync(result.UserId);

            if (result.RequiresProfile)
            {
                return RedirectToAction(nameof(Profile), new { returnUrl });
            }

            return SafeRedirect(returnUrl);
        }
        catch (AppException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginOtpViewModel(mobile));
        }
    }

    // مرحله ۳ — تکمیل پروفایل (ورود اول).
    [HttpGet("/login/profile")]
    public async Task<IActionResult> Profile(string? returnUrl, CancellationToken ct)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var cities = await _cities.ListAsync(ct);
        return View(new LoginProfileViewModel(cities.Select(c => (c.Id, c.Name, c.IsActive)).ToList()));
    }

    [HttpPost("/login/profile")]
    public async Task<IActionResult> Profile(CompleteProfileDto request, string? returnUrl, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var cities = await _cities.ListAsync(ct);
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginProfileViewModel(cities.Select(c => (c.Id, c.Name, c.IsActive)).ToList()));
        }

        await _auth.CompleteProfileAsync(_currentUser.RequireUserId(), request, ct);
        return SafeRedirect(returnUrl);
    }

    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    private IActionResult SafeRedirect(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : Redirect("/hunts");

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
