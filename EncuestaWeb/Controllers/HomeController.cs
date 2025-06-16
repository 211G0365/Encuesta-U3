using Microsoft.AspNetCore.Mvc;

namespace EncuestaWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

       
    }
}
