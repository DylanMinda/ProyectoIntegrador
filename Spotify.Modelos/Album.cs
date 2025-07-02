namespace Spotify.Modelos
{
    public class Album
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Artista { get; set; }
        public string Genero { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public List<Cancion>? Canciones { get; set; }

    }
}
