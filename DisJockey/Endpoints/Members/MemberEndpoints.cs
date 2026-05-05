using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DisJockey.Endpoints.Members;

public static class MemberEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public void MapMemberEndpoints()
        {
            var group = builder.MapGroup("/members");

            GetAllMembers.MapEndpoint(group);
            GetMember.MapEndpoint(group);
        }
    }
}
