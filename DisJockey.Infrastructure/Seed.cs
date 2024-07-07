using System.Text.Json;
using DisJockey.Core;
using Microsoft.EntityFrameworkCore;

namespace DisJockey.Infrastructure;

public class Seed
{
    public static async Task SeedData(DataContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var filePath = AppDomain.CurrentDomain.BaseDirectory + "SeedData.json";

        if (File.Exists(filePath))
        {
            var seedDataString = await System.IO.File.ReadAllTextAsync(filePath);

            var json = JsonSerializer.Deserialize<SeedData>(seedDataString);

            await context.Users.AddRangeAsync(json.Users);
            await context.Tracks.AddRangeAsync(json.Tracks);

            await context.SaveChangesAsync();

            var users = await context.Users.ToListAsync();
            var tracks = await context.Tracks.ToListAsync();

            await SeedTrackPlays(users, tracks, context);

            await SeedPlaylists(users, tracks, context);
        }
    }

    private static async Task SeedTrackPlays(List<AppUser> users, List<Track> tracks, DataContext context)
    {
        var random = new Random();

        var dateToSet = DateTime.UtcNow;

        tracks.ForEach(track =>
        {
            track.TrackPlays = new List<TrackPlay>();
            var trackPlays = (List<TrackPlay>)track.TrackPlays;

            track.PullUps = new List<PullUp>();
            var pullUps = (List<PullUp>)track.PullUps;

            users.ForEach(user =>
            {
                var trackPlay = new TrackPlay
                {
                    AppUserId = user.Id,
                    User = user,
                    TrackId = track.Id,
                    Track = track,
                    TrackPlayHistory = new List<TrackPlayHistory>() {
                            new TrackPlayHistory {
                                CreatedOn = dateToSet
                            }
                        },
                    LastPlayed = dateToSet
                };

                track.TrackPlays.Add(trackPlay);
                track.CreatedOn = dateToSet;

                var pullUp = new PullUp
                {
                    UserId = user.Id,
                    User = user,
                    TrackId = track.Id,
                    Track = track,
                    CreatedOn = dateToSet,
                    TimePulled = random.NextDouble() * 60
                };

                pullUps.Add(pullUp);

                dateToSet = dateToSet.AddDays(-1);
            });

        });

        await context.SaveChangesAsync();
    }

    private static async Task SeedPlaylists(List<AppUser> users, List<Track> tracks, DataContext context)
    {
        users.ForEach(user =>
        {
            user.Playlists = new List<Playlist>();

            for (int i = 0; i < 5; i++)
            {
                var playlist = new Playlist
                {
                    Name = $"{user.UserName}: Playlist {i}",
                    YoutubeId = $"{user.DiscordId}{i}"
                };

                playlist.Tracks = tracks.Select(t => new PlaylistTrack
                {
                    Playlist = playlist,
                    PlaylistId = playlist.Id,
                    Track = t,
                    TrackId = t.Id,
                    CreatedBy = user,
                    CreatedOn = DateTime.UtcNow
                }).ToList();

                user.Playlists.Add(playlist);
            }
        });

        await context.SaveChangesAsync();
    }
}