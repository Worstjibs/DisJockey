using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;

namespace API.Data {
    public class Seed {
        public static async Task SeedData(DataContext context) {
            if (await context.Users.AnyAsync()) {
                return;
            }

            if (File.Exists("Data/SeedData.json")) {
                var seedDataString = await System.IO.File.ReadAllTextAsync("Data/SeedData.json");

                var json = JsonSerializer.Deserialize<SeedData>(seedDataString);

                await context.Users.AddRangeAsync(json.Users);
                await context.Tracks.AddRangeAsync(json.Tracks);

                await context.SaveChangesAsync();

                var users = await context.Users.ToListAsync();
                var tracks = await context.Tracks.ToListAsync();

                var dateToSet = DateTime.UtcNow;

                tracks.ForEach(track => {
                    track.TrackPlays = new List<TrackPlay>();
                    var trackPlays = (List<TrackPlay>)track.TrackPlays;

                    users.ForEach(user => {
                        var trackPlay = new TrackPlay {
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

                        dateToSet = dateToSet.AddDays(-1);
                    });

                });

                await context.SaveChangesAsync();

                users.ForEach(user => {
                    user.Playlists = new List<Playlist>();

                    for (int i = 0; i < 5; i++) {
                        var playlist = new Playlist {
                            Name = $"{user.UserName}: Playlist {i}",
                            YoutubeId = $"{user.DiscordId}{i}"
                        };

                        playlist.Tracks = tracks.Select(t => new PlaylistTrack {
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
    }
}