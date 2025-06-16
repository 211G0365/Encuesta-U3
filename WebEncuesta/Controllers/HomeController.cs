using Microsoft.AspNetCore.Mvc;

namespace WebEncuesta.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
