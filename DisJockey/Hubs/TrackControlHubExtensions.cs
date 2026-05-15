using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DisJockey.Hubs;

public static class TrackControlHubExtensions
{
    public static Task SendToUserAsync(
        this IHubCallerClients<ITrackControlHub> hubContext, 
        ulong discordId, 
        string message)
    {
        return hubContext.User(discordId.ToString()).SendMessageAsync(message);
    }
    public static Task SendUserStatusMessageAsync(
        this IHubCallerClients<ITrackControlHub> hubContext,
        ulong discordId,
        UserStatusMessage? message)
    {
        return hubContext.User(discordId.ToString()).NotifyUserStatus(message);
    }

    public static Task SendTrackNotificationAsync(
        this IHubCallerClients<ITrackControlHub> hubContext,
        ulong voiceChannelId,
        TrackStatusMessage? message)
    {
        return hubContext.Groups(voiceChannelId.ToString()).NotifyTrackStatus(message);
    }

    public static Task SendTrackProgressNotificationAsync(
        this IHubCallerClients<ITrackControlHub> hubContext,
        ulong voiceChannelId,
        TrackProgressMessage message)
    {
        return hubContext.Groups(voiceChannelId.ToString()).NotifyTrackProgress(message);
    }
}
