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
using System.Threading.Tasks;

namespace DisJockey.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakJwtBearer(
                serviceName: "keycloak",
                realm: "master",
                options =>
                {
                    options.RequireHttpsMetadata = false;

                    options.Audience = "account";

                    var keycloakPublicUrl = config["KeycloakPublicUrl"];
                    if (!string.IsNullOrEmpty(keycloakPublicUrl))
                    {
                        options.TokenValidationParameters.ValidIssuer = $"{keycloakPublicUrl}/realms/master";
                    }

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
        services.AddSingleton<IUserConnectionTracker, UserConnectionTracker>();

        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }

    public static IServiceCollection AddDiscordServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IDiscordTrackService, DiscordTrackService>();

        // Disabling for now; users won't be able to play any tracks anyway
        //services.AddScoped<DiscordRestClient>();
        //services.AddScoped<IAuthorizationMiddlewareResultHandler, DisJockeyAuthorizationMiddlewareResultHandler>();
        //services.AddSingleton<BotGuildsService>();

        //services.AddHostedService<BotGuildsScheduledService>();

        return services;
    }
}
