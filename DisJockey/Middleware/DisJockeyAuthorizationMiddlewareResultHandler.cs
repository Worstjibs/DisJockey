using Discord;
using Discord.Rest;
using DisJockey.Services;
using DisJockey.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DisJockey.Middleware;

public class DisJockeyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{

    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    private readonly IHostEnvironment _env;
    private readonly DiscordRestClient _discordRestClient;
    private readonly BotGuildsService _botGuildsService;

    public DisJockeyAuthorizationMiddlewareResultHandler(
        IHostEnvironment env,
        DiscordRestClient discordRestClient,
        BotGuildsService botGuildsService)
    {
        _env = env;
        _discordRestClient = discordRestClient;
        _botGuildsService = botGuildsService;
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

            var guildIds = await _botGuildsService.TryGetUserGuilds(discordId.Value);
            if (guildIds is null)
                guildIds = await FetchGuildsFromDiscordAsync(discordId.Value, context.User);

            var isPartOfGuild = await _botGuildsService.IsPartOfGuildsAsync(guildIds);
            if (!isPartOfGuild)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }

    private async Task<IEnumerable<ulong>> FetchGuildsFromDiscordAsync(ulong discordId, ClaimsPrincipal user)
    {
        var discordToken = user.Claims.First(x => x.Type == "discord_token");

        await _discordRestClient.LoginAsync(TokenType.Bearer, discordToken.Value);

        var guildSummaries = await _discordRestClient.GetGuildSummariesAsync().ToListAsync();
        var guildIds = guildSummaries.SelectMany(x => x).Select(x => x.Id).ToArray();

        await _botGuildsService.AddUserGuildsAsync(discordId, guildIds);

        return guildIds;
    }
}
