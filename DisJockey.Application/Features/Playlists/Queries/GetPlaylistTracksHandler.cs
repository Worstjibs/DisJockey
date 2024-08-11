using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using MediatR;

namespace DisJockey.Application.Features.Playlists.Queries;

public class GetPlaylistTracksHandler : IRequestHandler<GetPlaylistTracksQuery, PagedList<TrackListDto>>
{
    private readonly IPlaylistRepository _playlistRepository;

    public GetPlaylistTracksHandler(IPlaylistRepository playlistRepository)
    {
        _playlistRepository = playlistRepository;
    }

    public async Task<PagedList<TrackListDto>> Handle(GetPlaylistTracksQuery request, CancellationToken cancellationToken)
    {
        var tracks = await _playlistRepository.GetPlaylistTracks(request.Pagination, request.YouTubeId);

        return tracks;
    }
}
