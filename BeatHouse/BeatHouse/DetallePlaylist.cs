using System.ComponentModel.DataAnnotations;

namespace BeatHouse.Models
{
    public class DetallePlaylist
    {
        [Key]
        public int Id { get; set; }

        public int PlaylistId { get; set; }

        public int CancionId { get; set; }

        public virtual Cancion? Cancion { get; set; }

        public virtual Playlist? Playlist { get; set; }
    }
}