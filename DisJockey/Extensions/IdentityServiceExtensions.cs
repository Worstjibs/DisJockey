using System.Threading.Tasks;
using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Extensions {
    public static class IdentityServiceExtensions {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config) {
            var authenticationSettings = config.GetSection("AuthenticationSettings").Get<AuthenticationSettings>();
            services.AddSingleton<AuthenticationSettings>(authenticationSettings);
            
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            // .AddJwtBearer(options => {
            //     options.TokenValidationParameters = new TokenValidationParameters {
            //         ValidateAudience = false,
            //         ValidateIssuer = false,
            //         ValidateIssuerSigningKey = true,
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtTokenKey))
            //     };
            // })
            .AddCookie()
            .AddDiscord(options => {
                options.ClientId = authenticationSettings.DiscordClientId;
                options.ClientSecret = authenticationSettings.DiscordClientSecret;
                
                options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents {
                    OnCreatingTicket = ticketContext => {
                        var context = ticketContext;

                        return Task.CompletedTask;
                    }
                };
            });
            
            return services;
        }
    }
}