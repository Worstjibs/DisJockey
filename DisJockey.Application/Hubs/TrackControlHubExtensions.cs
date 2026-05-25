using Microsoft.AspNetCore.SignalR;

namespace DisJockey.Application.Hubs;

public static class TrackControlHubExtensions
{
    public static Task SendToUserAsync(
        this IHubClients<ITrackControlHub> hubContext, 
        ulong discordId, 
        string message)
    {
        return hubContext.User(discordId.ToString()).SendMessageAsync(message);
    }
    public static Task SendUserStatusMessageAsync(
        this IHubClients<ITrackControlHub> hubContext,
        ulong discordId,
        UserStatusMessage? message)
    {
        return hubContext.User(discordId.ToString()).NotifyUserStatus(message);
    }

    public static Task SendTrackNotificationAsync(
        this IHubClients<ITrackControlHub> hubContext,
        ulong voiceChannelId,
        TrackStatusMessage? message)
    {
        return hubContext.Groups(voiceChannelId.ToString()).NotifyTrackStatus(message);
    }

    public static Task SendTrackProgressNotificationAsync(
        this IHubClients<ITrackControlHub> hubContext,
        ulong voiceChannelId,
        TrackProgressMessage message)
    {
        return hubContext.Groups(voiceChannelId.ToString()).NotifyTrackProgress(message);
    }
}
