using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions {
    public static class IdentityServiceExtensions {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config) {
            var tokenKey = config.GetValue<string>("TokenKey");
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("TokenKey")))
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