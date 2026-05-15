namespace DisJockey.Shared.Notifications;

public record TrackProgressNotification(
    ulong VoiceChannelId,
    int ElapsedSeconds,
    int TotalSeconds);
