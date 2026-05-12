using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using DisJockey.Shared.DTOs.PullUps;
using DisJockey.Shared.Helpers;

namespace DisJockey.Application.Features.PullUps.Queries.PullUpsForMember;

public class PullUpsForMemberHandler : IRequestHandler<PullUpsForMemberQuery, PagedList<PullUpDto>>
{
    private readonly ITrackRepository _trackRepository;

    public PullUpsForMemberHandler(ITrackRepository trackRepository)
    {
        _trackRepository = trackRepository;
    }

    public async Task<PagedList<PullUpDto>> HandleAsync(PullUpsForMemberQuery request, CancellationToken cancellationToken)
    {
        var pullUps = await _trackRepository.GetPullUpsForMember(request.Pagination, request.DiscordId);

        return pullUps;
    }
}

public record PullUpsForMemberQuery(PaginationParams Pagination, ulong DiscordId) : IRequest<PagedList<PullUpDto>>;
