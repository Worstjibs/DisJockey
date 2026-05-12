using DisJockey.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DisJockey.Infrastructure.Persistence;

public class Seed
{
    public static async Task SeedData(DataContext context, IConfiguration configuration)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var seedData = configuration.GetSection("SeedData").Get<SeedData>();
        if (seedData is null)
        {
            return;
        }

        if (seedData.Users.Count > 0)
        {
            await context.Users.AddRangeAsync(seedData.Users);
        }

        if (seedData.Tracks.Count > 0)
        {
            await context.Tracks.AddRangeAsync(seedData.Tracks);
        }

        await context.SaveChangesAsync();

        var users = await context.Users.ToListAsync();
        var tracks = await context.Tracks.ToListAsync();

        await SeedTrackPlays(users, tracks, context);
        await SeedPlaylists(users, tracks, context);
    }

    private static async Task SeedTrackPlays(List<AppUser> users, List<Track> tracks, DataContext context)
    {
        var random = new Random();

        var dateToSet = DateTime.UtcNow;

        tracks.ForEach(track =>
        {
            track.TrackPlays = [];
            var trackPlays = (List<TrackPlay>)track.TrackPlays;

            track.PullUps = [];
            var pullUps = (List<PullUp>)track.PullUps;

            users.ForEach(user =>
            {
                var trackPlay = new TrackPlay
                {
                    AppUserId = user.Id,
                    User = user,
                    TrackId = track.Id,
                    Track = track,
                    TrackPlayHistory = [
                        new() { CreatedOn = dateToSet }
                    ],
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
            user.Playlists = [];

            for (int i = 0; i < 5; i++)
            {
                var playlist = new Playlist
                {
                    Name = $"{user.UserName}: Playlist {i}",
                    YoutubeId = $"{user.DiscordId}{i}"
                };

                playlist.Tracks = [.. tracks.Select(t => new PlaylistTrack
                {
                    Playlist = playlist,
                    PlaylistId = playlist.Id,
                    Track = t,
                    TrackId = t.Id,
                    CreatedBy = user,
                    CreatedOn = DateTime.UtcNow
                })];

                user.Playlists.Add(playlist);
            }
        });

        await context.SaveChangesAsync();
    }
}