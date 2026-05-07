using DisJockey.Application.Contracts;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;

namespace DisJockey.Application.Features.Playlists.Queries;

public class GetPlaylistTracksHandler : IRequestHandler<GetPlaylistTracksQuery, PagedList<TrackListDto>>
{
    private readonly IPlaylistRepository _playlistRepository;

    public GetPlaylistTracksHandler(IPlaylistRepository playlistRepository)
    {
        _playlistRepository = playlistRepository;
    }

    public async Task<PagedList<TrackListDto>> HandleAsync(
        GetPlaylistTracksQuery request, 
        CancellationToken cancellationToken = default)
    {
        return await _playlistRepository.GetPlaylistTracks(request.Pagination, request.YouTubeId);
    }
}

public record GetPlaylistTracksQuery(PaginationParams Pagination, string YouTubeId) : IRequest<PagedList<TrackListDto>>;
