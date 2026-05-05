using DisJockey.Application.Contracts;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;

namespace DisJockey.Application.Features.Playlists.Queries;

public record GetPlaylistTracksQuery(PaginationParams Pagination, string YouTubeId) : IRequest<PagedList<TrackListDto>>;
