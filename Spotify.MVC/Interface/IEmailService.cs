namespace Spotify.MVC.Interface
{
    public interface IEmailService
    {
        Task enviarEmailRecuperacionContraseña(string email);// Método para enviar un email de recuperación de contraseña
    }
}
