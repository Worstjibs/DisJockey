using System.Text;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions {
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
            .AddCookie(options => {
                options.LoginPath = "/signin";
                options.ExpireTimeSpan = new System.TimeSpan(7, 0, 0, 0);
            })
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
            // .AddOAuth("Discord", options => {
            //     // Add configuration for Discord Authentication
            //     options.AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";

            //     // Add identify scope to access the Discord Id of the user
            //     options.Scope.Add("identify");

            //     options.CallbackPath = new PathString("/account/discord");

            //     // Add Client Id and Secret from Discord Application
            //     options.ClientId = config.GetValue<string>("Discord:ClientId");
            //     options.ClientSecret = config.GetValue<string>("Discord:ClientSecret");

            //     // Add Endpoints from Discord application
            //     options.TokenEndpoint = "https://discord.com/api/oauth2/token";
            //     options.UserInformationEndpoint = "https://discord.com/api/users/@me";

            //     options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            //     options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
            // });
            
            return services;
        }
    }
}