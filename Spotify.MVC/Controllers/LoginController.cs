using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.Modelos;
using Spotify.MVC.Interface;

namespace Spotify.MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAutorizarService _autoService;
        private readonly AppDbContext _context;  // Cambiado a AppDbContext
        private readonly IEmailService _emailService;

        // Modificado el constructor para inyectar el contexto de la base de datos
        public LoginController(IAutorizarService autoService, AppDbContext context, IEmailService emailService)
        {
            _autoService = autoService;
            _context = context;  // Asignamos el contexto aquí
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult RegisterArtista()
        {
            return View();
        }

        // Iniciar sesión
        [HttpPost]
        public async Task<IActionResult> Login(string email, string contraseña)
        {
            // Verificar las credenciales del usuario
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(contraseña, usuario.Contraseña))
            {
                ViewBag.ErrorMessage = "Email o contraseña no son correctos";
                return View("Index");  // Si el login falla, mostrar mensaje de error y retornar al login
            }

<<<<<<< Updated upstream
            // Guardar el ID del usuario en la sesión
            HttpContext.Session.SetInt32("UserId", usuario.Id);
=======
            // Crear los claims para el usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("TipoUsuario", usuario.TipoUsuario)
            };
>>>>>>> Stashed changes

            // Verificar el tipo de usuario y redirigir al dashboard correspondiente
            if (usuario.TipoUsuario == "artista")
            {
                return RedirectToAction("DashboardArtista", "Home");  // Redirige al dashboard de artista
            }
            else if (usuario.TipoUsuario == "admin")
            {
                return RedirectToAction("DashboardAdmin", "Home");  // Redirige al dashboard del admin
            }
            else if (usuario.TipoUsuario == "cliente")
            {
                return RedirectToAction("Dashboard", "Home");  // Redirige al home del cliente
            }

            // Redirige al home en caso de que no sea ninguno de los anteriores (esto debería ser un caso no esperado)
            return RedirectToAction("Index", "Home");
        }

        // Registro para Cliente
        [HttpPost]
        public async Task<IActionResult> Register(string email, string nombre, string contraseña, string confirmPassword)
        {
            if (contraseña != confirmPassword)
            {
                ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                return View("RegisterCliente");
            }

            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuarioExistente != null)
            {
                ViewBag.ErrorMessage = "Este correo electrónico ya está registrado.";
                return View("RegisterCliente");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(contraseña);
            var nuevoUsuario = new Usuario
            {
                Email = email,
                Nombre = nombre,
                Contraseña = hashedPassword,
                TipoUsuario = "cliente",  // Asigna "cliente" como rol
                FechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

<<<<<<< Updated upstream
            ViewBag.SuccessMessage = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction("Index", "Login");
=======
            await _emailService.enviarEmailBienvenida(email);
            ViewBag.SuccessMessage = "Se ha enviado un correo electrónico de bienvenida";

            ViewBag.SuccessMessage = "Registro exitoso. Ahora puedes iniciar sesión.";  // Mensaje de éxito
            return RedirectToAction("Index", "Login");  // Redirige al login después de un registro exitoso
        }
        [HttpGet]
        public IActionResult RegisterArtista()
        {
            return View();
>>>>>>> Stashed changes
        }

        // Registro para Artista
        [HttpPost]
        public async Task<IActionResult> RegisterArtista(string email, string nombre, string contraseña, string confirmPassword)
        {
            if (contraseña != confirmPassword)
            {
                ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                return View("RegisterArtista");
            }

            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuarioExistente != null)
            {
                ViewBag.ErrorMessage = "Este correo electrónico ya está registrado.";
                return View("RegisterArtista");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(contraseña);
            var nuevoUsuario = new Usuario
            {
                Email = email,
                Nombre = nombre,
                Contraseña = hashedPassword,
                TipoUsuario = "artista",  // Asigna "artista" como rol
                FechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction("Index", "Login");
        }

        // Logout
        public IActionResult Logout()
        {
            // Aquí puedes eliminar la sesión o las cookies si las usas
            // Ejemplo: HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
