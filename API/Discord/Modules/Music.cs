using System;
using System.Linq;
using System.Threading.Tasks;
using API.Discord.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
using Victoria.Enums;

namespace API.Discord.Modules {
    public class Music : ModuleBase<SocketCommandContext> {
        private readonly LavaNode _lavaNode;
        private readonly MusicService _musicService;
        public Music(LavaNode lavaNode, MusicService musicService) {
            _musicService = musicService;
            _lavaNode = lavaNode;
        }

        [Command("Join")]
        public async Task JoinAsync() {
            var user = Context.User as IVoiceState;

            if (_lavaNode.HasPlayer(Context.Guild)) {
                await ReplyAsync("I'm already conntected your voice channel!");
                return;
            }

            if (user?.VoiceChannel is null) {
                await ReplyAsync("You need to connect to a voice channel first");
                return;
            }

            try {
                await _lavaNode.JoinAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {user.VoiceChannel.Name}");
            } catch (Exception e) {
                await ReplyAsync(e.Message);
            }
        }

        [Command("Leave")]
        public async Task LeaveAsync() {
            if (!_lavaNode.HasPlayer(Context.Guild)) {
                await ReplyAsync("I'm not connected to a channel");
                return;
            }

            var voiceChannel = _lavaNode.GetPlayer(Context.Guild).VoiceChannel;
            await _lavaNode.LeaveAsync(voiceChannel);
        }

        [Command("Play")]
        public async Task Play([Remainder] string query) {
            var user = Context.User as IVoiceState;
            if (user?.VoiceChannel is null) {
                await ReplyAsync("You need to connect to a voice channel first");
                return;
            }

            // var player = _lavaNode.GetPlayer(Context.Guild);

            // var results = await _lavaNode.SearchYouTubeAsync(query);

            // if (results.LoadStatus == LoadStatus.LoadFailed || results.LoadStatus == LoadStatus.NoMatches) {
            //     await ReplyAsync("No matches found");
            // }

            // var track = results.Tracks.FirstOrDefault();

            // if (player.PlayerState == PlayerState.Playing) {
            //     player.Queue.Enqueue(track);
            //     await ReplyAsync($"Track {track.Title} is queued");
            // } else {
            //     await player.PlayAsync(track);
            //     await ReplyAsync($"Track {track.Title} is now playing");
            // }

            var userId = Context.User.Id;
            var username = Context.User.Username;

            var response = await _musicService.PlayTrack(query, userId, username, Context.Guild);
            await ReplyAsync(response);
        }

        [Command("Skip")]
        public async Task Skip() {
            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState != PlayerState.Playing) {
                await ReplyAsync("I am not playing anything");
                return;
            }

            await player.SkipAsync();
        }

        [Command("Stop")]
        public async Task Stop() {
            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState != PlayerState.Playing) {
                await ReplyAsync("I am not playing anything");
                return;
            }

            await player.StopAsync();
        }
    }
}