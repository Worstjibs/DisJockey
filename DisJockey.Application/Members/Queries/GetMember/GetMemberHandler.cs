using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Member;
using MediatR;

namespace DisJockey.Application.Members.Queries.GetMember;

public class GetMemberHandler : IRequestHandler<GetMemberQuery, MemberDetailDto?>
{
    private readonly IUserRepository _userRepository;

    public GetMemberHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<MemberDetailDto?> Handle(GetMemberQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetMemberByDiscordIdAsync(request.DiscordId);

        return user;
    }
}
