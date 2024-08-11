using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using MediatR;

namespace DisJockey.Application.Features.Tracks.Queries.TracksForMember;

public record TracksForMemberQuery(PaginationParams Pagination, ulong DiscordId) : IRequest<PagedList<TrackListDto>>;
