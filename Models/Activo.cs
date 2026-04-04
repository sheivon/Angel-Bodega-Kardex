namespace kardex_Web.Models
{
    public sealed class Activo
    {
        public int Id { get; init; }
        public DateTime? Fecha { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdProductos { get; set; }
        public string? CodigoContable { get; set; }
        public string? NoSerie { get; set; }
        public string? Modelo { get; set; }
        public string? Marca { get; set; }
        public string? Nombre { get; set; }
        public decimal? Precio { get; set; }
        public decimal? IVA { get; set; }
        public decimal? CTotal { get; set; }
        public int? IdMovimientos { get; set; }
        public int? IdSalida { get; set; }
        public int? IdEntrada { get; set; }
        public int? NoFactura { get; set; }
        public DateTime? FechaFactura { get; set; }
    }
}
