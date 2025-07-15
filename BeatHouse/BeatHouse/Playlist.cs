using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatHouse.Models
{
  public  class Playlist
    {
        [Key]
        public int id { get; set; }

        public string Nombre { get; set; }

        public int UsuarioCodigo { get; set; }

        public virtual List<DetallePlaylist>? DetallePlaylists { get; set; }

        public virtual Usuario? Usuario { get; set; }
    }
}
