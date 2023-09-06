using TunaPianoApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddNpgsql<TunaPianoApiDbContext>(builder.Configuration["TunaPianoApiDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//   SONG ENDPOINTS
app.MapGet("api/songs", async (TunaPianoApiDbContext db) =>
{
    var songsWithGenre = await db.Songs.Include("Genres").ToListAsync();

    return Results.Ok(songsWithGenre);
});
// get by id
app.MapGet("api/songs/{id}", async (TunaPianoApiDbContext db, int id) =>
{
    var song = await db.Songs
    .Include(s => s.Genres)
    .Include(s => s.Artist)
        .FirstOrDefaultAsync(s => s.Id == id);

    return Results.Ok(song);
});
// create song
app.MapPost("api/songs", async (TunaPianoApiDbContext db, Song song) =>
{
    db.Songs.Add(song);
    db.SaveChanges();
    return Results.Created($"/api/songs{song.Id}", song);
});
// update song 
app.MapPut("api/songs/{id}", async (TunaPianoApiDbContext db, int id, Song song) =>
{
    Song songToUpdate = await db.Songs.SingleOrDefaultAsync(song => song.Id == id);
    if (songToUpdate == null)
    {
        return Results.NotFound();
    }
    songToUpdate.Id = song.Id;
    songToUpdate.ArtistId = song.ArtistId;
    songToUpdate.Album = song.Album;
    songToUpdate.Length = song.Length;
    songToUpdate.Title = song.Title;
    db.SaveChanges();
    return Results.NoContent();
});
// delete song 
app.MapDelete("api/songs/{id}", (TunaPianoApiDbContext db, int id) =>
{
    Song song =  db.Songs.SingleOrDefault(song => song.Id == id);
    if (song == null)
    {
        return Results.NotFound();
    }
    db.Songs.Remove(song);
    db.SaveChanges();
    return Results.NoContent();
});
// ARTIST
app.MapGet("api/artists", async (TunaPianoApiDbContext db) =>
{
    return db.Artists;
});
// artist details
app.MapGet("api/artists/{id}", async (TunaPianoApiDbContext db, int id) =>
{
    var artist = await db.Artists
    .Include(a => a.Song)
    .ThenInclude(s => s.Genres)
    .FirstOrDefaultAsync(a => a.Id == id);
    return Results.Ok(artist);
});
// create artist 
app.MapPost("api/artists", async (TunaPianoApiDbContext db, Artist artist) =>
{
    db.Artists.Add(artist);
    db.SaveChanges();
    return Results.Created($"/api/artists{artist.Id}", artist);
});
// update artist 
app.MapPut("api/artist/{id}", async (TunaPianoApiDbContext db, int id, Artist artist) =>
{
    Artist artistToUpdate = await db.Artists.SingleOrDefaultAsync(artist => artist.Id == id);
    if (artistToUpdate == null)
    {
        return Results.NotFound();
    }
   artistToUpdate.Id = artist.Id;
   artistToUpdate.Age = artist.Age;
   artistToUpdate.Bio =artist.Bio;
   artistToUpdate.Name = artist.Name;
    db.SaveChanges();
    return Results.NoContent();
});
// delete artist 
app.MapDelete("api/artists/{id}", (TunaPianoApiDbContext db, int id) =>
{
    Artist artist = db.Artists.SingleOrDefault(artist => artist.Id == id);
    if (artist == null)
    {
        return Results.NotFound();
    }
    db.Artists.Remove(artist);
    db.SaveChanges();
    return Results.NoContent();
});
// GENRES
app.MapGet("api/genres", async (TunaPianoApiDbContext db) =>
{
    return db.Genres;
});
// genre details
app.MapGet("api/grenres/{id}", async (TunaPianoApiDbContext db, int id) =>
{
    var genre = await db.Genres
    .Include(g => g.Songs)
    .FirstOrDefaultAsync(g => g.Id == id);
    return Results.Ok(genre);
});
// create genre 
app.MapPost("api/genres", async (TunaPianoApiDbContext db, Genre genres) =>
{
    db.Genres.Add(genres);
    db.SaveChanges();
    return Results.Created($"/api/genres{genres.Id}", genres);
});
// update genre
app.MapPut("api/genres/{id}", async (TunaPianoApiDbContext db, int id, Genre genres) =>
{
    Genre genreToUpdate = await db.Genres.SingleOrDefaultAsync(genres => genres.Id == id);
    if (genreToUpdate == null)
    {
        return Results.NotFound();
    }
    genreToUpdate.Id = genres.Id;
    genreToUpdate.Name = genres.Name;
    db.SaveChanges();
    return Results.NoContent();
});
// delete artist 
app.MapDelete("api/genres/{id}", (TunaPianoApiDbContext db, int id) =>
{
    Genre genre = db.Genres.SingleOrDefault(artist => artist.Id == id);
    if (genre == null)
    {
        return Results.NotFound();
    }
    db.Genres.Remove(genre);
    db.SaveChanges();
    return Results.NoContent();
});
app.Run();
