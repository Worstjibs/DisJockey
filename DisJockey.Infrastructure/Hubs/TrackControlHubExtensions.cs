using DisJockey.Shared.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace DisJockey.Infrastructure.Hubs;

public static class TrackControlHubExtensions
{
    public static Task SendUserStatusMessageAsync(
        this IHubClients<ITrackControlHub> hubContext,
        ulong discordId,
        UserStatusNotification? notification)
    {
        return hubContext.User(discordId.ToString()).NotifyUserStatus(notification);
    }

    public static Task SendTrackNotificationAsync(
        this IHubClients<ITrackControlHub> hubContext,
        ulong voiceChannelId,
        TrackStatusNotification? notification)
    {
        return hubContext.Groups(voiceChannelId.ToString()).NotifyTrackStatus(notification);
    }

    public static Task SendTrackProgressNotificationAsync(
        this IHubClients<ITrackControlHub> hubContext,
        ulong voiceChannelId,
        TrackProgressNotification notification)
    {
        return hubContext.Groups(voiceChannelId.ToString()).NotifyTrackProgress(notification);
    }
}
