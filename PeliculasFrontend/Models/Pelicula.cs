using System;
using System.ComponentModel.DataAnnotations;

namespace PeliculasFrontend.Models
{
    public class Pelicula
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Título es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Range(1895, 2100, ErrorMessage = "El año debe estar entre 1895 y 2100.")]
        [Required(ErrorMessage = "El campo Año es obligatorio.")]
        public int Año { get; set; }

        [StringLength(50)]
        public string Genero { get; set; } = string.Empty;
    }
}