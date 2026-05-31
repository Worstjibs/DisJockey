using DisJockey.Shared.Notifications;

namespace DisJockey.Application.Contracts;

public interface INotifier
{
    Task SendUserStatusMessageAsync(
        ulong discordId,
        UserStatusNotification? notification);

    Task SendTrackNotificationAsync(
        ulong voiceChannelId,
        TrackStatusNotification? notification);

    Task SendTrackProgressNotificationAsync(
        ulong voiceChannelId,
        TrackProgressNotification notification);

    Task UpdateUserConnectionVoiceChannelAsync(
        ulong discordId, 
        ulong voiceChannelId);
}
