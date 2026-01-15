using Discord.Rest;
using DisJockey.Application.Interfaces;
using DisJockey.Application.Services;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Infrastructure.Persistence.Repositories;
using DisJockey.Infrastructure.YouTube;
using DisJockey.Middleware;
using DisJockey.Services;
using DisJockey.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }

    public static IServiceCollection AddDiscordServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IDiscordTrackService, DiscordTrackService>();

        services.AddScoped<DiscordRestClient>();
        services.AddScoped<IAuthorizationMiddlewareResultHandler, DisJockeyAuthorizationMiddlewareResultHandler>();
        services.AddSingleton<BotGuildsService>();
        services.AddHostedService<BotGuildsScheduledService>();

        return services;
    }
}
