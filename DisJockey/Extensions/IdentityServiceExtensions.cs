using System.Security.Claims;
using System.Threading.Tasks;
using DisJockey.Middleware;
using DisJockey.Services;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Extensions
{
    public static class IdentityServiceExtensions
    {
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
    }
}