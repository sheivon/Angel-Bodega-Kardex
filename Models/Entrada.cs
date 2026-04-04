namespace kardex_Web.Models
{
    public sealed class Entrada
    {
        public int Id { get; init; }
        public int? NumeroE { get; set; }
        public DateTime? Fecha { get; set; }
        public int? IdOrdCompra { get; set; }
        public int? IdProveedor { get; set; }
        public DateTime? FechaFactura { get; set; }
        public string? NoFactura { get; set; }
        public string? Estado { get; set; }
        public string? NoCK { get; set; }
        public string? NombreAutoriza { get; set; }
        public string? NombreRegistra { get; set; }
    }
}
