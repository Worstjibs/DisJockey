using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DisJockey.Api.Integration.Tests;

public static class JwtTokenProvider
{
    internal static string Key { get; } = "NAJk1Is1ttYldciuGRxDrLS6h9On05nDpTmDAElnjW15AJ1cuS4b6s0F8DhmTgpM";
    internal static string Issuer { get; } = "NAAFITrack.Tests";
    internal static SecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

    public static void ConfigureTestJwt(this IServiceCollection services)
    {
        services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters.ValidIssuer = Issuer;
                options.TokenValidationParameters.ValidAudience = Issuer;

                options.Configuration = new();

                options.Configuration.SigningKeys.Add(SecurityKey);
            });
    }
}
