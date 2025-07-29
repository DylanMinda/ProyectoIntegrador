using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Spotify.APIConsumer;
using Spotify.Modelos;
using Spotify.MVC.Interface;
using System.Security.Claims;

namespace Spotify.MVC.Services
{
    public class AutorizarService : IAutorizarService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;// Acceso al contexto HTTP para manejar la sesión

        // Inyectar IHttpContextAccessor en el constructor
        public AutorizarService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Login(string email, string contraseña)// Método para autenticar al usuario
        {
            var usuarios = CRUD<Usuario>.GetAll();// Obtener todos los usuarios desde la base de datos
            foreach (var usuario in usuarios)// Iterar sobre cada usuario
            {
                if (usuario.Email == email)// Verificar si el email coincide con el ingresado
                {

                    if (BCrypt.Net.BCrypt.Verify(contraseña, usuario.Contraseña))// Verificar la contraseña usando BCrypt
                    {
                        var datosUsuario = new List<Claim>// Crear una lista de claims con la información del usuario
                        {
                            new Claim(ClaimTypes.Name, usuario.Nombre),
                            new Claim(ClaimTypes.Email, usuario.Email),
                            new Claim("TipoUsuario", usuario.TipoUsuario)
                        };

                        var credencialDigital = new ClaimsIdentity(datosUsuario, "Cookies");// Crear una identidad de claims para el usuario
                        var usuarioAutenticado = new ClaimsPrincipal(credencialDigital);// Crear un ClaimsPrincipal con la identidad del usuario

                        // Usar _httpContextAccessor para acceder al contexto de la sesión
                        await _httpContextAccessor.HttpContext.SignInAsync("Cookies", usuarioAutenticado);// Iniciar sesión del usuario usando cookies
                        return true;// Retornar true si el login fue exitoso
                    }
                }
            }
            return false;
        }

        public async Task<bool> Register(string email, string nombre, string contraseña)// Método para registrar un nuevo usuario
        {
            if (string.IsNullOrEmpty(contraseña))// Verificar si la contraseña está vacía
            {
                return false; // La contraseña no puede ser vacía
            }

            var usuarioExistente = CRUD<Usuario>.GetAll()// Obtener todos los usuarios desde la base de datos
                .FirstOrDefault(u => u.Email == email);// Verificar si ya existe un usuario con el mismo email

            if (usuarioExistente != null)// Si ya existe un usuario con el mismo email
            {
                return false; // El usuario ya existe
            }

            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(contraseña);// Hashear la contraseña usando BCrypt

                var nuevoUsuario = new Usuario// Crear un nuevo objeto Usuario con la información proporcionada
                {
                    Nombre = nombre,
                    Email = email,
                    Contraseña = hashedPassword,
                    TipoUsuario = "Cliente", // O el tipo de usuario que quieras asignar
                    FechaRegistro = DateTime.UtcNow
                };

                CRUD<Usuario>.Create(nuevoUsuario);
                return true; // Registro exitoso
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar usuario: {ex.Message}");
                return false; // Si ocurre un error
            }
        }
    }
}
