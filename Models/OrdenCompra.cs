namespace kardex_Web.Models
{
    public sealed class OrdenCompra
    {
        public int Id { get; init; }
        public DateTime? FechaOrdCompra { get; set; }
        public string? Numero { get; set; }
        public int? IdProyecto { get; set; }
        public string? TipoEntrada { get; set; }
        public string? NoProceso { get; set; }
        public string? Estado { get; set; }
        public string? Concepto { get; set; }
        public bool Eliminado { get; set; }
    }
}
