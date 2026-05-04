using DisJockey.Application.Contracts;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Member;

namespace DisJockey.Application.Features.Members.Queries.AllMembers;

public class AllMembersHandler : IRequestHandler<AllMembersQuery, IEnumerable<MemberListDto>>
{
    private readonly IUserRepository _userRepository;

    public AllMembersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<MemberListDto>> HandleAsync(
        AllMembersQuery request, 
        CancellationToken cancellationToken = default)
    {
        var members = await _userRepository.GetMembersAsync();

        return members;
    }
}

public record AllMembersQuery : IRequest<IEnumerable<MemberListDto>>;