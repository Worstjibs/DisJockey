namespace DisJockey.BotService.Grpc;

public static partial class DisJockeyGrpcServiceLoggerExtensions
{
    [LoggerMessage(
        LogLevel.Debug,
        "Player {SessionId}, for Server {ServerId}, Voice Channel {VoiceChannelId} {State}")]
    public static partial void LogPlayPausedState(
        this ILogger logger,
        string sessionId,
        ulong serverId,
        ulong voiceChannelId,
        string state);
}