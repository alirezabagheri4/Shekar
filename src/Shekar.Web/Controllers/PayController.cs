using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shekar.Web.Controllers;

[Authorize]
public class PayController : Controller
{
    [HttpGet("/pay")]
    public IActionResult Index() => View();

    [HttpGet("/pay/success")]
    public IActionResult Success() => View();
}
