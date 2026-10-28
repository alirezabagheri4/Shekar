using Microsoft.AspNetCore.Mvc;

namespace Shekar.Web.Controllers;

public class LoginController : Controller
{
    [HttpGet("/login")]
    public IActionResult Index() => View();

    [HttpGet("/login/otp")]
    public IActionResult Otp() => View();

    [HttpGet("/login/profile")]
    public IActionResult Profile() => View();
}
