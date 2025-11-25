using ApiPeliculas.Models;
using ApiPeliculas.Service;
using Microsoft.AspNetCore.Mvc;
using System; // Necesario para ArgumentException/InvalidOperationException
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaService _service;

        public PeliculasController(IPeliculaService service)
        {
            _service = service;
        }

        // GET: api/Peliculas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pelicula>>> GetPeliculas()
        {
            var peliculas = await _service.ObtenerTodasAsync();
            return Ok(peliculas);
        }

        // GET: api/Peliculas/5
        [HttpGet("{id}", Name = "GetPelicula")] // ⭐ Name es necesario para CreatedAtRoute ⭐
        public async Task<ActionResult<Pelicula>> GetPelicula(int id)
        {
            var pelicula = await _service.ObtenerPorIdAsync(id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return Ok(pelicula);
        }

        // POST: api/Peliculas
        [HttpPost]
        public async Task<ActionResult<Pelicula>> PostPelicula(Pelicula pelicula)
        {
            try
            {
                // ⭐ CAMBIO CLAVE: Usamos Crear() que retorna la entidad y tiene TODAS las validaciones.
                var peliculaCreada = await _service.Crear(pelicula);

                // HTTP 201 Created: Usamos CreatedAtRoute y el objeto retornado.
                return CreatedAtRoute(nameof(GetPelicula), new { id = peliculaCreada.Id }, peliculaCreada);
            }
            catch (ArgumentException ex) // Maneja título vacío o año futuro
            {
                return BadRequest(ex.Message); // HTTP 400
            }
            catch (InvalidOperationException ex) // ⭐ AGREGADO: Maneja duplicados
            {
                return BadRequest(ex.Message); // HTTP 400
            }
        }

        // PUT: api/Peliculas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPelicula(int id, Pelicula pelicula)
        {
            if (id != pelicula.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }

            var success = await _service.ActualizarAsync(pelicula);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Peliculas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePelicula(int id)
        {
            var success = await _service.EliminarAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}