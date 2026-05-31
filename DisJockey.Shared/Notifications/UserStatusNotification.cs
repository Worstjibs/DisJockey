namespace DisJockey.Shared.Notifications;

public record UserStatusNotification(
    string VoiceChannelName,
    string ServerName,
    TrackStatusNotification? TrackStatusMessage);
