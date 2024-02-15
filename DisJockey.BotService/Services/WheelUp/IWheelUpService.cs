using Lavalink4NET.Players.Queued;
using Lavalink4NET.Tracks;

namespace DisJockey.BotService.Services.WheelUp;
public interface IWheelUpService
{
    Task PullUp(LavalinkTrack currentTrack, IQueuedLavalinkPlayer player);
}