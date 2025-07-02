using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Modelos
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public string TipoUsuario { get; set; } // Puede ser "Usuario", "Artista" o "Administrador"
        public Plan Plan { get; set; } // Plan al que está suscrito el usuario
        public int PlanId { get; set; } // Identificador del plan al que está suscrito el usuario

        public List<Playlist>? Playlists { get; set; } // Lista de playlists creadas por el usuario


    }
}
