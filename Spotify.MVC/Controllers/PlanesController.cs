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

            // Descontar saldo y asignar plan
            usuario.Saldo -= plan.PrecioMensual;
            usuario.Plan = plan;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"¡Plan {plan.Nombre} activado con éxito!";
            return RedirectToAction("Dashboard", "Home");
        }

        // Invitar usuario a plan grupal (solo para administradores del plan)
        [HttpPost]
        public async Task<IActionResult> InvitarUsuario(string email)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.Include(u => u.Plan).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null || usuario.Plan == null)
            {
                return Json(new { success = false, message = "No tienes un plan activo" });
            }

            // Verificar que es un plan grupal
            if (usuario.Plan.MaximoUsuarios <= 1)
            {
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

            // Obtener todos los miembros del plan
            var miembrosDelPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                .OrderBy(u => u.FechaRegistro)
                .ToListAsync();

            // Determinar quién es el administrador (primer usuario)
            var administrador = miembrosDelPlan.FirstOrDefault();

            ViewBag.Plan = usuario.Plan;
            ViewBag.EsAdministrador = administrador?.Id == usuarioId;
            ViewBag.Administrador = administrador;
            ViewBag.EspaciosDisponibles = usuario.Plan.MaximoUsuarios - miembrosDelPlan.Count;

            return View(miembrosDelPlan);
        }

        // Expulsar miembro del plan
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
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
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
            if (miembro == null || miembro.Plan?.Id != usuario.Plan.Id)
            {
                return Json(new { success = false, message = "Miembro no encontrado en tu plan" });
            }

            // Remover del plan (vuelve a plan gratuito)
            var planGratuito = await _context.Planes.FirstOrDefaultAsync(p => p.PrecioMensual == 0);
            miembro.Plan = planGratuito;
            _context.Usuarios.Update(miembro);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Miembro expulsado exitosamente" });
        }

        // Abandonar plan grupal
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
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                .OrderBy(u => u.FechaRegistro)
                .FirstOrDefaultAsync();

            if (administrador.Id == usuarioId)
            {
                return Json(new { success = false, message = "No puedes abandonar un plan que administras. Debes cancelar el plan." });
            }

            // Cambiar a plan gratuito
            var planGratuito = await _context.Planes.FirstOrDefaultAsync(p => p.PrecioMensual == 0);
            usuario.Plan = planGratuito;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Has abandonado el plan grupal exitosamente" });
        }

        // Cancelar plan grupal (solo administrador)
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
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                .OrderBy(u => u.FechaRegistro)
                .FirstOrDefaultAsync();

            if (administrador.Id != usuarioId)
            {
                return Json(new { success = false, message = "Solo el administrador puede cancelar el plan" });
            }

            // Obtener todos los miembros del plan
            var miembrosDelPlan = await _context.Usuarios
                .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                .ToListAsync();

            // Cambiar todos los miembros a plan gratuito
            var planGratuito = await _context.Planes.FirstOrDefaultAsync(p => p.PrecioMensual == 0);
            foreach (var miembro in miembrosDelPlan)
            {
                miembro.Plan = planGratuito;
            }

            _context.Usuarios.UpdateRange(miembrosDelPlan);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Plan cancelado exitosamente. Todos los miembros han sido movidos al plan gratuito." });
        }

        // Obtener información del plan actual
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
                    .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                    .CountAsync();

                var administrador = await _context.Usuarios
                    .Where(u => u.Plan != null && u.Plan.Id == usuario.Plan.Id)
                    .OrderBy(u => u.FechaRegistro)
                    .FirstOrDefaultAsync();

                ViewBag.CantidadMiembros = miembrosDelPlan;
                ViewBag.EsAdministrador = administrador?.Id == usuarioId;
                ViewBag.Administrador = administrador;
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

            // Verificar saldo
            if (usuario.Saldo >= nuevoPlan.PrecioMensual)
            {
                usuario.Saldo -= nuevoPlan.PrecioMensual;
                usuario.Plan = nuevoPlan;

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