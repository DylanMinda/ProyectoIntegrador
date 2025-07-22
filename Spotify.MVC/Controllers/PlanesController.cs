using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.APIConsumer;
using Spotify.Modelos;
using System.Security.Claims;

namespace Spotify.MVC.Controllers
{
    public class PlanesController : Controller
    {
        private readonly AppDbContext _context;

        public PlanesController(AppDbContext context)
        {
            _context = context;
        }

        // Ver los planes disponibles
        public async Task<IActionResult> Index()
        {
            var planes = await _context.Planes.ToListAsync();
            return View(planes);
        }

        // Comprar un plan
        [HttpPost]
        public async Task<IActionResult> ComprarPlan(int planId)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var plan = await _context.Planes.FindAsync(planId);
            if (plan == null)
            {
                return NotFound();
            }

            // Verificar si ya tiene este plan
            if (usuario.Plan != null && usuario.Plan.Id == planId)
            {
                TempData["ErrorMessage"] = "Ya tienes este plan activo.";
                return RedirectToAction("Index");
            }

            // Verificar saldo suficiente
            if (usuario.Saldo < plan.PrecioMensual)
            {
                TempData["ErrorMessage"] = "No tienes suficiente saldo para comprar este plan.";
                return RedirectToAction("Index");
            }

<<<<<<< HEAD
            // CORRECCIÓN: Cada usuario que compra un plan grupal crea su PROPIO plan independiente
            // Ya no verificamos límites aquí porque cada compra crea un plan nuevo
=======
            // NUEVA LÓGICA: Verificar límite de usuarios para planes grupales
            if (plan.MaximoUsuarios > 1) // Es un plan grupal (Familiar/Empresarial)
            {
                // Contar usuarios actuales en este plan
                var usuariosEnPlan = await _context.Usuarios
                    .Where(u => u.Plan != null && u.Plan.Id == planId)
                    .CountAsync();

                if (usuariosEnPlan >= plan.MaximoUsuarios)
                {
                    TempData["ErrorMessage"] = $"El plan {plan.Nombre} ha alcanzado su límite máximo de {plan.MaximoUsuarios} usuarios.";
                    return RedirectToAction("Index");
                }
            }
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5

            // Descontar saldo y asignar plan
            usuario.Saldo -= plan.PrecioMensual;
            usuario.Plan = plan;
<<<<<<< HEAD

            // Si es un plan grupal, generar código de invitación único
            if (plan.MaximoUsuarios > 1)
            {
                usuario.CodigoInvitacion = GenerarCodigoInvitacion();
            }
=======
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

<<<<<<< HEAD
            if (plan.MaximoUsuarios > 1)
            {
                TempData["SuccessMessage"] = $"¡Plan {plan.Nombre} activado con éxito! Tu código de invitación es: {usuario.CodigoInvitacion}";
            }
            else
            {
                TempData["SuccessMessage"] = $"¡Plan {plan.Nombre} activado con éxito!";
            }

            return RedirectToAction("Dashboard", "Home");
        }

        // Método para generar código de invitación único
        private string GenerarCodigoInvitacion()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // NUEVO: Unirse a plan grupal mediante código
        [HttpPost]
        public async Task<IActionResult> UnirseAPlan(string codigoInvitacion)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            // Verificar que no tenga un plan de pago activo
            if (usuario.Plan != null && usuario.Plan.PrecioMensual > 0)
            {
                return Json(new { success = false, message = "Ya tienes un plan de pago activo. Debes cancelarlo primero." });
            }

            // Buscar al administrador del plan con el código
            var administrador = await _context.Usuarios
                .Include(u => u.Plan)
                .FirstOrDefaultAsync(u => u.CodigoInvitacion == codigoInvitacion);

            if (administrador == null || administrador.Plan == null || administrador.Plan.MaximoUsuarios <= 1)
            {
                return Json(new { success = false, message = "Código de invitación inválido o no corresponde a un plan grupal" });
            }

            // Contar usuarios actuales en el plan del administrador
            var usuariosEnPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == administrador.Plan.Id && u.CodigoInvitacion == codigoInvitacion)
                .CountAsync();

            if (usuariosEnPlan >= administrador.Plan.MaximoUsuarios)
            {
                return Json(new { success = false, message = $"El plan ha alcanzado su límite máximo de {administrador.Plan.MaximoUsuarios} usuarios" });
            }

            // Verificar que no esté ya en este plan
            if (usuario.Plan != null && usuario.Plan.Id == administrador.Plan.Id && usuario.CodigoInvitacion == codigoInvitacion)
            {
                return Json(new { success = false, message = "Ya estás en este plan grupal" });
            }

            // Agregar usuario al plan (sin costo adicional)
            usuario.Plan = administrador.Plan;
            usuario.CodigoInvitacion = codigoInvitacion; // Importante: usar el mismo código que el administrador
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Te has unido exitosamente al plan {administrador.Plan.Nombre}"
            });
        }

        // Invitar usuario a plan grupal (MEJORADO - ahora solo comparte el código)
        [HttpGet]
        public async Task<IActionResult> CompartirCodigoInvitacion()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

=======
            TempData["SuccessMessage"] = $"¡Plan {plan.Nombre} activado con éxito!";
            return RedirectToAction("Dashboard", "Home");
        }

        // Invitar usuario a plan grupal (solo para administradores del plan)
        [HttpPost]
        public async Task<IActionResult> InvitarUsuario(string email)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
            if (usuario == null || usuario.Plan == null)
            {
                return Json(new { success = false, message = "No tienes un plan activo" });
            }

            // Verificar que es un plan grupal
            if (usuario.Plan.MaximoUsuarios <= 1)
            {
<<<<<<< HEAD
                return Json(new { success = false, message = "Solo puedes compartir códigos en planes grupales" });
            }

            // Verificar que es el administrador (quien tiene el código original)
            if (string.IsNullOrEmpty(usuario.CodigoInvitacion))
            {
                return Json(new { success = false, message = "No tienes un código de invitación válido" });
            }

            // Verificar que es realmente el administrador (primer usuario con este código)
            var administrador = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id && u.CodigoInvitacion == usuario.CodigoInvitacion)
                .OrderBy(u => u.FechaRegistro)
                .FirstOrDefaultAsync();

            if (administrador.Id != usuarioId)
            {
                return Json(new { success = false, message = "Solo el administrador del plan puede compartir el código" });
            }

            return Json(new
            {
                success = true,
                codigoInvitacion = usuario.CodigoInvitacion,
                message = $"Comparte este código con las personas que quieras invitar: {usuario.CodigoInvitacion}"
            });
        }

        // Ver miembros del plan grupal (MEJORADO)
=======
                return Json(new { success = false, message = "Solo puedes invitar usuarios en planes grupales" });
            }

            // Verificar que es el "administrador" (primer usuario que compró el plan)
            var primerUsuarioDelPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                .OrderBy(u => u.FechaRegistro) // O podrías usar otro criterio
                .FirstOrDefaultAsync();

            if (primerUsuarioDelPlan.Id != usuarioId)
            {
                return Json(new { success = false, message = "Solo el administrador del plan puede invitar usuarios" });
            }

            // Verificar límite de usuarios
            var usuariosEnPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                .CountAsync();

            if (usuariosEnPlan >= usuario.Plan.MaximoUsuarios)
            {
                return Json(new { success = false, message = $"Has alcanzado el límite de {usuario.Plan.MaximoUsuarios} usuarios" });
            }

            // Verificar si el usuario existe
            var usuarioInvitado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuarioInvitado == null)
            {
                return Json(new { success = false, message = "No existe un usuario con ese email" });
            }

            // Verificar que no esté ya en el plan
            if (usuarioInvitado.Plan != null && usuarioInvitado.Plan.Id == usuario.Plan.Id)
            {
                return Json(new { success = false, message = "Este usuario ya está en tu plan" });
            }

            // Agregar usuario al plan (sin costo adicional)
            usuarioInvitado.Plan = usuario.Plan;
            _context.Usuarios.Update(usuarioInvitado);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Usuario {usuarioInvitado.Nombre} agregado al plan exitosamente"
            });
        }

        // Ver miembros del plan grupal
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
        [HttpGet]
        public async Task<IActionResult> MiembrosDelPlan()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null || usuario.Plan == null)
            {
                return RedirectToAction("Index");
            }

            // Verificar que es un plan grupal
            if (usuario.Plan.MaximoUsuarios <= 1)
            {
                TempData["ErrorMessage"] = "Esta función solo está disponible para planes grupales.";
                return RedirectToAction("Index");
            }

<<<<<<< HEAD
            // Obtener todos los miembros del plan con el mismo código de invitación
            var miembrosDelPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id && u.CodigoInvitacion == usuario.CodigoInvitacion)
=======
            // Obtener todos los miembros del plan
            var miembrosDelPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
                .OrderBy(u => u.FechaRegistro)
                .ToListAsync();

            // Determinar quién es el administrador (primer usuario)
            var administrador = miembrosDelPlan.FirstOrDefault();

            ViewBag.Plan = usuario.Plan;
            ViewBag.EsAdministrador = administrador?.Id == usuarioId;
            ViewBag.Administrador = administrador;
            ViewBag.EspaciosDisponibles = usuario.Plan.MaximoUsuarios - miembrosDelPlan.Count;
<<<<<<< HEAD
            ViewBag.CodigoInvitacion = usuario.CodigoInvitacion;
=======
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5

            return View(miembrosDelPlan);
        }

<<<<<<< HEAD
        // Expulsar miembro del plan (MEJORADO)
=======
        // Expulsar miembro del plan
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
        [HttpPost]
        public async Task<IActionResult> ExpulsarMiembro(int miembroId)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null || usuario.Plan == null)
            {
                return Json(new { success = false, message = "No tienes un plan activo" });
            }

            // Verificar que es el administrador
            var administrador = await _context.Usuarios
<<<<<<< HEAD
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id && u.CodigoInvitacion == usuario.CodigoInvitacion)
=======
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
                .OrderBy(u => u.FechaRegistro)
                .FirstOrDefaultAsync();

            if (administrador.Id != usuarioId)
            {
                return Json(new { success = false, message = "Solo el administrador puede expulsar miembros" });
            }

            // No puede expulsarse a sí mismo
            if (miembroId == usuarioId)
            {
                return Json(new { success = false, message = "No puedes expulsarte a ti mismo del plan" });
            }

            var miembro = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == miembroId);
<<<<<<< HEAD
            if (miembro == null || miembro.Plan?.Id != usuario.Plan.Id || miembro.CodigoInvitacion != usuario.CodigoInvitacion)
=======
            if (miembro == null || miembro.Plan?.Id != usuario.Plan.Id)
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
            {
                return Json(new { success = false, message = "Miembro no encontrado en tu plan" });
            }

            // Remover del plan (vuelve a plan gratuito)
            var planGratuito = await _context.Planes.FirstOrDefaultAsync(p => p.PrecioMensual == 0);
            miembro.Plan = planGratuito;
<<<<<<< HEAD
            miembro.CodigoInvitacion = null; // Limpiar código de invitación
=======
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
            _context.Usuarios.Update(miembro);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Miembro expulsado exitosamente" });
        }

<<<<<<< HEAD
        // Abandonar plan grupal (MEJORADO)
=======
        // Abandonar plan grupal
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
        [HttpPost]
        public async Task<IActionResult> AbandonarPlan()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null || usuario.Plan == null)
            {
                return Json(new { success = false, message = "No tienes un plan activo" });
            }

            // Verificar si es administrador
            var administrador = await _context.Usuarios
<<<<<<< HEAD
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id && u.CodigoInvitacion == usuario.CodigoInvitacion)
=======
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
                .OrderBy(u => u.FechaRegistro)
                .FirstOrDefaultAsync();

            if (administrador.Id == usuarioId)
            {
                return Json(new { success = false, message = "No puedes abandonar un plan que administras. Debes cancelar el plan." });
            }

            // Cambiar a plan gratuito
            var planGratuito = await _context.Planes.FirstOrDefaultAsync(p => p.PrecioMensual == 0);
            usuario.Plan = planGratuito;
<<<<<<< HEAD
            usuario.CodigoInvitacion = null; // Limpiar código de invitación
=======
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Has abandonado el plan grupal exitosamente" });
        }

<<<<<<< HEAD
        // Cancelar plan grupal (MEJORADO)
=======
        // Cancelar plan grupal (solo administrador)
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
        [HttpPost]
        public async Task<IActionResult> CancelarPlan()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null || usuario.Plan == null)
            {
                return Json(new { success = false, message = "No tienes un plan activo" });
            }

            // Verificar que es el administrador
            var administrador = await _context.Usuarios
<<<<<<< HEAD
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id && u.CodigoInvitacion == usuario.CodigoInvitacion)
=======
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
                .OrderBy(u => u.FechaRegistro)
                .FirstOrDefaultAsync();

            if (administrador.Id != usuarioId)
            {
                return Json(new { success = false, message = "Solo el administrador puede cancelar el plan" });
            }

<<<<<<< HEAD
            // Obtener todos los miembros del plan con el mismo código
            var miembrosDelPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id && u.CodigoInvitacion == usuario.CodigoInvitacion)
=======
            // Obtener todos los miembros del plan
            var miembrosDelPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
                .ToListAsync();

            // Cambiar todos los miembros a plan gratuito
            var planGratuito = await _context.Planes.FirstOrDefaultAsync(p => p.PrecioMensual == 0);
            foreach (var miembro in miembrosDelPlan)
            {
                miembro.Plan = planGratuito;
<<<<<<< HEAD
                miembro.CodigoInvitacion = null; // Limpiar código de invitación
=======
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
            }

            _context.Usuarios.UpdateRange(miembrosDelPlan);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Plan cancelado exitosamente. Todos los miembros han sido movidos al plan gratuito." });
        }

<<<<<<< HEAD
        // Obtener información del plan actual (MEJORADO)
=======
        // Obtener información del plan actual
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
        [HttpGet]
        public async Task<IActionResult> MiPlan()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            if (usuario.Plan == null)
            {
                ViewBag.Mensaje = "No tienes un plan activo.";
                return View();
            }

            // Si es plan grupal, obtener información adicional
            if (usuario.Plan.MaximoUsuarios > 1)
            {
                var miembrosDelPlan = await _context.Usuarios
<<<<<<< HEAD
                    .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id && u.CodigoInvitacion == usuario.CodigoInvitacion)
                    .CountAsync();

                var administrador = await _context.Usuarios
                    .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id && u.CodigoInvitacion == usuario.CodigoInvitacion)
=======
                    .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                    .CountAsync();

                var administrador = await _context.Usuarios
                    .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
                    .OrderBy(u => u.FechaRegistro)
                    .FirstOrDefaultAsync();

                ViewBag.CantidadMiembros = miembrosDelPlan;
                ViewBag.EsAdministrador = administrador?.Id == usuarioId;
                ViewBag.Administrador = administrador;
<<<<<<< HEAD
                ViewBag.CodigoInvitacion = usuario.CodigoInvitacion;
=======
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
            }

            return View(usuario);
        }

        // Métodos originales mantenidos
        [HttpGet]
        public IActionResult RecargarSaldo()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecargarSaldo(double monto)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);
            if (usuario == null) return NotFound();

            usuario.Saldo = (usuario.Saldo ?? 0) + monto;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Has recargado ${monto} con éxito.";
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CambiarPlan(int nuevoPlanId)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);
            var nuevoPlan = await _context.Planes.FindAsync(nuevoPlanId);

            if (usuario == null || nuevoPlan == null)
            {
                return NotFound();
            }

            // Verificar si el usuario ya tiene un plan activo
            if (usuario.Plan != null && usuario.Plan.Id != nuevoPlanId)
            {
                TempData["ErrorMessage"] = "Ya tienes un plan activo. Cancela tu plan actual antes de cambiar.";
                return RedirectToAction("Dashboard", "Home");
            }

<<<<<<< HEAD
            // CORRECCIÓN: Ya no verificamos límites globales aquí porque cada compra es independiente
=======
            // Verificar límite para planes grupales
            if (nuevoPlan.MaximoUsuarios > 1)
            {
                var usuariosEnPlan = await _context.Usuarios
                    .Where(u => u.Plan != null && u.Plan.Id == nuevoPlanId)
                    .CountAsync();

                if (usuariosEnPlan >= nuevoPlan.MaximoUsuarios)
                {
                    TempData["ErrorMessage"] = $"El plan {nuevoPlan.Nombre} ha alcanzado su límite máximo de usuarios.";
                    return RedirectToAction("Index");
                }
            }
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5

            // Verificar saldo
            if (usuario.Saldo >= nuevoPlan.PrecioMensual)
            {
                usuario.Saldo -= nuevoPlan.PrecioMensual;
                usuario.Plan = nuevoPlan;

<<<<<<< HEAD
                // Si es un plan grupal, generar nuevo código
                if (nuevoPlan.MaximoUsuarios > 1)
                {
                    usuario.CodigoInvitacion = GenerarCodigoInvitacion();
                }

=======
>>>>>>> 38bd56aee838ff4ca0ba931573fc2338dff106f5
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"¡Plan {nuevoPlan.Nombre} activado con éxito!";
                return RedirectToAction("Dashboard", "Home");
            }

            TempData["ErrorMessage"] = "No tienes suficiente saldo para cambiar de plan.";
            return RedirectToAction("Index");
        }
    }
}