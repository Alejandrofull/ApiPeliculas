using ApiPeliculas.Controllers;
using ApiPeliculas.Models;
using ApiPeliculas.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPeliculasTests
{
    [TestFixture]
    public class PeliculasControllerTests
    {
        private Mock<IPeliculaService> _mockService;
        private PeliculasController _controller;

        [SetUp]
        public void Setup()
        {
            // ARRANGE: Simular el servicio y crear el controlador
            _mockService = new Mock<IPeliculaService>();
            _controller = new PeliculasController(_mockService.Object);
        }

        // --- PRUEBAS DEL MÉTODO POST (Crear) ---

        [Test]
        public async Task PostPelicula_DebeRetornarCreatedAtRoute_SiEsValido()
        {
            // ARRANGE
            var peliculaValida = new Pelicula { Id = 1, Titulo = "Valida" };
            // El controlador llama a Crear. Simulamos que el servicio devuelve la entidad.
            _mockService.Setup(s => s.Crear(peliculaValida)).ReturnsAsync(peliculaValida);

            // ACT
            var resultado = await _controller.PostPelicula(peliculaValida);

            // ASSERT
            Assert.That(resultado.Result, Is.InstanceOf<CreatedAtRouteResult>());
            var createdResult = resultado.Result as CreatedAtRouteResult;
            Assert.That(createdResult.StatusCode, Is.EqualTo(201)); // Código 201 Created
        }

        [Test]
        public async Task PostPelicula_DebeRetornarBadRequest_SiFallaValidacion()
        {
            // ARRANGE
            var peliculaInvalida = new Pelicula { Titulo = "" };

            // Simular que el servicio lanza la excepción de lógica de negocio (título obligatorio)
            _mockService.Setup(s => s.Crear(peliculaInvalida))
                .ThrowsAsync(new ArgumentException("El título de la película es obligatorio."));

            // ACT
            var resultado = await _controller.PostPelicula(peliculaInvalida);

            // ASSERT
            Assert.That(resultado.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = resultado.Result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400)); // Código 400 Bad Request
        }

        // --- PRUEBAS DEL MÉTODO GET ---

        [Test]
        public async Task GetPeliculas_DebeRetornarOkConLista()
        {
            // ARRANGE
            _mockService.Setup(s => s.ObtenerTodasAsync()).ReturnsAsync(new List<Pelicula> { new Pelicula() });

            // ACT
            var resultado = await _controller.GetPeliculas();

            // ASSERT
            Assert.That(resultado.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = resultado.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<Pelicula>>());
        }
    }
}