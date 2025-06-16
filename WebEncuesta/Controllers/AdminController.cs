using Microsoft.AspNetCore.Mvc;

namespace WebEncuesta.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Encuestas()
        {
            return View();
        }

        public IActionResult AddEncuestas()
        {
            return View();
        }
        public IActionResult EditarEncuesta()
        {
            return View();
        }
        public IActionResult EliminarEncuesta()
        {
            return View();
        }
        public IActionResult Usuarios()
        {
            return View();
        }
        public IActionResult AgregarUsuario()
        {
            return View();
        }
        public IActionResult EliminarUsuario()
        {
            return View();
        }
        public IActionResult VerAlumnosQueRespondieron()
        {
            return View();
        }
        public IActionResult VerRespuestasDeAlumno()
        {
            return View();
        }
    }
}
