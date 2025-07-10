using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotify.Modelos;

namespace Spotify.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AsignarAdmin(string email)
        {
            // Verificar si el usuario existe
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                ViewBag.ErrorMessage = "Usuario no encontrado.";
                return View("DashboardAdmin");  // Redirige a la vista o dashboard correspondiente
            }

            // Verificar si el usuario ya tiene el rol Admin
            var roles = await _userManager.GetRolesAsync(usuario);
            if (roles.Contains("Admin"))
            {
                ViewBag.ErrorMessage = "El usuario ya tiene el rol de Admin.";
                return View("DashboardAdmin");
            }

            // Asignar el rol Admin
            await _userManager.AddToRoleAsync(usuario, "Admin");

            ViewBag.SuccessMessage = "El rol de Admin ha sido asignado correctamente.";
            return RedirectToAction("DashboardAdmin");  // Redirige al dashboard o página administrativa
        }
        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
