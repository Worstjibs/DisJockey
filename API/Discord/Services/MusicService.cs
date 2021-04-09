using System;
using System.Linq;
using System.Threading.Tasks;
using API.Discord.Interfaces;
using API.Exceptions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace API.Discord.Services {
    public class MusicService {
        private readonly LavaNode _lavaNode;
        private readonly IServiceScopeFactory _serviceScope;
        public MusicService(LavaNode lavaNode, IServiceScopeFactory serviceScope) {
            _serviceScope = serviceScope;
            _lavaNode = lavaNode;

            _lavaNode.OnTrackEnded += OnTrackEnded;
        }

        public async Task<string> PlayTrack(string query, SocketUser user, IGuild guild) {
            var player = _lavaNode.GetPlayer(guild);

            var results = await _lavaNode.SearchYouTubeAsync(query);

            if (results.LoadStatus == LoadStatus.LoadFailed || results.LoadStatus == LoadStatus.NoMatches) {
                return "No matches found";
            }

            var track = results.Tracks.FirstOrDefault();

            string returnMessage = "";

            if (player.PlayerState == PlayerState.Playing) {
                player.Queue.Enqueue(track);
                returnMessage = $"Track {track.Title} is queued";
            } else {
                await player.PlayAsync(track);
                returnMessage = $"Track {track.Title} is now playing";
            }

            using (var scope = _serviceScope.CreateScope()) {
                try {
                    var discordTrackService = scope.ServiceProvider.GetService<IDiscordTrackService>();

                    try {
                        await discordTrackService.AddTrackAsync(user, track.Url);
                    } catch (InvalidUrlException e) {
                        returnMessage = e.ToString();
                    } catch (DataContextException e) {
                        returnMessage = e.ToString();
                    }
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }

            return returnMessage;
        }    

        private async Task OnTrackEnded(TrackEndedEventArgs args) {
            var player = args.Player;

            if (!player.Queue.TryDequeue(out var queueable)) {
                return;
            }

            if (!(queueable is LavaTrack track)) {
                await player.TextChannel.SendMessageAsync("Something wrong with the next track");
                return;
            }

            await args.Player.PlayAsync(track);
        }
    }
}