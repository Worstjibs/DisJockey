using DisJockey.Application.Contracts;
using DisJockey.Application.Features.Members.Queries.GetMember;
using DisJockey.Shared.DTOs.Member;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace DisJockey.Endpoints.Members;

public static class GetMember
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapGet(
                "/{discordId}",
                async (
                    IMediator mediator, 
                    ulong discordId,
                    CancellationToken cancellationToken) =>
                {
                    var member = await mediator.SendAsync<GetMemberQuery, MemberDetailDto?>(new GetMemberQuery(discordId), cancellationToken);
                    if (member is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(member);
                })
            .RequireAuthorization();

            return builder;
        }
    }
}
