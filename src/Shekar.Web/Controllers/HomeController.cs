using Microsoft.AspNetCore.Mvc;

namespace Shekar.Web.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => View();
}
