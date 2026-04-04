namespace kardex_Web.Models
{
    public sealed class Asignar
    {
        public int Id { get; init; }
        public int? IdActivo { get; set; }
        public DateTime? Fecha { get; set; }
        public int? IdRetira { get; set; }
        public string? NombresApellidos { get; set; }
        public bool Eliminado { get; set; }
    }
}
