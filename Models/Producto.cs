namespace kardex_Web.Models
{
    public sealed class Producto
    {
        public int Id { get; init; }
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdUM { get; set; }
    }
}
