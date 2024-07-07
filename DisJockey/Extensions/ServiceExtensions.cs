using Discord.Rest;
using DisJockey.Application.YouTube;
using DisJockey.Infrastructure;
using DisJockey.Middleware;
using DisJockey.Profiles;
using DisJockey.Services;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DisJockey.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(configuration =>
        {
            configuration.AddProfile<AutoMapperProfiles>();
        });

        services.Configure<YoutubeSettings>(config.GetSection("YoutubeSettings"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<ITrackRepository, TrackRepository>();

        services.AddScoped<IVideoDetailService, VideoDetailService>();

        var dbConnectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(dbConnectionString);
        });

        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.Load("DisJockey.Application")));

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        var authenticationSettings = config.GetSection("AuthenticationSettings").Get<AuthenticationSettings>();
        services.AddSingleton(authenticationSettings);

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddDiscord(options =>
        {
            options.ClientId = authenticationSettings.DiscordClientId;
            options.ClientSecret = authenticationSettings.DiscordClientSecret;
            options.Scope.Add("guilds");

            options.Events.OnCreatingTicket = context =>
            {
                context.Identity.AddClaim(new Claim("discord_token", context.AccessToken));

                return Task.CompletedTask;
            };
        });

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
