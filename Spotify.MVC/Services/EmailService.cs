using Spotify.APIConsumer;
using Spotify.Modelos;
using Spotify.MVC.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using MailKit;

namespace Spotify.MVC.Services
{
    public class EmailService : IEmailService // Implementación de IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // Servidor SMTP de Gmail
        private readonly int _smptPort = 587; // Puerto SMTP para TLS
        private readonly string _fromEmail = "beathousetucasa@gmail.com"; // Correo electrónico del remitente
        private readonly string _fromPassword = "miih bbhy lwox drof"; // Contraseña del correo electrónico del remitente (debe ser una contraseña de aplicación si se usa autenticación de dos factores)

        public async Task enviarEmailRecuperacionContraseña(string email)// Método para enviar un correo electrónico de recuperación de contraseña
        {
            try
            {
                Console.WriteLine("Enviando correo de recuperación...");// Mensaje de depuración para indicar que se está enviando el correo
                var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 10); // Generar una nueva contraseña temporal de 10 caracteres
                var mensaje = new MimeMessage();// Crear un nuevo mensaje MIME
                mensaje.From.Add(new MailboxAddress("BeatHouse", _fromEmail)); // Agregar el remitente al mensaje
                mensaje.To.Add(new MailboxAddress("", email)); // Agregar el destinatario al mensaje
                mensaje.Subject = "Recuperación de contraseña"; // Asunto del mensaje

                // Cuerpo del mensaje
                mensaje.Body = new TextPart("plain")// Crear el cuerpo del mensaje en texto plano
                {
                    Text = $"¡Hola!\n\nRecibimos una solicitud para recuperar tu contraseña en **BeatHouse**.\n" +
                           $"No te preocupes, hemos generado una nueva contraseña temporal para ti:\n\n" +
                           $"**{tempPassword}**\n\n" +
                           $"Te recomendamos que la cambies lo antes posible para garantizar la seguridad de tu cuenta.\n\n" +
                           "Si no solicitaste este cambio, por favor ignora este mensaje. Si tienes alguna duda, nuestro equipo " +
                           "de soporte está aquí para ayudarte.\n\n" +
                           "¡Gracias por ser parte de BeatHouse! 🎶"
                };
                using (var cliente = new SmtpClient())// Crear un nuevo cliente SMTP
                {
                    await cliente.ConnectAsync(_smtpServer, _smptPort, SecureSocketOptions.StartTls); // Conectar al servidor SMTP usando TLS
                    await cliente.AuthenticateAsync(_fromEmail, _fromPassword);// Autenticar al cliente SMTP con el correo y la contraseña del remitente
                    await cliente.SendAsync(mensaje); // Enviar el mensaje
                    await cliente.DisconnectAsync(true);// Desconectar el cliente SMTP
                }

                var usuario = CRUD<Usuario>.GetAll().FirstOrDefault(u => u.Email == email); // Buscar el usuario por email
                if (usuario != null)// Si el usuario existe, actualizar su contraseña
                {
                    usuario.Contraseña = tempPassword;// Asignar la nueva contraseña temporal al usuario
                    CRUD<Usuario>.Update(usuario.Id, usuario);// Actualizar el usuario en la base de datos
                    Console.WriteLine($"contraseña actualizada: {tempPassword}");

                }
                Console.WriteLine("Correo enviado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo electrónico: {ex.Message}");
            }
        }
    }
}
