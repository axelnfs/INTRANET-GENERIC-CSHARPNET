using Microsoft.AspNetCore.Mvc;

namespace INTRANET_GENERIC.Controllers;

public class RegisterController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}