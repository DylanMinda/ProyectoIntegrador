using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotify.Modelos;
using System.Security.Claims;

namespace Spotify.MVC.Controllers
{
    [Authorize]
    public class PlaylistsController : Controller
    {
        private readonly AppDbContext _context;

        public PlaylistsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Playlists/Index
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)//si el usuario no está autenticado
            {
                return RedirectToAction("Index", "Login");
            }

            // 1. Obtenemos el Id del usuario logueado
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));// Obtenemos el ID del usuario logueado

            // 2. Filtramos solo las playlists de este usuario
            var playlists = await _context.Playlists// Obtenemos las playlists del usuario logueado
                .Where(p => p.UsuarioId == userId)// Filtramos las playlists por el ID del usuario
                .Include(p => p.DetallesPlaylists)// Incluimos los detalles de la playlist
                    .ThenInclude(d => d.Cancion)// Incluimos las canciones de cada detalle de la playlist
                .ToListAsync();// Convertimos el resultado a una lista asíncrona

            return View(playlists);
        }

        // GET: Playlists/Create
        public async Task<IActionResult> Create(string searchTerm = "", List<int> selectedSongs = null)
        {
            ViewBag.SearchTerm = searchTerm;// Guardamos el término de búsqueda en ViewBag para usarlo en la vista

            // Obtener todas las canciones y aplicar filtro de búsqueda si es necesario
            var cancionesQuery = _context.Canciones.Include(c => c.Album).ThenInclude(a => a.Artista).AsQueryable();// Obtenemos todas las canciones y las incluimos con sus álbumes y artistas

            if (!string.IsNullOrEmpty(searchTerm))// Si hay un término de búsqueda, filtramos las canciones
            {
                cancionesQuery = cancionesQuery.Where(c =>// Filtramos las canciones por el término de búsqueda
                    c.Titulo.Contains(searchTerm) ||// Filtramos por el título de la canción
                    c.Album.Nombre.Contains(searchTerm) ||// Filtramos por el nombre del álbum
                    c.Album.Artista.Nombre.Contains(searchTerm));// Filtramos por el nombre del artista
            }

            var canciones = await cancionesQuery.Take(20).ToListAsync();// Obtenemos las primeras 20 canciones que coinciden con el término de búsqueda
            ViewBag.Canciones = canciones;// Guardamos las canciones en ViewBag para usarlas en la vista

            // Mantener las canciones seleccionadas
            if (selectedSongs != null && selectedSongs.Any())// Si hay canciones seleccionadas, las filtramos
            {
                var cancionesSeleccionadas = canciones.Where(c => selectedSongs.Contains(c.Id)).ToList();// Filtramos las canciones seleccionadas por los IDs en selectedSongs
                ViewBag.CancionesSeleccionadas = cancionesSeleccionadas;// Guardamos las canciones seleccionadas en ViewBag para usarlas en la vista
                ViewBag.SelectedSongIds = selectedSongs;// Guardamos los IDs de las canciones seleccionadas en ViewBag para usarlas en la vista
            }
            else
            {
                ViewBag.CancionesSeleccionadas = new List<Cancion>();// Si no hay canciones seleccionadas, inicializamos la lista vacía
                ViewBag.SelectedSongIds = new List<int>();// Inicializamos la lista de IDs de canciones seleccionadas
            }

            return View(new Playlist { FechaCreacion = DateTime.Now });
        }
        // POST: Buscar canciones
        [HttpPost]
        public async Task<IActionResult> SearchSongs(string searchTerm, List<int> selectedSongs)
        {
            if (!User.Identity.IsAuthenticated)// Verifica si el usuario está autenticado
            {
                ViewBag.ErrorMessage = "No estás autenticado.";
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Create", new { searchTerm = searchTerm, selectedSongs = selectedSongs });// Redirige a la acción Create con el término de búsqueda y las canciones seleccionadas
        }

        // POST: Agregar canción
        [HttpPost]
        public async Task<IActionResult> AddSong(int songId, string searchTerm, List<int> selectedSongs)
        {
            selectedSongs ??= new List<int>();  // Si es null, inicializa la lista

            if (!selectedSongs.Contains(songId))// Verifica si la canción ya está en la lista de seleccionadas
            {
                selectedSongs.Add(songId);// Agrega la canción a la lista de seleccionadas
            }

            return RedirectToAction("Create", new { searchTerm, selectedSongs });// Redirige a la acción Create con el término de búsqueda y las canciones seleccionadas
        }

        // POST: Remover canción
        [HttpPost]
        public async Task<IActionResult> RemoveSong(int songId, string searchTerm, List<int> selectedSongs)
        {
            selectedSongs ??= new List<int>();  // Si es null, inicializa la lista

            selectedSongs.Remove(songId);// Elimina la canción de la lista de seleccionadas

            return RedirectToAction("Create", new { searchTerm, selectedSongs });//Redirige a la acción Create con el término de búsqueda y las canciones seleccionadas
        }

        // POST: Crear playlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,FechaCreacion")] Playlist playlist, List<int> selectedSongs)
        {
            try
            {
                if (ModelState.IsValid)// Verifica si el modelo es válido
                {
                    if (!User.Identity.IsAuthenticated)// Verifica si el usuario está autenticado
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    // Obtener el ID del usuario logueado
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    playlist.UsuarioId = userId;
                    playlist.FechaCreacion = DateTime.UtcNow; // Aseguramos que la fecha esté en UTC

                    // Crear la playlist en la base de datos
                    _context.Add(playlist);
                    await _context.SaveChangesAsync();

                    // Agregar las canciones seleccionadas a la playlist (DetallesPlaylist)
                    if (selectedSongs != null && selectedSongs.Any())
                    {
                        foreach (var songId in selectedSongs)
                        {
                            var detallePlaylist = new DetallesPlaylist// Creamos un nuevo detalle de playlist
                            {
                                PlaylistId = playlist.Id,// Asignamos el ID de la playlist recién creada
                                CancionId = songId// Asignamos el ID de la canción seleccionada
                            };
                            _context.DetallesPlaylist.Add(detallePlaylist); // Relaciona la canción con la playlist
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Redirigir al Index de Playlists
                    ViewBag.SuccessMessage = "Playlist creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ErrorMessage = "Error al crear la playlist. Verifique los campos.";
                return View(playlist);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                ViewBag.ErrorMessage = "Ocurrió un error: " + ex.Message;
                return View(playlist);
            }
        }

        //EDIT Y DELETE 

        // GET: Playlists/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var playlist = await _context.Playlists// Obtenemos la playlist por ID
                .Include(p => p.Creador)// Incluimos el creador de la playlist
                .Include(p => p.DetallesPlaylists)// Incluimos los detalles de la playlist
                    .ThenInclude(dp => dp.Cancion)// Incluimos las canciones de cada detalle de la playlist
                .FirstOrDefaultAsync(p => p.Id == id);// Buscamos la playlist por ID
            if (playlist == null) return NotFound();// Si no se encuentra la playlist, retornamos NotFound
            return View(playlist);
        }

        // GET: Playlists/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.DetallesPlaylists)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (playlist == null) return NotFound();

            // Poblamos ViewBag con todas las canciones y con las ya seleccionadas
            ViewBag.AllSongs = await _context.Canciones.ToListAsync();
            ViewBag.SelectedSongIds = playlist.DetallesPlaylists.Select(d => d.CancionId).ToList();
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Playlist model, List<int> selectedSongs)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.AllSongs = await _context.Canciones.ToListAsync();
                ViewBag.SelectedSongIds = selectedSongs;
                return View(model);
            }

            // Actualizar datos básicos
            _context.Update(model);
            // Primero eliminamos viejos detalles
            var viejos = _context.DetallesPlaylist.Where(d => d.PlaylistId == id);
            _context.DetallesPlaylist.RemoveRange(viejos);
            // Luego agregamos los nuevos
            foreach (var songId in selectedSongs)
                _context.DetallesPlaylist.Add(new DetallesPlaylist { PlaylistId = id, CancionId = songId });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Playlists/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == id);
            if (playlist == null) return NotFound();
            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
