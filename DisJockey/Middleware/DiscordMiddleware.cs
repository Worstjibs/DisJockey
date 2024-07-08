using Discord.Rest;
using Discord;
using DisJockey.Services;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using DisJockey.Shared.Extensions;

namespace DisJockey.Middleware;

public class DiscordMiddleware : IMiddleware
{
    private readonly DiscordRestClient _discordRestClient;
    private readonly BotGuildsService _botGuildsService;

    public DiscordMiddleware(DiscordRestClient discordRestClient, BotGuildsService botGuildsService)
    {
        _discordRestClient = discordRestClient;
        _botGuildsService = botGuildsService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //if (!_env.IsDevelopment())
        //{
        var discordId = context.User.GetDiscordId();

        if (!discordId.HasValue)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        var discordAuthed = context.User.Claims.FirstOrDefault(x => x.Type == "discord_authed");
        if (discordAuthed is not null)
        {
            await next(context);
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
        else
        {
            context.User.Identities.First().AddClaim(new Claim("discord_authed", "true"));
        }
        //}

        await next(context);
    }
}
