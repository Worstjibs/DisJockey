using DisJockey.Endpoints.Members;
using DisJockey.Endpoints.Tracks;
using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints;

public static class EndpointExtensions
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapDisJockeyEndpoints()
        {
            builder.MapMemberEndpoints();
            builder.MapTrackEndpoints();
        }
    }
}
