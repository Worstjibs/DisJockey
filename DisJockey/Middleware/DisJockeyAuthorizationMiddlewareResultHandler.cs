using DisJockey.Discord;
using DisJockey.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DisJockey.Middleware {
    public class DisJockeyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
        private readonly DiscordService _discordService;

        public DisJockeyAuthorizationMiddlewareResultHandler(DiscordService discordService) {
            _discordService = discordService;
        }

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult) {
            var discordId = context.User.GetDiscordId();

            if (!discordId.HasValue) {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            if (!_discordService.UserPermitted(discordId.Value)) {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
