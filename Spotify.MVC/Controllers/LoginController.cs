using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.APIConsumer;
using Spotify.Modelos;
using Spotify.MVC.Interface;
using Spotify.MVC.Services;

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

        [HttpPost]
        public async Task<IActionResult> Login(string email, string contraseña)
        {
            if (await _autoService.Login(email, contraseña))
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
                HttpContext.Session.SetInt32("UserId", usuario.Id);  // Guardar el ID del usuario en la sesión
                return RedirectToAction("Dashboard", "Home");  // Redirigir al dashboard con las funcionalidades
            }
            else
            {
                ViewBag.ErrorMessage = "Email o contraseña no son correctos";
                return View("Index");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string nombre, string contraseña, string confirmPassword)
        {
            // Verifica que la contraseña y la confirmación coincidan
            if (contraseña != confirmPassword)
            {
                ViewBag.ErrorMessage = "Las contraseñas no coinciden.";
                return View();
            }

            // Asegúrate de que esta consulta esté buscando correctamente el correo en la base de datos.
            var usuarioExistente = await _context.Usuarios
                                                 .FirstOrDefaultAsync(u => u.Email == email);

            if (usuarioExistente != null)
            {
                ViewBag.ErrorMessage = "El correo electrónico ya está registrado.";
                return View();  // Retorna al formulario con el mensaje de error
            }

            // Si no existe, crea el nuevo usuario
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(contraseña);  // Hashea la contraseña antes de guardarla
            var nuevoUsuario = new Usuario
            {
                Email = email,
                Nombre = nombre,
                Contraseña = hashedPassword,
                TipoUsuario = "cliente",
                FechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();  // Guarda el nuevo usuario en la base de datos

            ViewBag.SuccessMessage = "Registro exitoso. Ahora puedes iniciar sesión.";  // Mensaje de éxito
            return RedirectToAction("Index", "Login");  // Redirige al login después de un registro exitoso
        }
        [HttpGet]
        public IActionResult RecuperarContraseña()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarContraseña(string email)
        {
            var usuario = CRUD<Usuario>.GetAll().FirstOrDefault(u => u.Email == email);
            if (usuario == null)
            {
                ViewBag.ErrorMessage = "El correo electrónico no está registrado.";
                return View("Index");
            }
            // Enviar correo electrónico de recuperación de contraseña
            await _emailService.enviarEmailRecuperacionContraseña(email);
            ViewBag.SuccessMessage = "Se ha enviado un correo electrónico para recuperar la contraseña.";
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Logout()
        {
            // Aquí puedes eliminar la sesión o las cookies si las usas
            // Ejemplo: HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
    