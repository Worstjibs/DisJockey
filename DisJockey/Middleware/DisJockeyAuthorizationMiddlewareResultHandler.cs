using DisJockey.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Threading.Tasks;

namespace DisJockey.Middleware;

public class DisJockeyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{

    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    private readonly IHostEnvironment _env;

    public DisJockeyAuthorizationMiddlewareResultHandler(IHostEnvironment env)
    {
        _env = env;
    }

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (!_env.IsDevelopment())
        {
            var discordId = context.User.GetDiscordId();

            if (!discordId.HasValue)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            // TODO - Use RestClient
            //if (!_discordService.UserPermitted(discordId.Value))
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //    return;
            //}
        }
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
