using DisJockey.Application.Features.Tracks.Queries.TracksForMember;
using DisJockey.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace DisJockey.Endpoints.Tracks;

public static class GetTracksForMember
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapGet(
                "/api/tracks/{discordId}",
                async (
                    IMediator mediator,
                    ulong discordId,
                    //PaginationParams pagination,
                    CancellationToken cancellationToken) =>
                {
                    var pagination = new PaginationParams();

                    var tracks = await mediator.Send(new TracksForMemberQuery(pagination, discordId), cancellationToken);

                    return Results.Ok(tracks);
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
