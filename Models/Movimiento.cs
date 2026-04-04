namespace kardex_Web.Models
{
    public sealed class Movimiento
    {
        public int Id { get; init; }
        public string? TipoES { get; set; }
        public int? IdOrdCompra { get; set; }
        public int? IdProyecto { get; set; }
        public string? Detalle { get; set; }
        public DateTime? FechaES { get; set; }
        public int? IdSalida { get; set; }
        public int? IdEntrada { get; set; }
        public int? IdProducto { get; set; }
        public decimal? ECantidad { get; set; }
        public decimal? SCantidad { get; set; }
        public decimal? Precio { get; set; }
        public decimal? IVA { get; set; }
        public int? IdAreaTrabajo { get; set; }
    }
}
