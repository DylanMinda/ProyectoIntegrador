namespace Spotify.MVC.Interface
{
    public interface IAutorizarService
    {
        Task<bool> Login(string email, string contraseña);// METODO Login para autenticar un usuario
        Task<bool> Register(string email, string nombre, string contraseña);// metodo Register para registrar un nuevo usuario
    }
}
