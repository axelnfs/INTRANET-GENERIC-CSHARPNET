using Microsoft.AspNetCore.Mvc;

namespace INTRANET_GENERIC.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
