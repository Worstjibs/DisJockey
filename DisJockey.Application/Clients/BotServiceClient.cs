using DisJockey.Shared.Protos;
using Grpc.Core;

namespace DisJockey.Application.Clients;

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

public class BotServiceClient : IBotServiceClient
{
    private readonly DisJockeyGrpc.DisJockeyGrpcClient _grpcClient;

    public BotServiceClient(DisJockeyGrpc.DisJockeyGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<VoiceChannelInfo?> GetUserVoiceChannelAsync(
        ulong userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _grpcClient.GetUserVoiceChannelAsync(
                new() { UserId = userId },
                cancellationToken: cancellationToken);

            return new VoiceChannelInfo(
                response.VoiceChannelId,
                response.VoiceChannelName,
                response.ServerId,
                response.ServerName);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<TrackStatusInfo?> GetTrackStatusAsync(
        ulong serverId,
        ulong voiceChannelId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _grpcClient.GetTrackStatusAsync(new()
            {
                ServerId = serverId,
                VoiceChannelId = voiceChannelId
            }, cancellationToken: cancellationToken);

            return new TrackStatusInfo(response.TrackName, response.Paused);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task SeekTrackAsync(
        ulong serverId,
        ulong voiceChannelId,
        int positionSeconds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _grpcClient.SeekTrackAsync(new()
            {
                ServerId = serverId,
                VoiceChannelId = voiceChannelId,
                PositionSeconds = positionSeconds
            }, cancellationToken: cancellationToken);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound) { }
    }

    public async Task PlayPauseTrackAsync(
        ulong serverId,
        ulong voiceChannelId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _grpcClient.PlayPauseTrackAsync(new()
            {
                ServerId = serverId,
                VoiceChannelId = voiceChannelId
            }, cancellationToken: cancellationToken);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound) { }
    }
}

public record VoiceChannelInfo(
    ulong VoiceChannelId,
    string VoiceChannelName,
    ulong ServerId,
    string ServerName);

public record TrackStatusInfo(
    string TrackName,
    bool Paused);