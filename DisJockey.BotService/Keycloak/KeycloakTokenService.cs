using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace DisJockey.BotService.Keycloak;

internal sealed class KeycloakTokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KeycloakSettings _settings;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private string? _cachedToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;

    public KeycloakTokenService(IHttpClientFactory httpClientFactory, IOptions<KeycloakSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null && DateTimeOffset.UtcNow < _tokenExpiry)
            return _cachedToken;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cachedToken is not null && DateTimeOffset.UtcNow < _tokenExpiry)
                return _cachedToken;

            return await RefreshAsync(cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<string?> RefreshAsync(CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret
        };

        var response = await client.PostAsync(
            "https+http://keycloak/realms/master/protocol/openid-connect/token",
            new FormUrlEncodedContent(form),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken: cancellationToken);

        _cachedToken = tokenResponse!.AccessToken;
        // Refresh 30 seconds before actual expiry to avoid races
        _tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 30);

        return _cachedToken;
    }

    private sealed record KeycloakTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);
}
