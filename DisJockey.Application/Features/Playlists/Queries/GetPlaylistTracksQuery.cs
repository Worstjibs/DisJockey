using DisJockey.Shared.DTOs.Playlist;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using MediatR;

namespace DisJockey.Application.Features.Playlists.Queries;

public record GetPlaylistTracksQuery(PaginationParams Pagination, string YouTubeId) : IRequest<PagedList<TrackListDto>>;
