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

                users.ForEach(user => {
                    user.Tracks = new List<TrackPlay>();
                    var userTracks = (List<TrackPlay>)user.Tracks;

                    userTracks.AddRange(tracks.Select(y => new TrackPlay {
                        AppUserId = user.Id,
                        User = user,
                        TrackId = y.Id,
                        Track = y,
                        TrackPlayHistory = new List<TrackPlayHistory>()
                    }));

                    userTracks.ForEach(tp => tp.TrackPlayHistory.Add(new TrackPlayHistory {
                        CreatedOn = DateTime.UtcNow
                    }));                
                });

                await context.SaveChangesAsync();
            }
            
        }
    }
}