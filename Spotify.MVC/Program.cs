using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Spotify.APIConsumer;
using Spotify.Modelos;
using Spotify.MVC.Interface;
using Spotify.MVC.Services;
using static System.Net.WebRequestMethods;

CRUD<Cancion>.EndPoint = "https://localhost:7028/api/Canciones";
CRUD<Usuario>.EndPoint = "https://localhost:7028/api/Usuarios";
CRUD<Plan>.EndPoint = "https://localhost:7028/api/Planes";
CRUD<Playlist>.EndPoint = "https://localhost:7028/api/Playlists";
CRUD<Album>.EndPoint = "https://localhost:7028/api/Albums";

var builder = WebApplication.CreateBuilder(args);

// Contexto de EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext")));

// Cache distribuido y sesi�n
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Configura el tiempo de expiraci�n de la sesi�n a 2 horas
    options.Cookie.HttpOnly = true;             // Solo accesible desde HTTP
    options.Cookie.IsEssential = true;          // Siempre se env�a
    options.Cookie.SameSite = SameSiteMode.Lax; // Pol�tica SameSite Lax para cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Seguridad de cookies dependiendo del protocolo de la solicitud
});

// Registrar HttpClient para servicios que lo necesiten
builder.Services.AddHttpClient();

// Servicios de tu aplicaci�n
builder.Services.AddScoped<IAutorizarService, AutorizarService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// MVC y autenticaci�n por cookies
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/Login/Index";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiraci�n de la cookie de autenticaci�n
    });

// Para acceder al HttpContext en servicios
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed del admin (para inicializar el usuario admin si no existe)
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!ctx.Usuarios.Any(u => u.TipoUsuario == "admin"))
    {
        ctx.Usuarios.Add(new Usuario
        {
            Email = "dylant0909@gmail.com",
            Nombre = "Dylan",
            Contrase�a = BCrypt.Net.BCrypt.HashPassword("123"),
            TipoUsuario = "admin",
            FechaRegistro = DateTime.UtcNow
        });
        ctx.SaveChanges();
    }
}

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();  // Asegura que la app siempre use HTTPS
app.UseStaticFiles();  // Habilita el acceso a archivos est�ticos (CSS, JS, im�genes, etc.)
app.UseRouting();  // Habilita el enrutamiento

// Orden correcto: sesi�n primero, luego autenticaci�n y autorizaci�n
app.UseSession();        // Habilita el uso de sesiones
app.UseAuthentication(); // Habilita la autenticaci�n por cookies
app.UseAuthorization();  // Habilita la autorizaci�n

// Configuraci�n de las rutas de los controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
