using Spotify.Modelos;

namespace Spotify.MVC.ViewModels
{
    // El ViewModel es un patrón de diseño utilizado para transferir datos entre la Vista y el Controlador.
    // Sirve para organizar y transformar los datos necesarios para la presentación, sin exponer directamente el Modelo de datos.
    public class AdminDashboardViewModel
    {
        public List<Usuario> Usuarios { get; set; }
        public List<Cancion> Canciones { get; set; }
        public List<Album> Albums { get; set; }
    }
}
