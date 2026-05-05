using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace DisJockey.Api.Integration.Tests;

public static class HttpClientExtensions
{
    public static void AsUser(this HttpClient client, string? discordId = null)
    {
        discordId ??= new Random().Next(1, int.MaxValue).ToString();

        var handler = new JsonWebTokenHandler();

        List<Claim> claims = [
            new("discord_id", discordId)
        ];

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Audience = JwtTokenProvider.Issuer,
            Issuer = JwtTokenProvider.Issuer,
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(
                JwtTokenProvider.SecurityKey,
                SecurityAlgorithms.HmacSha512Signature)
        };

        var token = handler.CreateToken(descriptor);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static void WithoutAuthentication(this HttpClient client)
        => client.DefaultRequestHeaders.Authorization = null;
}