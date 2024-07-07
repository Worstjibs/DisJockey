using DisJockey.Shared.DTOs.Member;
using MediatR;

namespace DisJockey.Application.Members.Queries.GetMember;

public record GetMemberQuery(ulong DiscordId) : IRequest<MemberDetailDto?>;
