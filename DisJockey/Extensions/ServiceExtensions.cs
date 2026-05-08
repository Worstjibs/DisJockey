using System.Threading.Tasks;
using Discord.Rest;
using DisJockey.Application.Interfaces;
using DisJockey.Application.Services;
using DisJockey.Hubs;
using DisJockey.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // SignalR WebSocket/SSE connections cannot send Authorization headers,
                // so the token is passed as a query parameter instead.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddSingleton<IUserIdProvider, DiscordUserIdProvider>();

        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }

    public static IServiceCollection AddDiscordServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IDiscordTrackService, DiscordTrackService>();

        services.AddScoped<DiscordRestClient>();
        services.AddScoped<IAuthorizationMiddlewareResultHandler, DisJockeyAuthorizationMiddlewareResultHandler>();
        services.AddSingleton<BotGuildsService>();

        //services.AddHostedService<BotGuildsScheduledService>();

        return services;
    }
}
