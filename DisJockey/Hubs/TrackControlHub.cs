using DisJockey.Shared.Notifications;
using DisJockey.Shared.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DisJockey.Hubs;

[Authorize]
public class TrackControlHub : Hub<ITrackControlHub>
{
    private readonly DisJockeyGrpc.DisJockeyGrpcClient _disJockeyGrpcClient;
    private readonly IUserConnectionTracker _connectionTracker;
    private readonly ILogger<TrackControlHub> _logger;

    public TrackControlHub(
        DisJockeyGrpc.DisJockeyGrpcClient disJockeyGrpcClient,
        IUserConnectionTracker connectionTracker,
        ILogger<TrackControlHub> logger)
    {
        _disJockeyGrpcClient = disJockeyGrpcClient;
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

        var notification = new UserVoiceStateNotification
        {
            DiscordId = discordId
        };

        var voiceChannel = await GetUserVoiceChannelAsync(discordId, Context.ConnectionAborted);
        if (voiceChannel is not null)
        {
            notification.VoiceState = VoiceState.Connected;
            notification.VoiceChannel = voiceChannel;

            await Groups.AddToGroupAsync(Context.ConnectionId, voiceChannel.Id.ToString(), Context.ConnectionAborted);
        }

        await HandleVoiceStateNotification(notification);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            _connectionTracker.Remove(userId);
        }
    }

    public async Task UserVoiceStateChanged(UserVoiceStateNotification notification)
    {
        var voiceChannel = await GetUserVoiceChannelAsync(notification.DiscordId, Context.ConnectionAborted);
        if (voiceChannel is not null)
        {
            notification.VoiceState = VoiceState.Connected;
            notification.VoiceChannel = voiceChannel;

            var userConnectionId = _connectionTracker.GetConnection(notification.DiscordId.ToString());
            if (userConnectionId is not null)
            {
                await Groups.AddToGroupAsync(userConnectionId, voiceChannel.Id.ToString(), Context.ConnectionAborted);
            }
        }

        await HandleVoiceStateNotification(notification);
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

        var userStatus = await GetUserVoiceChannelAsync(discordId, Context.ConnectionAborted);
        if (userStatus is null)
        {
            return;
        }

        await SeekTrackAsync(userStatus.ServerId, userStatus.Id, positionSeconds, Context.ConnectionAborted);
    }

    public async Task TogglePlayPause()
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var discordId = ulong.Parse(userId);

        var userStatus = await GetUserVoiceChannelAsync(discordId, Context.ConnectionAborted);
        if (userStatus is null)
        {
            return;
        }

        await PlayPauseTrackAsync(userStatus.ServerId, userStatus.Id, Context.ConnectionAborted);
    }

    private async Task HandleVoiceStateNotification(UserVoiceStateNotification notification)
    {
        if (notification.VoiceState is VoiceState.Disconnected)
        {
            await Clients.SendUserStatusMessageAsync(notification.DiscordId, null);
            return;
        }

        var voiceChannel = notification.VoiceChannel;

        if (voiceChannel is null)
        {
            return;
        }

        var trackStatus = await GetCurrentTrackStatusAsync(voiceChannel.ServerId, voiceChannel.Id, Context.ConnectionAborted);

        TrackStatusMessage? trackStatusMessage = null;
        if (trackStatus is not null)
        {
            trackStatusMessage = new TrackStatusMessage(trackStatus.TrackName, trackStatus.Paused);
        }

        var userStatusMessage = new UserStatusMessage(
            voiceChannel.Name,
            voiceChannel.ServerName,
            trackStatusMessage);

        await Clients.SendUserStatusMessageAsync(notification.DiscordId, userStatusMessage);
    }

    private async Task<VoiceChannelInfo?> GetUserVoiceChannelAsync(
        ulong userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var voiceChannelDetails = await _disJockeyGrpcClient.GetUserVoiceChannelAsync(
                new() { UserId = userId },
                cancellationToken: cancellationToken);

            return new VoiceChannelInfo
            {
                Id = voiceChannelDetails.VoiceChannelId,
                Name = voiceChannelDetails.VoiceChannelName,
                ServerId = voiceChannelDetails.ServerId,
                ServerName = voiceChannelDetails.ServerName
            };
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound)
        {
            return null;
        }
    }

    private async Task<TrackStatusMessage?> GetCurrentTrackStatusAsync(
        ulong serverId,
        ulong voiceChannelId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var trackStatus = await _disJockeyGrpcClient.GetTrackStatusAsync(new()
            {
                ServerId = serverId,
                VoiceChannelId = voiceChannelId
            }, cancellationToken: cancellationToken);

            return new TrackStatusMessage(trackStatus.TrackName, trackStatus.Paused);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound)
        {
            return null;
        }
    }

    private async Task SeekTrackAsync(
        ulong serverId,
        ulong voiceChannelId,
        int positionSeconds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _disJockeyGrpcClient.SeekTrackAsync(new()
            {
                ServerId = serverId,
                VoiceChannelId = voiceChannelId,
                PositionSeconds = positionSeconds
            }, cancellationToken: cancellationToken);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound)
        {
            return;
        }
    }

    private async Task PlayPauseTrackAsync(
        ulong serverId,
        ulong voiceChannelId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _disJockeyGrpcClient.PlayPauseTrackAsync(new()
            {
                ServerId = serverId,
                VoiceChannelId = voiceChannelId
            }, cancellationToken: cancellationToken);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound)
        {
            // TODO: Handle NotFound errors better
            return;
        }
    }
}

public interface ITrackControlHub
{
    Task SendMessageAsync(string message);
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