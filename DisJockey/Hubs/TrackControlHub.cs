using DisJockey.Shared.Notifications;
using DisJockey.Shared.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DisJockey.Hubs;

[Authorize]
public class TrackControlHub : Hub<ITrackControlHub>
{
    private readonly DisJockeyGrpc.DisJockeyGrpcClient _disJockeyGrpcClient;

    public TrackControlHub(DisJockeyGrpc.DisJockeyGrpcClient disJockeyGrpcClient)
    {
        _disJockeyGrpcClient = disJockeyGrpcClient;
    }

    public async Task UserVoiceStateChanged(UserVoiceStateNotification notification)
    {
        await HandleVoiceStateNotification(notification);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var notification = new UserVoiceStateNotification
        {
            DiscordId = ulong.Parse(userId)
        };

        var voiceChannel = await GetUserVoiceChannelAsync(notification.DiscordId);
        if (voiceChannel is not null)
        {
            notification.VoiceState = VoiceState.Connected;
            notification.VoiceChannel = voiceChannel;
        }

        await HandleVoiceStateNotification(notification);
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

    private static string ConstructConnectedMessage(string voiceChannelName, string serverName)
        => $"Connected to {voiceChannelName} in {serverName}";

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
}

public interface ITrackControlHub
{
    Task SendMessageAsync(string message);
}
