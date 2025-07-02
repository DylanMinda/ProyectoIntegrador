using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Modelos
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string ImagenUrl { get; set; }
        public List<Cancion>? Canciones { get; set; } // Lista de canciones en la playlist
        public List<Usuario>? Seguidores { get; set; } // Lista de usuarios que siguen la playlist
        public Usuario Creador { get; set; } // Usuario que creó la playlist
    }
}
