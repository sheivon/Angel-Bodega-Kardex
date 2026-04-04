namespace kardex_Web.Models
{
    public sealed class Usuario
    {
        public int Id { get; init; }
        public string? Nombre { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? Contrasena { get; set; }
        public string? Cargo { get; set; }
        public string? LecturaEscritura { get; set; }
        public bool Eliminado { get; set; }
    }
}
