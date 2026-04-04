namespace kardex_Web.Models
{
    public sealed class Proveedor
    {
        public int Id { get; init; }
        public string? Nombre { get; set; }
        public int? Telefono { get; set; }
        public string? Direccion { get; set; }
        public bool Eliminado { get; set; }
    }
}
