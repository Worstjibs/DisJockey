namespace DisJockey.Shared.Notifications;

public record TrackStatusChangedNotification(
    ulong VoiceChannelId, 
    TrackStatusDetails? TrackDetails = null);

public record TrackStatusDetails(string TrackName);