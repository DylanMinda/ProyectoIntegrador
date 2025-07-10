using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.MVC.Models;
using System.Diagnostics;

namespace Spotify.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            var usuarioId = HttpContext.Session.GetInt32("UserId");

            var usuario = _context.Usuarios.Include(u => u.Plan).FirstOrDefault(u => u.Id == usuarioId);
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");  // Redirige al login si no está logueado
            }

            return View("Dashboard", usuario);  // Especifica la vista correctamente
        }
        public IActionResult DashboardArtista()
        {
            var usuarioId = HttpContext.Session.GetInt32("UserId");

            var usuario = _context.Usuarios.Include(u => u.Plan).FirstOrDefault(u => u.Id == usuarioId);
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");  // Redirige al login si no está logueado
            }

            return View("DashboardArtista", usuario);   // Esto busca automáticamente la vista DashboardArtista.cshtml en la carpeta Views/Home
        }
        // Acción para cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();  // Limpiar la sesión
            return RedirectToAction("Index", "Home");  // Redirigir al login
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
