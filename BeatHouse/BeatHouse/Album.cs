using System.ComponentModel.DataAnnotations;

namespace BeatHouse.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int ArtistaId { get; set; }

        public Usuario? Artista { get; set; }

        public List<Cancion>? Canciones { get; set; }



    }
}
