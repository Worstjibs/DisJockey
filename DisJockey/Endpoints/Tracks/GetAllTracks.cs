using DisJockey.Application.Features.Tracks.Queries.AllTracks;
using DisJockey.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace DisJockey.Endpoints.Tracks;

public static class GetAllTracks
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapEndpoint()
        {
            builder.MapGet(
                "/",
                async (
                    IMediator mediator, 
                    //PaginationParams pagination,
                    CancellationToken cancellationToken) =>
                {
                    var pagination = new PaginationParams();

                    var tracks = await mediator.Send(new AllTracksQuery(pagination), cancellationToken);

                    return Results.Ok(tracks);
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
