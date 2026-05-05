using DisJockey.Application.Interfaces;
using DisJockey.Core;
using DisJockey.Shared.Helpers;

namespace DisJockey.Api.Integration.Tests;

public class FakeVideoDetailService : IVideoDetailService
{
    public YouTubePagedList<Track> QueryResult { get; set; } = YouTubePagedList<Track>.Empty();

    public Task<YouTubePagedList<Track>> QueryTracksAsync(PaginationParams paginationParams)
        => Task.FromResult(QueryResult);

    public Task<Track> GetVideoDetailsAsync(Track track) => Task.FromResult(track);

    public Task<Playlist?> GetPlaylistDetailsAsync(string playlistId) => Task.FromResult<Playlist?>(null);
}
