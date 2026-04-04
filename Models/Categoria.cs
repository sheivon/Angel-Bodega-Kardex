namespace kardex_Web.Models
{
    public sealed class Categoria
    {
        public int Id { get; init; }
        public string? Nombre { get; set; }
        public string? Activo { get; set; }
        public bool Eliminado { get; set; }
    }
}
