﻿using Discord;
using Discord.WebSocket;
using DisJockey.MassTransit.Enums;
using DisJockey.MassTransit.Events;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using MassTransit;
using Microsoft.Extensions.Options;

namespace DisJockey.BotService.Consumers;

public class PlayTrackEventConsumer : IConsumer<PlayTrackEvent>
{
    private readonly IAudioService _audioService;
    private readonly IDiscordClient _discordClient;
    private readonly IOptions<QueuedLavalinkPlayerOptions> _queuePlayerOptions;
    private readonly IBus _bus;

    public PlayTrackEventConsumer(
        IAudioService audioService,
        DiscordSocketClient discordClient,
        IOptions<QueuedLavalinkPlayerOptions> queuePlayerOptions,
        IBus bus
    )
    {
        _audioService = audioService;
        _discordClient = discordClient;
        _queuePlayerOptions = queuePlayerOptions;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<PlayTrackEvent> context)
    {
        var message = context.Message;

        var discordUser = await _discordClient.GetUserAsync(message.DiscordId) as SocketUser;
        if (discordUser is null)
            throw new Exception("Discord user not found");

        var guild = discordUser.MutualGuilds.FirstOrDefault(g => g.VoiceChannels.Any(v => v.ConnectedUsers.Any(u => u.Id == message.DiscordId)));
        if (guild is null)
            throw new Exception("Music first be connected to a voice channel");

        var voiceChannel = guild.VoiceChannels.First(x => x.ConnectedUsers.Any(u => u.Id == message.DiscordId));

        var retrieveOptions = new PlayerRetrieveOptions(PlayerChannelBehavior.Join);

        var playerResult = await _audioService.Players.RetrieveAsync(guild.Id, voiceChannel.Id, playerFactory: PlayerFactory.Queued, _queuePlayerOptions, retrieveOptions);
        if (!playerResult.IsSuccess)
            return;

        var track = await _audioService.Tracks.LoadTrackAsync(message.YoutubeId, TrackSearchMode.YouTube);
        if (track is null)
            return;

        await playerResult.Player.PlayAsync(track, enqueue: message.Queue);

        await _bus.Publish(new TrackPlayedEvent
        {
            SearchMode = SearchMode.YouTube,
            TrackId = track.Identifier,
            DiscordId = discordUser.Id,
            AvatarUrl = discordUser.GetAvatarUrl(),
            UserName = discordUser.Username
        });
    }
}
