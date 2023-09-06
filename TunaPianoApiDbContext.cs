using Microsoft.EntityFrameworkCore;

namespace TunaPianoApi.Models
{
    public class TunaPianoApiDbContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public TunaPianoApiDbContext(DbContextOptions<TunaPianoApiDbContext> context) : base(context) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>()
                .HasMany(g => g.Genres)
                .WithMany(s => s.Songs)
                .UsingEntity(sg => sg.ToTable("SongGenre"));
            modelBuilder.Entity<Genre>().HasData(new Genre[]
            {
                new Genre { Id = 1, Name = "Rock" }, new Genre { Id= 2, Name = "Jazz"} 
            });
            modelBuilder.Entity<Artist>().HasData(new Artist[]
            {
                new Artist
                {
                    Id = 1,
                    Age = 50,
                    Bio = "This is a Bio",
                    Name = "Dude McDuderson"
                },
                new Artist
                {
                    Id = 2,
                    Age = 53,
                    Bio = "This is not a Bio",
                    Name = "Not Dude McDuderson"
                }
            });
            modelBuilder.Entity<Song>().HasData(new Song[]
            {
                new Song { Id = 1, Album = "yes", Title = "no", Length = "3:40", ArtistId = 1 }, 
                new Song { Id = 2, Album = "Maybe", Title = "possibly", Length = "2:30", ArtistId = 2}
            });

           base.OnModelCreating(modelBuilder);
        }
    }
}
