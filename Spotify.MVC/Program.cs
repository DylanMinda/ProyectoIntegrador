using Microsoft.EntityFrameworkCore;
using Spotify.APIConsumer;
using Spotify.Modelos;
using Spotify.MVC.Interface;
using Spotify.MVC.Services;
using static System.Net.WebRequestMethods;

// Configurar los endpoints para el CRUD

CRUD<Album>.EndPoint = "https://localhost:7028/api/Albums";
    CRUD<Cancion>.EndPoint = "https://localhost:7028/api/Canciones";
    CRUD<Usuario>.EndPoint = "https://localhost:7028/api/Usuarios"; 
    CRUD<Plan>.EndPoint = "https://localhost:7028/api/Planes"; 
    CRUD<Playlist>.EndPoint = "https://localhost:7028/api/Playlists"; 


    var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext")));

// Configurar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Tiempo de expiración de la sesión
});

builder.Services.AddScoped<IAutorizarService, AutorizarService>();
builder.Services.AddScoped<IEmailService, EmailService>();

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
    }

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


