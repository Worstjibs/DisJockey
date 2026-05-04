using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints.Members;

public static class MemberEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapMemberEndpoints()
        {
            GetAllMembers.MapEndpoint(builder);
            GetMember.MapEndpoint(builder);
        }
    }
}
