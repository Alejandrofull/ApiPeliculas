namespace ApiPeliculas.Models
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int AnioLanzamiento { get; set; }
    }
}