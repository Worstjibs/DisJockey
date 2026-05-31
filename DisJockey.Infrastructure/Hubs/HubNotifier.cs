using DisJockey.Application.Contracts;
using DisJockey.Shared.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace DisJockey.Infrastructure.Hubs;

internal class HubNotifier : INotifier
{
    private readonly IHubContext<TrackControlHub, ITrackControlHub> _hubContext;
    private readonly IUserConnectionTracker _userConnectionTracker;

    public HubNotifier(
        IHubContext<TrackControlHub, ITrackControlHub> hubContext,
        IUserConnectionTracker userConnectionTracker)
    {
        _hubContext = hubContext;
        _userConnectionTracker = userConnectionTracker;
    }

    public async Task SendTrackNotificationAsync(
        ulong voiceChannelId,
        TrackStatusNotification? notification)
    {
        await _hubContext.Clients.SendTrackNotificationAsync(voiceChannelId, notification);
    }

    public async Task SendTrackProgressNotificationAsync(
        ulong voiceChannelId,
        TrackProgressNotification notification)
    {
        await _hubContext.Clients.SendTrackProgressNotificationAsync(voiceChannelId, notification);
    }

    public async Task SendUserStatusMessageAsync(
        ulong discordId,
        UserStatusNotification? notification)
    {
        await _hubContext.Clients.SendUserStatusMessageAsync(discordId, notification);
    }

    public async Task UpdateUserConnectionVoiceChannelAsync(ulong discordId, ulong voiceChannelId)
    {
        var userConnection = _userConnectionTracker.GetConnection(discordId.ToString());
        if (userConnection is not null)
        {
            await _hubContext.Groups.AddToGroupAsync(userConnection.ConnectionId, voiceChannelId.ToString());
        }
    }
}
