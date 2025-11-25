using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculasTests
{
    [TestFixture]
    public class PeliculaRepositoryTests
    {
        private AppDbContext _context;
        private PeliculaRepository _repository;

        [SetUp]
        public void Setup()
        {
            // ARRANGE: Configuración de la DB en memoria (simulada)
            // Usamos Guid.NewGuid().ToString() para asegurar una DB limpia por cada test.
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new PeliculaRepository(_context);

            // Poblar algunos datos para las pruebas
            _context.Peliculas.Add(new Pelicula { Id = 1, Titulo = "Matrix", Director = "Wachowski", AnioLanzamiento = 1999 });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // -----------------------------------------------------------
        //              PRUEBAS DE LECTURA (GET)
        // -----------------------------------------------------------

        [Test]
        public async Task GetAllAsync_DebeRetornarTodasLasPeliculas()
        {
            // ACT
            var resultado = await _repository.GetAllAsync();

            // ASSERT
            Assert.That(resultado.Count(), Is.EqualTo(1));
            Assert.That(resultado.First().Titulo, Is.EqualTo("Matrix"));
        }

        [Test]
        public async Task GetByIdAsync_DebeRetornarPelicula_SiExiste()
        {
            // ACT
            var resultado = await _repository.GetByIdAsync(1);

            // ASSERT
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_DebeRetornarNull_SiNoExiste()
        {
            // ACT
            var resultado = await _repository.GetByIdAsync(99);

            // ASSERT
            Assert.That(resultado, Is.Null);
        }

        // -----------------------------------------------------------
        //              PRUEBAS DE CREACIÓN (ADD)
        // -----------------------------------------------------------

        [Test]
        public async Task AddAsync_DebeIncrementarElConteo()
        {
            // ARRANGE
            var nuevaPelicula = new Pelicula { Id = 2, Titulo = "Avatar", Director = "Cameron", AnioLanzamiento = 2009 };

            // ACT
            await _repository.AddAsync(nuevaPelicula);

            // ASSERT
            var conteo = await _context.Peliculas.CountAsync();
            Assert.That(conteo, Is.EqualTo(2));
        }

        // -----------------------------------------------------------
        //              PRUEBAS DE ACTUALIZACIÓN (UPDATE)
        // -----------------------------------------------------------

        [Test]
        public async Task UpdateAsync_DebeActualizarPelicula_Exitoso()
        {
            // ARRANGE
            var peliculaAActualizar = await _repository.GetByIdAsync(1);
            peliculaAActualizar.Titulo = "The Matrix Reloaded"; // Cambiamos el título

            // ACT
            var resultado = await _repository.UpdateAsync(peliculaAActualizar);

            // ASSERT
            Assert.That(resultado, Is.True);
            var peliculaVerificada = await _repository.GetByIdAsync(1);
            Assert.That(peliculaVerificada.Titulo, Is.EqualTo("The Matrix Reloaded"));
        }

        // -----------------------------------------------------------
        //              PRUEBAS DE ELIMINACIÓN (DELETE)
        // -----------------------------------------------------------

        [Test]
        public async Task DeleteAsync_DebeRetornarTrue_SiPeliculaExiste()
        {
            // ACT
            var resultado = await _repository.DeleteAsync(1);

            // ASSERT
            Assert.That(resultado, Is.True);
            var peliculaEliminada = await _repository.GetByIdAsync(1);
            Assert.That(peliculaEliminada, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_DebeRetornarFalse_SiPeliculaNoExiste()
        {
            // ACT
            var resultado = await _repository.DeleteAsync(99); // ID que no existe

            // ASSERT
            Assert.That(resultado, Is.False);
            var conteo = await _context.Peliculas.CountAsync();
            Assert.That(conteo, Is.EqualTo(1)); // El conteo debe seguir siendo 1 (Matrix)
        }
    }
}