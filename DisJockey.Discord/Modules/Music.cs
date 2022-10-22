using System;
using System.Threading.Tasks;
using DisJockey.Discord.Services;
using Discord;
using Discord.Commands;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace DisJockey.Discord.Modules {
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

            var response = await _musicService.PlayTrack(query, Context.User, Context.Guild, false, SearchType.YouTube);
            await ReplyAsync(response);
        }

        [Command("Playsc")]
        public async Task PlaySoundcloud([Remainder] string query)
        {
            var user = Context.User as IVoiceState;
            if (user?.VoiceChannel is null)
            {
                await ReplyAsync("You need to connect to a voice channel first");
                return;
            }

            var response = await _musicService.PlayTrack(query, Context.User, Context.Guild, false, SearchType.SoundCloud);
            await ReplyAsync(response);
        }

        [Command("Skip")]
        public async Task SkipAsync() {
            var player = TryGetCurrentPlayer();

            if (player.PlayerState != PlayerState.Playing) {
                await ReplyAsync("I am not playing anything");
                return;
            }

            if (!player.Queue.TryDequeue(out var queueable)) {                
                await ReplyAsync("There is nothing in the queue");
                return;
            }
            
            await player.PlayAsync(queueable);
        }

        [Command("Stop")]
        public async Task StopAsync() {
            var player = TryGetCurrentPlayer();

            if (player?.PlayerState != PlayerState.Playing) {
                await ReplyAsync("I am not playing anything");
                return;
            }

            await player.StopAsync();
        }

        [Command("PullIt")]
        public async Task PullItAsync() {
            var player = TryGetCurrentPlayer();

            if (player?.PlayerState != PlayerState.Playing) {
                await ReplyAsync("I am not playing anything");
                return;
            }

            await _musicService.PullUpTrackAsync(player, player.Track, Context.User);
            await ReplyAsync("Wheel that one up");
        }

        [Command("TrackId")]
        public async Task TrackIdAsync() {
            var player = TryGetCurrentPlayer();            

            if (player?.PlayerState != PlayerState.Playing) {
                await ReplyAsync("I am not playing anything");
                return;
            }

            await ReplyAsync($"Currently playing {player.Track.Title}");
        }

        private LavaPlayer? TryGetCurrentPlayer() {
            _lavaNode.TryGetPlayer(Context.Guild, out var player);

            return player;
        }
    }
}