using Discord;
using Discord.Rest;
using DisJockey.Services;
using DisJockey.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Net;
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

            var discordToken = context.User.Claims.First(x => x.Type == "discord_token");

            await _discordRestClient.LoginAsync(TokenType.Bearer, discordToken.Value);

            var guildSummaries = await _discordRestClient.GetGuildSummariesAsync().ToListAsync();
            var guildIds = guildSummaries.SelectMany(x => x).Select(x => x.Id).ToArray();

            var isPartOfGuild = await _botGuildsService.IsPartOfGuildsAsync(guildIds);

            if (!isPartOfGuild)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
