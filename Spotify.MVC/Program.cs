using Microsoft.EntityFrameworkCore;
using Spotify.APIConsumer;
using Spotify.Modelos;
using Spotify.MVC.Interface;
using Spotify.MVC.Services;
using static System.Net.WebRequestMethods;

<<<<<<< Updated upstream
// Configurar los endpoints para el CRUD

CRUD<Album>.EndPoint = "https://localhost:7028/api/Albums";
    CRUD<Cancion>.EndPoint = "https://localhost:7028/api/Canciones";
    CRUD<Usuario>.EndPoint = "https://localhost:7028/api/Usuarios"; 
    CRUD<Plan>.EndPoint = "https://localhost:7028/api/Planes"; 
    CRUD<Playlist>.EndPoint = "https://localhost:7028/api/Playlists"; 
=======
CRUD<Cancion>.EndPoint = "https://localhost:7028/api/Canciones";
CRUD<Usuario>.EndPoint = "https://localhost:7028/api/Usuarios";
CRUD<Plan>.EndPoint = "https://localhost:7028/api/Planes";
CRUD<Playlist>.EndPoint = "https://localhost:7028/api/Playlists";
>>>>>>> Stashed changes

var builder = WebApplication.CreateBuilder(args);

// Contexto de EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext")));

<<<<<<< Updated upstream

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext")));

// Configurar sesiones
=======
// Cache distribuido y sesión
builder.Services.AddDistributedMemoryCache();
>>>>>>> Stashed changes
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;                       // Solo accesible desde HTTP
    options.Cookie.IsEssential = true;                       // Siempre se envía
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Servicios de tu aplicación
builder.Services.AddScoped<IAutorizarService, AutorizarService>();
builder.Services.AddScoped<IEmailService, EmailService>();
<<<<<<< Updated upstream

// Add services to the container.
builder.Services.AddControllersWithViews();

    builder.Services.AddAuthentication("Cookies") //cokies
        .AddCookie("Cookies", options =>
        {
            options.LoginPath = "/Login/Index"; // Ruta de inicio de sesión


        });
    builder.Services.AddHttpContextAccessor(); // Para acceder al contexto HTTP en los servicios //cokies



var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
=======

// MVC y autenticación por cookies
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/Login/Index";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Para acceder al HttpContext en servicios
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed del admin
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!ctx.Usuarios.Any(u => u.TipoUsuario == "admin"))
    {
        ctx.Usuarios.Add(new Usuario
        {
            Email = "dylant0909@gmail.com",
            Nombre = "Dylan",
            Contraseña = BCrypt.Net.BCrypt.HashPassword("123"),
            TipoUsuario = "admin",
            FechaRegistro = DateTime.UtcNow
        });
        ctx.SaveChanges();
>>>>>>> Stashed changes
    }
}

<<<<<<< Updated upstream
app.UseSession();
app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseAuthentication(); // Habilitar autenticación antes de usar routing//cookies

    app.UseRouting();

    app.UseAuthorization();//cokies

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"); 

    app.Run();


=======
// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Orden correcto: sesión primero, luego auth
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
>>>>>>> Stashed changes
