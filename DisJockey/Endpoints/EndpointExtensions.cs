using DisJockey.Endpoints.Members;
using DisJockey.Endpoints.PullUps;
using DisJockey.Endpoints.Tracks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints;

public static class EndpointExtensions
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapDisJockeyEndpoints()
        {
            var apiGroup = builder.MapGroup("/api");

            apiGroup.MapMemberEndpoints();
            apiGroup.MapTrackEndpoints();
            apiGroup.MapPullUpEndpoints();
        }
    }
}
