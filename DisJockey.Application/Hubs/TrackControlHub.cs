using DisJockey.Application.Clients;
using DisJockey.Shared.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Hubs;

public class TrackControlHub : Hub<ITrackControlHub>
{
    private readonly IBotServiceClient _botServiceClient;
    private readonly IUserConnectionTracker _connectionTracker;
    private readonly ILogger<TrackControlHub> _logger;

    public TrackControlHub(
        IBotServiceClient botServiceClient,
        IUserConnectionTracker connectionTracker,
        ILogger<TrackControlHub> logger)
    {
        _botServiceClient = botServiceClient;
        _connectionTracker = connectionTracker;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        _connectionTracker.Add(userId, Context.ConnectionId);

        var discordId = ulong.Parse(userId);

        var voiceChannel = await _botServiceClient.GetUserVoiceChannelAsync(discordId, Context.ConnectionAborted);
        if (voiceChannel is not null)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                voiceChannel.VoiceChannelId.ToString(),
                Context.ConnectionAborted);
        }

        await HandleVoiceStateNotificationAsync(discordId, voiceChannel);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            _connectionTracker.Remove(userId);
        }
    }

    public async Task PublishTrackProgress(TrackProgressNotification notification)
    {
        await Clients.SendTrackProgressNotificationAsync(
            notification.VoiceChannelId,
            new TrackProgressMessage(notification.ElapsedSeconds, notification.TotalSeconds));
    }

    public async Task TrackStatusChanged(TrackStatusChangedNotification notification)
    {
        _logger.LogDebug(
            "Received track status changed notification for VoiceChannel: {VoiceChannelId}",
            notification.VoiceChannelId);

        TrackStatusMessage? message = null;
        if (notification.TrackDetails is not null)
        {
            message = new TrackStatusMessage(notification.TrackDetails.TrackName, Paused: false); // TODO: Fix Paused
        }

        await Clients.SendTrackNotificationAsync(notification.VoiceChannelId, message);
    }

    public async Task SeekTrack(int positionSeconds)
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var discordId = ulong.Parse(userId);

        var voiceChannel = await _botServiceClient.GetUserVoiceChannelAsync(discordId, Context.ConnectionAborted);
        if (voiceChannel is null)
        {
            return;
        }

        await _botServiceClient.SeekTrackAsync(voiceChannel.ServerId, voiceChannel.VoiceChannelId, positionSeconds, Context.ConnectionAborted);
    }

    public async Task TogglePlayPause()
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var discordId = ulong.Parse(userId);

        var voiceChannel = await _botServiceClient.GetUserVoiceChannelAsync(discordId, Context.ConnectionAborted);
        if (voiceChannel is null)
        {
            return;
        }

        await _botServiceClient.PlayPauseTrackAsync(voiceChannel.ServerId, voiceChannel.VoiceChannelId, Context.ConnectionAborted);
    }

    private async Task HandleVoiceStateNotificationAsync(ulong discordId, VoiceChannelInfo? voiceChannelInfo)
    {
        if (voiceChannelInfo is null)
        {
            await Clients.SendUserStatusMessageAsync(discordId, null);
            return;
        }

        var trackStatus = await _botServiceClient.GetTrackStatusAsync(
            voiceChannelInfo.ServerId,
            voiceChannelInfo.VoiceChannelId,
            Context.ConnectionAborted);

        var trackStatusMessage = trackStatus is not null
                                    ? new TrackStatusMessage(trackStatus.TrackName, trackStatus.Paused)
                                    : null;

        var userStatusMessage = new UserStatusMessage(
            voiceChannelInfo.VoiceChannelName,
            voiceChannelInfo.ServerName,
            trackStatusMessage);

        await Clients.SendUserStatusMessageAsync(discordId, userStatusMessage);
    }
}

public interface ITrackControlHub
{
    Task NotifyUserStatus(UserStatusMessage? message);
    Task NotifyTrackStatus(TrackStatusMessage? message);
    Task NotifyTrackProgress(TrackProgressMessage message);
}

public record UserStatusMessage(
    string VoiceChannelName,
    string ServerName,
    TrackStatusMessage? TrackStatusMessage);

public record TrackStatusMessage(string TrackName, bool Paused);

public record TrackProgressMessage(int Elapsed, int Total);

