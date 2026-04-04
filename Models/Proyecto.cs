namespace kardex_Web.Models
{
    public sealed class Proyecto
    {
        public int Id { get; init; }
        public string? Nombre { get; set; }
        public int? Anio { get; set; }
        public bool Eliminado { get; set; }
    }
}
