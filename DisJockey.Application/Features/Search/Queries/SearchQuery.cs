using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using MediatR;
using static DisJockey.Application.Features.Search.Queries.SearchHandler;

namespace DisJockey.Application.Features.Search.Queries;

public record SearchQuery(PaginationParams Pagination) : IRequest<(IEnumerable<TrackListDto> Results, YouTubePagination? Pagination)>;
