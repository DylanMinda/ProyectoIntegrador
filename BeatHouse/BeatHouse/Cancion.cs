using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatHouse.Models
{
   public class Cancion
    {
        [Key]
        public int Codigo { get; set; }

        public string Titulo { get; set; } = null!;

        public TimeSpan Duracion { get; set; }


        public DateTime FechaLanzamiento { get; set; }

        public int TotalReproducciones { get; set; }

        public int ArtistaId { get; set; }

        public int? AlbumId { get; set; }

        public Album? Album { get; set; }

        public Usuario? Artista { get; set; }

        public List<DetallePlaylist>? DetallePlaylists { get; set; }
    }
}
