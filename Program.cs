using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using BoticaOnline.Data;
using Microsoft.AspNetCore.Mvc;
using BoticaOnline.Models; 

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuración de Servicios (Dependency Injection) ---

// a. Configuración de la Conexión a MySQL con EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

// b. Soporte para API RESTful (Controladores) y Front-end (Razor Pages)
// ¡CRUCIAL! Esto registra los controladores de la API.
builder.Services.AddControllers(); 
builder.Services.AddRazorPages();

// c. Soporte para Swagger (Opcional, pero útil para probar la API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// d. CONFIGURACIÓN DE CORS (Cross-Origin Resource Sharing)
// Esto permite que un frontend externo (como una app React) acceda a tu API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        corsBuilder =>
        {
            corsBuilder.AllowAnyOrigin() // Permite solicitudes desde CUALQUIER dominio
                       .AllowAnyMethod() // Permite métodos GET, POST, PUT, DELETE, etc.
                       .AllowAnyHeader(); // Permite cualquier encabezado en la petición
        });
});

// --- 2. Configuración del Pipeline HTTP ---

var app = builder.Build();

// Configura el Pipeline HTTP (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Middleware para servir archivos estáticos (CSS, JS, imágenes de wwwroot)
app.UseStaticFiles(); 

app.UseRouting();

// ¡CRUCIAL! Aplica la política CORS ANTES de UseAuthorization
app.UseCors("AllowAnyOrigin");

app.UseAuthorization();

// 1. Mapea las páginas de Razor (el Front-end)
app.MapRazorPages(); 

// 2. Mapea los controladores (la API)
app.MapControllers(); 

app.Run();