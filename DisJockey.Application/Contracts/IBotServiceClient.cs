namespace DisJockey.Application.Contracts;

public interface IBotServiceClient
{
    Task<VoiceChannelInfo?> GetUserVoiceChannelAsync(
        ulong userId,
        CancellationToken cancellationToken = default);
    Task<TrackStatusInfo?> GetTrackStatusAsync(
        ulong serverId,
        ulong voiceChannelId,
        CancellationToken cancellationToken = default);
    Task SeekTrackAsync(
        ulong serverId,
        ulong voiceChannelId,
        int positionSeconds,
        CancellationToken cancellationToken = default);
    Task PlayPauseTrackAsync(
        ulong serverId,
        ulong voiceChannelId,
        CancellationToken cancellationToken = default);
}

public record VoiceChannelInfo(
    ulong VoiceChannelId,
    string VoiceChannelName,
    ulong ServerId,
    string ServerName);

public record TrackStatusInfo(
    string TrackName,
    bool Paused);