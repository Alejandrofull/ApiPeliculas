using ApiPeliculas.Data;
using ApiPeliculas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositories
{
    public class PeliculaRepository : IPeliculaRepository
    {
        private readonly AppDbContext _context;

        public PeliculaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pelicula>> GetAllAsync()
        {
            return await _context.Peliculas.ToListAsync();
        }

        public async Task<Pelicula?> GetByIdAsync(int id)
        {
            return await _context.Peliculas.FindAsync(id);
        }

        public async Task AddAsync(Pelicula pelicula)
        {
            _context.Peliculas.Add(pelicula);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Pelicula pelicula)
        {
            // Marca la entidad como modificada
            _context.Entry(pelicula).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Verifica si la entidad realmente existe antes de fallar
                if (!_context.Peliculas.Any(e => e.Id == pelicula.Id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pelicula = await GetByIdAsync(id);
            if (pelicula == null)
            {
                return false;
            }

            _context.Peliculas.Remove(pelicula);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}