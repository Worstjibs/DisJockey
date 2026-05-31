namespace DisJockey.Shared.Messaging.Events;

public record UserVoiceStateChangedEvent(
    ulong DiscordId,
    UserVoiceChannelDetails? VoiceChannelDetails);

public record UserVoiceChannelDetails(
    ulong VoiceChannelId,
    string VoiceChannelName,
    ulong ServerId,
    string ServerName);
