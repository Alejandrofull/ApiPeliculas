using ApiPeliculas.Data; // Necesario para AppDbContext
using ApiPeliculas.Repositories; // Necesario para IPeliculaRepository y PeliculaRepository
using ApiPeliculas.Service;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<IPeliculaService, PeliculaService>();

// =======================================================
// 3. CONFIGURACIÓN DE SWAGGER Y CONTROLADORES
// =======================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =======================================================
// ⭐ 4. CREAR BASE DE DATOS SI NO EXISTE (SIN INSERTAR DATOS)
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
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al crear la base de datos.");
    }
}

// =======================================================
// 5. CONFIGURACIÓN DEL PIPELINE — SWAGGER SIEMPRE ACTIVO
// =======================================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "🎬 API Películas CRUD V1");
    c.RoutePrefix = "swagger";
});


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
