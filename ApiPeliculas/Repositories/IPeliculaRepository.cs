using ApiPeliculas.Models;

namespace ApiPeliculas.Repositories
{
    public interface IPeliculaRepository
    {
        Task<IEnumerable<Pelicula>> GetAllAsync();
        Task<Pelicula?> GetByIdAsync(int id);
        Task AddAsync(Pelicula pelicula);
        Task<bool> UpdateAsync(Pelicula pelicula);
        Task<bool> DeleteAsync(int id);
    }
}