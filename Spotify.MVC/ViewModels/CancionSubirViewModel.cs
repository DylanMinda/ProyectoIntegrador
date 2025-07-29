using System.ComponentModel.DataAnnotations;
namespace Spotify.MVC.ViewModels

{
    // El ViewModel es un patrón de diseño utilizado para transferir datos entre la Vista y el Controlador.
    // Sirve para organizar y transformar los datos necesarios para la presentación, sin exponer directamente el Modelo de datos.
    public class CancionSubirViewModel
    {
        [Required] public string Titulo { get; set; } = null!;
        [Required] public IFormFile Archivo { get; set; } = null!;
        public TimeSpan Duracion { get; set; }
        [Required] public string Genero { get; set; } = null!;
        [Required] public int ArtistaId { get; set; }
        [Required] public int AlbumId { get; set; }
    }
}
