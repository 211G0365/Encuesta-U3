using Microsoft.AspNetCore.Mvc;

namespace EncuestaWeb.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
