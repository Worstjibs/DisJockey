using DisJockey.Application.Contracts;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Member;

namespace DisJockey.Application.Features.Members.Queries.GetMember;

public class GetMemberHandler : IRequestHandler<GetMemberQuery, MemberDetailDto?>
{
    private readonly IUserRepository _userRepository;

    public GetMemberHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<MemberDetailDto?> HandleAsync(GetMemberQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetMemberByDiscordIdAsync(request.DiscordId);

        return user;
    }
}

public record GetMemberQuery(ulong DiscordId) : IRequest<MemberDetailDto?>;