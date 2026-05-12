using DisJockey.Application.Contracts;
using DisJockey.Application.Features.PullUps.Queries.PullUpsForMember;
using DisJockey.Shared.DTOs.PullUps;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace DisJockey.Endpoints.PullUps;

public static class GetPullUpsForMember
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            // Dodgy parameter binding for pagination
            builder.MapGet(
                "/{discordId}",
                async (
                    IMediator mediator,
                    [FromRoute] ulong discordId,
                    CancellationToken cancellationToken,
                    int? pageNumber = null,
                    int? pageSize = null,
                    string? query = null,
                    string? sortBy = null) =>
                {
                    var pagination = PaginationParams.CreateParameters(pageNumber, pageSize, sortBy, query);

                    var pullUps = await mediator.SendAsync<PullUpsForMemberQuery, PagedList<PullUpDto>>(new PullUpsForMemberQuery(pagination, discordId), cancellationToken);

                    return Results.Ok(pullUps);
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
