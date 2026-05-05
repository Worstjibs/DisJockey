using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints.PullUps;

public static class PullUpEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapPullUpEndpoints()
        {
            var group = builder.MapGroup("/pullups");

            GetPullUpsForMember.MapEndpoint(group);
        }
    }
}
