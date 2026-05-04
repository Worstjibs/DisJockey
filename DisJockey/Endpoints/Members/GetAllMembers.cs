using DisJockey.Application.Contracts;
using DisJockey.Application.Features.Members.Queries.AllMembers;
using DisJockey.Shared.DTOs.Member;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading;

namespace DisJockey.Endpoints.Members;

public static class GetAllMembers
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapGet(
                "/api/members", 
                async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var members = await mediator.SendAsync<AllMembersQuery, IEnumerable<MemberListDto>>(new AllMembersQuery(), cancellationToken);
                return Results.Ok(members);
            })
            .RequireAuthorization();

            return builder;
        }
    }
}
