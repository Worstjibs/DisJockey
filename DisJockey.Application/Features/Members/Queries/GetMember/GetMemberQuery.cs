using DisJockey.Shared.DTOs.Member;
using MediatR;

namespace DisJockey.Application.Features.Members.Queries.GetMember;

public record GetMemberQuery(ulong DiscordId) : IRequest<MemberDetailDto?>;
