namespace DisJockey.Shared.Messaging.Events;

public record TrackProgressEvent(
    ulong VoiceChannelId,
    int ElapsedSeconds,
    int TotalSeconds);
