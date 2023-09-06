namespace TunaPianoApi.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public int Age { get; set; }
        public List<Song>? Song { get; set; }
    }
}
