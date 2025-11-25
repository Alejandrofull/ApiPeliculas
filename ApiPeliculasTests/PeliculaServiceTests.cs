using ApiPeliculas.Models;
using ApiPeliculas.Repositories;
using ApiPeliculas.Service;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculasTests
{
    [TestFixture]
    public class PeliculaServiceTests
    {
        private Mock<IPeliculaRepository> _repoMock;
        private PeliculaService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IPeliculaRepository>();
            _service = new PeliculaService(_repoMock.Object);
        }

        // --- PRUEBAS DEL MÉTODO CREAR ---

        [Test]
        public async Task Crear_DeberiaCrearPelicula_Exitoso()
        {
            // ARRANGE: Simular que no hay duplicados y que AddAsync termina.
            var nuevaPelicula = new Pelicula { Id = 1, Titulo = "Duna", Director = "Denis Villeneuve", AnioLanzamiento = 2021 };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Pelicula>());
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Pelicula>())).Returns(Task.CompletedTask);

            // ACT
            var resultado = await _service.Crear(nuevaPelicula);

            // ASSERT
            Assert.That(resultado.Titulo, Is.EqualTo("Duna"));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Pelicula>()), Times.Once); // Verificar que llamó al repo
        }

        [Test]
        public void Crear_DeberiaLanzarExcepcion_CuandoTituloEsVacio()
        {
            // ARRANGE
            var peliculaInvalida = new Pelicula { Titulo = "", AnioLanzamiento = 2020 };

            // ACT & ASSERT
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.Crear(peliculaInvalida));
            Assert.That(ex.Message, Is.EqualTo("El título de la película es obligatorio."));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Pelicula>()), Times.Never); // No debe llamar al repo
        }

        [Test]
        public void Crear_DeberiaLanzarExcepcion_CuandoExisteDuplicado()
        {
            // ARRANGE
            var existente = new Pelicula { Titulo = "Inter", Director = "C. Nolan" };
            var duplicado = new Pelicula { Titulo = "Inter", Director = "C. Nolan" };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Pelicula> { existente });

            // ACT & ASSERT
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.Crear(duplicado));
            Assert.That(ex.Message, Is.EqualTo("Ya existe una película con el mismo título y director."));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Pelicula>()), Times.Never);
        }

        // --- PRUEBAS DEL MÉTODO ACTUALIZAR (Añadidas para Cobertura) ---

        [Test]
        public async Task ActualizarAsync_DebeRetornarTrue_Exitoso()
        {
            // ARRANGE
            var peliculaActualizada = new Pelicula { Id = 1, Titulo = "Actualizado", Director = "Dir" };

            // Simular que el repositorio retorna True al actualizar
            _repoMock.Setup(r => r.UpdateAsync(peliculaActualizada)).ReturnsAsync(true);

            // ACT
            var resultado = await _service.ActualizarAsync(peliculaActualizada);

            // ASSERT
            Assert.That(resultado, Is.True);
            // Verificar que se llamó al repositorio para actualizar
            _repoMock.Verify(r => r.UpdateAsync(peliculaActualizada), Times.Once);
        }

       
        // --- OTRAS PRUEBAS CRUD PARA COBERTURA ---

        [Test]
        public async Task ObtenerTodasAsync_DebeLlamarAlRepo()
        {
            // ARRANGE
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Pelicula>());
            // ACT
            await _service.ObtenerTodasAsync();
            // ASSERT
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task EliminarAsync_DebeRetornarTrue()
        {
            // ARRANGE
            _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);
            // ACT
            var resultado = await _service.EliminarAsync(1);
            // ASSERT
            Assert.That(resultado, Is.True);
            _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }
    }
}