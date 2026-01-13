using DisJockey.Shared.DTOs;
using DisJockey.Shared.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace DisJockey.Bff.Endpoints;

public static class AuthEndpointExtensions
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapAuthEndpoints()
        {
            builder.MapGet("/account/userinfo", async (HttpContext context, ClaimsPrincipal user) =>
            {
                if (!user.Identity?.IsAuthenticated ?? false)
                {
                    return Results.NoContent();
                }

                var discordId = user.GetDiscordId();
                if (!discordId.HasValue)
                {
                    return Results.NoContent();
                }

                var username = user.GetUsername();
                var avatarUrl = user.GetAvatarUrl();

                var userDto = new UserDto
                {
                    AvatarUrl = avatarUrl,
                    DiscordId = discordId.Value.ToString(),
                    Username = username
                };

                return Results.Ok(userDto);
            });

            builder.MapGet("/account/login", async (HttpContext context) =>
            {
                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = "https://localhost:7230/signin-oidc"
                };

                await context.ChallengeAsync(authProperties);
            });

            builder.MapGet("/account/logout", async (HttpContext context) =>
            {
                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = "https://localhost:7230/"
                };

                await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, authProperties);
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, authProperties);
            });

            return builder;
        }
    }
}
