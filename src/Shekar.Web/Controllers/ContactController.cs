using Microsoft.AspNetCore.Mvc;

namespace Shekar.Web.Controllers;

public class ContactController : Controller
{
    [HttpGet("/contact")]
    public IActionResult Index() => View();
}
