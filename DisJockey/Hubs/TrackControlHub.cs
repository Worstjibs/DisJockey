using DisJockey.Shared.Notifications;
using DisJockey.Shared.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DisJockey.Hubs;

[Authorize]
public class TrackControlHub : Hub<ITrackControlHub>
{
    private readonly DisJockeyGrpc.DisJockeyGrpcClient _disJockeyGrpcClient;

    private readonly ILogger<TrackControlHub> _logger;

    public TrackControlHub(
        DisJockeyGrpc.DisJockeyGrpcClient disJockeyGrpcClient,
        ILogger<TrackControlHub> logger)
    {
        _disJockeyGrpcClient = disJockeyGrpcClient;
        _logger = logger;
    }

    public async Task UserVoiceStateChanged(UserVoiceStateNotification notification)
    {
        await HandleVoiceStateNotification(notification);
    }

    public async Task TrackStatusChanged(TrackStatusChangedNotification notification)
    {
        _logger.LogDebug("Received track status changed notification: {TrackName}", notification.TrackName);

        var message = new TrackStatusMessage(notification.TrackName);

        await Clients.All.NotifyTrackStatus(message);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var discordId = ulong.Parse(userId);

        var notification = new UserVoiceStateNotification
        {
            DiscordId = discordId
        };

        var voiceChannel = await GetUserVoiceChannelAsync(notification.DiscordId);
        if (voiceChannel is not null)
        {
            notification.VoiceState = VoiceState.Connected;
            notification.VoiceChannel = voiceChannel;
        }

        await HandleVoiceStateNotification(notification);

        if (voiceChannel is not null)
        {
            var trackStatus = await GetCurrentTrackStatusAsync(voiceChannel.ServerId, voiceChannel.Id);
            if (trackStatus is not null)
            {
                await Clients.User(userId).NotifyTrackStatus(trackStatus);
            }
        }
    }

    private async Task HandleVoiceStateNotification(UserVoiceStateNotification notification)
    {
        if (notification.VoiceState is VoiceState.Disconnected)
        {
            await Clients.User(notification.DiscordId.ToString()).SendMessageAsync("Disconnected");
            return;
        }

        if (notification.VoiceChannel is null)
        {
            return;
        }

        var message = ConstructConnectedMessage(notification.VoiceChannel.Name, notification.VoiceChannel.ServerName);
        await Clients.User(notification.DiscordId.ToString()).SendMessageAsync(message);
    }

    private async Task<VoiceChannelInfo?> GetUserVoiceChannelAsync(ulong userId)
    {
        try
        {
            var voiceChannelDetails = await _disJockeyGrpcClient.GetUserVoiceChannelAsync(new() { UserId = userId });

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
        ulong voiceChannelId)
    {
        try
        {
            var trackStatus = await _disJockeyGrpcClient.GetTrackStatusAsync(new()
            {
                ServerId = serverId,
                VoiceChannelId = voiceChannelId
            });

            return new TrackStatusMessage(trackStatus.TrackName);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound)
        {
            return null;
        }
    }

    private static string ConstructConnectedMessage(string voiceChannelName, string serverName)
        => $"Connected to {voiceChannelName} in {serverName}";
}

public interface ITrackControlHub
{
    Task SendMessageAsync(string message);
    Task NotifyTrackStatus(TrackStatusMessage message);
}

public record TrackStatusMessage(string TrackName);