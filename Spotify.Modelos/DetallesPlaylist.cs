﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Modelos
{
    public class DetallesPlaylist
    {
        public int Id { get; set; }
        public int PlaylistId { get; set; }
        public int CancionId { get; set; }

        public virtual Playlist? Playlist { get; set; }
        public virtual Cancion? Cancion { get; set; } 
    }
}
