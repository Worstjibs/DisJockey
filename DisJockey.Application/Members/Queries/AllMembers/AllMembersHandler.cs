using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Member;
using DisJockey.Shared.Helpers;
using MediatR;

namespace DisJockey.Application.Members.Queries.AllMembers;

public class AllMembersHandler : IRequestHandler<AllMembersQuery, IEnumerable<MemberListDto>>
{
    private readonly IUserRepository _userRepository;

    public AllMembersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<MemberListDto>> Handle(AllMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await _userRepository.GetMembersAsync();

        return members;
    }
}
