using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BeatHouse.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<BeatHouse.Models.Album> Albums { get; set; } = default!;

public DbSet<BeatHouse.Models.Cancion> Canciones { get; set; } = default!;

public DbSet<BeatHouse.Models.DetallePlaylist> DetallePlaylists { get; set; } = default!;

public DbSet<BeatHouse.Models.Plan> Planes { get; set; } = default!;

public DbSet<BeatHouse.Models.Playlist> Playlists { get; set; } = default!;

public DbSet<BeatHouse.Models.Usuario> Usuarios { get; set; } = default!;
    }
