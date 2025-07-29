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
                return RedirectToAction("Index", "Login");  // Si no está logueado o no es admin, redirige al login
            }

            // Obtener estadísticas del sistema
            var totalUsers = _context.Usuarios.Count();
            var totalCanciones = _context.Canciones.Count();

            var model = new
            {
                TotalUsers = totalUsers,
                TotalCanciones = totalCanciones,
                Usuario = usuario  // Información básica del admin
            };

            return View(model);  // Pasa el modelo a la vista de DashboardAdmin
        }


        public IActionResult Dashboard()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = _context.Usuarios.Include(u => u.Plan)
                                           .Include(u => u.Canciones)
                                           .FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");  // Si no está logueado, redirige al login
            }

            // Obtener canciones para mostrar en tendencia (todas las canciones disponibles en Azure)
            var canciones = _context.Canciones.Take(12).ToList(); // Tomar las primeras 12 canciones

            // Obtener playlists del usuario (aquí puedes ajustar según tu modelo de Playlist)
            var playlists = _context.Playlists?.Where(p => p.UsuarioId == usuarioId).Take(6).ToList() ?? new List<Playlist>();

            var cancion = usuario.Canciones?.FirstOrDefault();

            var model = new
            {
                Nombre = usuario.Nombre,
                Plan = usuario.Plan,
                Saldo = usuario.Saldo ?? 0, // Asignar saldo 0 si es nulo
                CancionUrl = cancion?.ArchivoUrl,
                CancionId = cancion?.Id,
                Canciones = canciones // Agregar canciones para la sección de tendencias
            };

            // Pasar playlists a la vista
            ViewBag.Playlists = playlists;

            return View(model);
        }


        public IActionResult DashboardArtista()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usuario = _context.Usuarios.Include(u => u.Plan)
                                           .Include(u => u.Canciones)  // Asegúrate de incluir las canciones del artista
                                           .FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");  // Si no está logueado, redirige al login
            }

            var model = new
            {
                Nombre = usuario.Nombre,
                Plan = usuario.Plan,
                Canciones = usuario.Canciones,  // Lista de canciones que el artista ha subido
                CancionUrl = usuario.Canciones?.FirstOrDefault()?.ArchivoUrl // Primera canción del artista
            };

            return View(model);  // Pasa el modelo a la vista de DashboardArtista
        }


        public IActionResult Logout()
        {
            // Eliminar las cookies de autenticación y limpiar la sesión
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