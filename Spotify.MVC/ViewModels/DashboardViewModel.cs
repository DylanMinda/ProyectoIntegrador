using Spotify.Modelos;

namespace Spotify.MVC.ViewModels
{
    public class DashboardViewModel
    {
        public string Nombre { get; set; }
        public double? Saldo { get; set; }
        public Plan? Plan { get; set; }
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();

        public DashboardViewModel()
        {
            Playlists = new List<Playlist>();
        }
    }
}
