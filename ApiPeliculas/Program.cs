using ApiPeliculas.Data; // Necesario para AppDbContext
using ApiPeliculas.Repositories; // Necesario para IPeliculaRepository y PeliculaRepository
using ApiPeliculas.Service; // Contiene IPeliculaService y PeliculaService
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Necesario para ILogger

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1. CONFIGURACIÓN DE SQL SERVER Y DB CONTEXT
// =======================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// =======================================================
// 2. CONFIGURACIÓN DE LA INYECCIÓN DE DEPENDENCIAS (DI)
// =======================================================
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
// ⭐ CORRECCIÓN CLAVE: Mapear la INTERFAZ (IPeliculaService) a la IMPLEMENTACIÓN (PeliculaService)
builder.Services.AddScoped<IPeliculaService, PeliculaService>();

// =======================================================
// ⭐ 3. CONFIGURACIÓN CORS (CRUCIAL para el Frontend Blazor) ⭐
// =======================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        // Aquí se autoriza a tu frontend Blazor (https://localhost:7195) a hacer peticiones.
        // Si tu frontend tiene otra URL HTTP, deberías añadirla aquí también.
        builder => builder.WithOrigins("https://localhost:7195")
                          .AllowAnyMethod() // Permite GET, POST, PUT, DELETE
                          .AllowAnyHeader()); // Permite cualquier encabezado HTTP
});


// =======================================================
// 4. CONFIGURACIÓN DE SWAGGER Y CONTROLADORES
// =======================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =======================================================
// 5. CREAR BASE DE DATOS SI NO EXISTE
// =======================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    try
    {
        // Crea la base de datos y el esquema si no existen.
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        // Nota: Agregué ILogger<Program> para manejar el error.
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al crear la base de datos.");
    }
}

// =======================================================
// 6. CONFIGURACIÓN DEL PIPELINE
// =======================================================
// Aplicar CORS aquí antes de UseAuthorization/MapControllers
app.UseCors("CorsPolicy");

// Swagger siempre activo (Incluso en producción si lo deseas, aunque es mejor condicionarlo)
app.UseSwagger();
app.UseSwaggerUI(c => // Corregido: removí el 'app.' redundante aquí
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "🎬 API Películas CRUD V1");
    c.RoutePrefix = "swagger";
});


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();