using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.Modelos;
using Spotify.MVC.Models;
using System.Diagnostics;
using System.Security.Claims;

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

        public IActionResult DashboardAdmin()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = _context.Usuarios.Include(u => u.Plan)
                                           .FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null || usuario.TipoUsuario != "admin")
            {
                return RedirectToAction("Index", "Login");  // Si no est� logueado o no es admin, redirige al login
            }

            // Obtener estad�sticas del sistema
            var totalUsers = _context.Usuarios.Count();
            var totalCanciones = _context.Canciones.Count();

            var model = new
            {
                TotalUsers = totalUsers,
                TotalCanciones = totalCanciones,
                Usuario = usuario  // Informaci�n b�sica del admin
            };

            return View(model);  // Pasa el modelo a la vista de DashboardAdmin
        }


        //public IActionResult DashboardAdmin()
        //{
        //    // Si quieres, carga datos para estad�sticas:
        //    var totalUsers = _context.Usuarios.Count();
        //    ViewBag.TotalUsers = totalUsers;

        //    var usuarioId = HttpContext.Session.GetInt32("UserId");

        //    var usuario = _context.Usuarios.Include(u => u.Plan).FirstOrDefault(u => u.Id == usuarioId);
        //    if (usuario == null)
        //    {
        //        return RedirectToAction("Index", "Login");  // Redirige al login si no esta logueado
        //    }

        //    return View("DashboardAdmin", usuario);  // Especifica la vista correctamente
        //}
        public IActionResult Dashboard()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = _context.Usuarios.Include(u => u.Plan)
                                           .Include(u => u.Canciones)
                                           .FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");  // Si no est� logueado, redirige al login
            }

            var cancion = usuario.Canciones?.FirstOrDefault();

            var model = new
            {
                Nombre = usuario.Nombre,
                Plan = usuario.Plan,
                Saldo = usuario.Saldo ?? 0, // Asignar saldo 0 si es nulo
                CancionUrl = cancion?.ArchivoUrl,
                CancionId = cancion?.Id
            };

            return View(model);
        }


        public IActionResult DashboardArtista()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = _context.Usuarios.Include(u => u.Plan)
                                           .Include(u => u.Canciones)  // Aseg�rate de incluir las canciones del artista
                                           .FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");  // Si no est� logueado, redirige al login
            }

            var model = new
            {
                Nombre = usuario.Nombre,
                Plan = usuario.Plan,
                Canciones = usuario.Canciones,  // Lista de canciones que el artista ha subido
                CancionUrl = usuario.Canciones?.FirstOrDefault()?.ArchivoUrl // Primera canci�n del artista
            };

            return View(model);  // Pasa el modelo a la vista de DashboardArtista
        }

        //public IActionResult DashboardArtista()
        //{
        //    var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //    var usuario = _context.Usuarios.Include(u => u.Plan).FirstOrDefault(u => u.Id == usuarioId);
        //    if (usuario == null)
        //    {
        //        return RedirectToAction("Index", "Login");  // Redirige al login si no est? logueado
        //    }

        //    return View("DashboardArtista", usuario);   // Esto busca autom?ticamente la vista DashboardArtista.cshtml en la carpeta Views/Home
        //}

        // Acci�n para cerrar sesi�n
        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear();  // Limpiar la sesi�n
        //    return RedirectToAction("Index", "Home");  // Redirigir al login
        //}

        public IActionResult Logout()
        {
            // Eliminar las cookies de autenticaci�n y limpiar la sesi�n
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
