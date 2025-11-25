using ApiPeliculas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Esta propiedad mapea a la tabla 'Peliculas' en la base de datos
        public DbSet<Pelicula> Peliculas { get; set; } = null!;
    }
}