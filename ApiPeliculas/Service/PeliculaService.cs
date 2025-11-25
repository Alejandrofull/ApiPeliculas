using ApiPeliculas.Models;
using ApiPeliculas.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Service
{
    public class PeliculaService : IPeliculaService
    {
        private readonly IPeliculaRepository _repository;

        public PeliculaService(IPeliculaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Pelicula>> ObtenerTodasAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Pelicula?> ObtenerPorIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // ⭐ IMPLEMENTACIÓN DE CREAR (EL MÉTODO CENTRAL CON LÓGICA) ⭐
        public async Task<Pelicula> Crear(Pelicula nuevaPelicula)
        {
            // 1. VALIDACIÓN: Título obligatorio (ArgumentException)
            if (string.IsNullOrWhiteSpace(nuevaPelicula.Titulo))
            {
                throw new ArgumentException("El título de la película es obligatorio.");
            }

            // 2. VALIDACIÓN: Año futuro (ArgumentException)
            if (nuevaPelicula.AnioLanzamiento > DateTime.Now.Year)
            {
                throw new ArgumentException("El año de lanzamiento no puede ser futuro.");
            }

            // 3. VALIDACIÓN: Duplicado (InvalidOperationException)
            var peliculasExistentes = await _repository.GetAllAsync();
            var duplicado = peliculasExistentes.Any(p =>
                p.Titulo.Equals(nuevaPelicula.Titulo, StringComparison.OrdinalIgnoreCase) &&
                p.Director.Equals(nuevaPelicula.Director, StringComparison.OrdinalIgnoreCase));

            if (duplicado)
            {
                throw new InvalidOperationException("Ya existe una película con el mismo título y director.");
            }

            // Llama al repositorio (que retorna Task/void)
            await _repository.AddAsync(nuevaPelicula);

            // Retorna la entidad (asumiendo que el ID es asignado por el contexto real o simulado)
            return nuevaPelicula;
        }
        // ⭐ FIN DE CREAR ⭐

        // Método original (mantener para la interfaz, aunque el controlador ahora usa Crear)
        public async Task AgregarAsync(Pelicula pelicula)
        {
            if (pelicula.AnioLanzamiento > DateTime.Now.Year)
            {
                throw new ArgumentException("El año de lanzamiento no puede ser futuro.");
            }
            // Nota: Este método no tiene la validación de duplicados.
            await _repository.AddAsync(pelicula);
        }

        public async Task<bool> ActualizarAsync(Pelicula pelicula)
        {
            return await _repository.UpdateAsync(pelicula);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}