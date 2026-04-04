namespace kardex_Web.Models
{
    public sealed class Retira
    {
        public int Id { get; init; }
        public string? Cedula { get; set; }
        public string? NombresApellidos { get; set; }
        public string? Cargo { get; set; }
        public string? AyudaSocial { get; set; }
        public bool Eliminado { get; set; }
    }
}
