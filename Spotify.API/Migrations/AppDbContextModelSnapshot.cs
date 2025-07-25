﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Spotify.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Spotify.Modelos.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArtistaId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("FechaLanzamiento")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArtistaId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("Spotify.Modelos.Cancion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AlbumId")
                        .HasColumnType("integer");

                    b.Property<string>("ArchivoUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ArtistaId")
                        .HasColumnType("integer");

                    b.Property<TimeSpan>("Duracion")
                        .HasColumnType("interval");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ArtistaId");

                    b.ToTable("Canciones");
                });

            modelBuilder.Entity("Spotify.Modelos.DetallesPlaylist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CancionId")
                        .HasColumnType("integer");

                    b.Property<int>("PlaylistId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CancionId");

                    b.HasIndex("PlaylistId");

                    b.ToTable("DetallesPlaylist");
                });

            modelBuilder.Entity("Spotify.Modelos.Plan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MaximoUsuarios")
                        .HasColumnType("integer");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("PrecioMensual")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Planes");
                });

            modelBuilder.Entity("Spotify.Modelos.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("Spotify.Modelos.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Contraseña")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("FechaRegistro")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("PlanId")
                        .HasColumnType("integer");

                    b.Property<double?>("Saldo")
                        .HasColumnType("double precision");

                    b.Property<string>("TipoUsuario")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PlanId");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Spotify.Modelos.Album", b =>
                {
                    b.HasOne("Spotify.Modelos.Usuario", "Artista")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artista");
                });

            modelBuilder.Entity("Spotify.Modelos.Cancion", b =>
                {
                    b.HasOne("Spotify.Modelos.Album", "Album")
                        .WithMany("Canciones")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Spotify.Modelos.Usuario", "ArtistaCodigoNav")
                        .WithMany("Canciones")
                        .HasForeignKey("ArtistaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("ArtistaCodigoNav");
                });

            modelBuilder.Entity("Spotify.Modelos.DetallesPlaylist", b =>
                {
                    b.HasOne("Spotify.Modelos.Cancion", "Cancion")
                        .WithMany("DetallesPlaylist")
                        .HasForeignKey("CancionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Spotify.Modelos.Playlist", "Playlist")
                        .WithMany("DetallesPlaylists")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cancion");

                    b.Navigation("Playlist");
                });

            modelBuilder.Entity("Spotify.Modelos.Playlist", b =>
                {
                    b.HasOne("Spotify.Modelos.Usuario", "Creador")
                        .WithMany("Playlists")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creador");
                });

            modelBuilder.Entity("Spotify.Modelos.Usuario", b =>
                {
                    b.HasOne("Spotify.Modelos.Plan", "Plan")
                        .WithMany()
                        .HasForeignKey("PlanId");

                    b.Navigation("Plan");
                });

            modelBuilder.Entity("Spotify.Modelos.Album", b =>
                {
                    b.Navigation("Canciones");
                });

            modelBuilder.Entity("Spotify.Modelos.Cancion", b =>
                {
                    b.Navigation("DetallesPlaylist");
                });

            modelBuilder.Entity("Spotify.Modelos.Playlist", b =>
                {
                    b.Navigation("DetallesPlaylists");
                });

            modelBuilder.Entity("Spotify.Modelos.Usuario", b =>
                {
                    b.Navigation("Albums");

                    b.Navigation("Canciones");

                    b.Navigation("Playlists");
                });
#pragma warning restore 612, 618
        }
    }
}
