namespace kardex_Web.Models
{
    public sealed class Periodo
    {
        public int Id { get; init; }
        public int? Anio { get; set; }
        public bool Eliminado { get; set; }
    }
}
