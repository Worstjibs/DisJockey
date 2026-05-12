namespace DisJockey.BotService.Keycloak;

internal sealed class KeycloakSettings
{
    public required string Authority { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}
