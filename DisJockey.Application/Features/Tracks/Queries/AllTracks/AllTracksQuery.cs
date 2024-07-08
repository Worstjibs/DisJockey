using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using MediatR;

namespace DisJockey.Application.Features.Tracks.Queries.AllTracks;

public record AllTracksQuery(PaginationParams Pagination) : IRequest<PagedList<TrackListDto>>;
