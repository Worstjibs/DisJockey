using DisJockey.Application.Contracts;
using DisJockey.Application.Features.Tracks.Queries.AllTracks;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
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
                    [AsParameters] PaginationParams pagination,
                    CancellationToken cancellationToken) =>
                {
                    var tracks = await mediator.SendAsync<AllTracksQuery, PagedList<TrackListDto>>(new AllTracksQuery(pagination), cancellationToken);

                    return Results.Ok(tracks);
                })
                .RequireAuthorization();

            return builder;
        }
    }
}
