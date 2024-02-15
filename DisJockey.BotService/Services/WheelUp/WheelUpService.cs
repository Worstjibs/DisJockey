using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

namespace DisJockey.BotService.Services.WheelUp;

public class WheelUpService : IWheelUpService
{
    private readonly List<(string YoutubeId, int Start, int End, LavalinkTrack? Track)> _pullUpList = [
        ("pWHY04eWimU", 0, 13500, null),
        ("9JsPk-LLsh0", 4000, -1, null),
        ("nxqiQfb2r-4", 0, 12500, null),
        ("e9OLlI0_VeM", 0, -1, null),
        ("nxqiQfb2r-4", 12500, 18000, null)
    ];

    private int _current = 0;

    private readonly IAudioService _audioService;

    public WheelUpService(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public async Task PullUp(LavalinkTrack currentTrack, IQueuedLavalinkPlayer player)
    {
        var item = _pullUpList[_current];

        if (item.Track is null)
            item.Track = await _audioService.Tracks.LoadTrackAsync(item.YoutubeId, TrackSearchMode.YouTube);

        await player.Queue.InsertAsync(0, new TrackQueueItem(new TrackReference(currentTrack))).ConfigureAwait(false);
        await player.Queue.InsertAsync(0, new TrackQueueItem(new TrackReference(item.Track!))).ConfigureAwait(false);

        await player.SkipAsync().ConfigureAwait(false);

        await player.SeekAsync(TimeSpan.FromMilliseconds(item.Start)).ConfigureAwait(false);

        if (item.End > 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(item.End - item.Start)).ConfigureAwait(false);
            await player.SkipAsync().ConfigureAwait(false);
        }

        if (_current == _pullUpList.Count - 1)
            _current = 0;
        else
            _current++;
    }
}
