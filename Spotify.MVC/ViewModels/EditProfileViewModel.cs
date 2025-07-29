namespace Spotify.MVC.ViewModels
{
    // El ViewModel es un patrón de diseño utilizado para transferir datos entre la Vista y el Controlador.
    // Sirve para organizar y transformar los datos necesarios para la presentación, sin exponer directamente el Modelo de datos.
    public class EditProfileViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoUsuario { get; set; }
    }

}
