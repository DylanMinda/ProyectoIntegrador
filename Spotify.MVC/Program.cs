using Spotify.APIConsumer;
using Spotify.Modelos;
using Spotify.MVC.Interface;
using Spotify.MVC.Services;
using static System.Net.WebRequestMethods;

CRUD<Cancion>.EndPoint = "https://localhost:7028/api/Canciones";
CRUD<Usuario>.EndPoint = "https://localhost:7028/api/Usuarios"; 
CRUD<Plan>.EndPoint = "https://localhost:7028/api/Planes"; 
//CRUD<Suscripcion>.EndPoint = "https://localhost:7028/api/Suscripciones"; 


var builder = WebApplication.CreateBuilder(args);

//Registrar Servicios
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddScoped<IMetodoPagoService, MetodoPagoService>();
//builder.Services.AddScoped<ISuscripcionService, SuscripcionService>();
//builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IAutorizarService, AutorizarService>();
builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<IPerfilService, PerfilService>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication(); // Habilitar autenticación antes de usar routing//cookies

app.UseRouting();

app.UseAuthorization();//cokies

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


