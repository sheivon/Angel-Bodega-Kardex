namespace kardex_Web.Models
{
    public sealed class Autoriza
    {
        public int Id { get; init; }
        public string? Cargo { get; set; }
        public string? NombresApellidos { get; set; }
        public bool Eliminado { get; set; }
    }
}
