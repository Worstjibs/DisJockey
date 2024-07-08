using DisJockey.Shared.DTOs.Member;
using MediatR;

namespace DisJockey.Application.Features.Members.Queries.AllMembers;

public record AllMembersQuery : IRequest<IEnumerable<MemberListDto>>;
