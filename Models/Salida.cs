namespace kardex_Web.Models
{
    public sealed class Salida
    {
        public int Id { get; init; }
        public int? NumeroS { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Ayuda { get; set; }
        public int? IdAutoriza { get; set; }
        public int? IdRetira { get; set; }
        public string? Estado { get; set; }
        public string? NombreRegistra { get; set; }
        public string? Concepto { get; set; }
        public bool Eliminado { get; set; }
    }
}
