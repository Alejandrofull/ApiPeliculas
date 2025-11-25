using ApiPeliculas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPeliculas.Service
{
    public interface IPeliculaService
    {
        Task<IEnumerable<Pelicula>> ObtenerTodasAsync();
        Task<Pelicula?> ObtenerPorIdAsync(int id);

        // ⭐ Método necesario para tus pruebas unitarias.
        Task<Pelicula> Crear(Pelicula pelicula);

        // Métodos originales.
        Task AgregarAsync(Pelicula pelicula);
        Task<bool> ActualizarAsync(Pelicula pelicula);
        Task<bool> EliminarAsync(int id);
    }
}