using Microsoft.AspNetCore.Mvc;

namespace Monitoring.Controllers;

public class InputController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}