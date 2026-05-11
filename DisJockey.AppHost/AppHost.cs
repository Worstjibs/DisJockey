using Projects;

var builder = DistributedApplication.CreateBuilder(args);

AddMonitoringServices(builder);

var sql = builder.AddSqlServer("sql")
            .WithDataVolume("sql-data")
            .WithLifetime(ContainerLifetime.Persistent);

var database = sql.AddDatabase("disjockey-db");

var rabbitMq = builder.AddRabbitMQ("rabbit-mq")
                .WithManagementPlugin(61532)
                .WithDataVolume("rabbit-mq-data")
                .WithLifetime(ContainerLifetime.Persistent);

var lavalink = builder.AddLavalinkServer("lavalink")
            .WithLifetime(ContainerLifetime.Persistent);

await GetDiscordProvider();

var keycloak = AddKeycloak(builder);

var api = builder.AddProject<DisJockey>("api")
            .WithReference(database)
            .WaitFor(database)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq);

var bot = builder.AddProject<DisJockey_BotService>("bot")
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithReference(lavalink)
            .WaitFor(keycloak)
            .WithReference(keycloak);

api.WithReference(bot);

var bffClientId = builder.AddParameter("bff-client-id");
var bffClientSecret = builder.AddParameter("bff-client-secret");

var bff = builder.AddProject<DisJockey_Bff>("bff")
            .WithExternalHttpEndpoints()
            .WithReference(api)
            .WaitFor(api)
            .WithReference(keycloak)
            .WaitFor(keycloak)
            .WithEnvironment("ClientId", bffClientId)
            .WithEnvironment("ClientSecret", bffClientSecret);

if (builder.ExecutionContext.IsPublishMode)
{
    bff.WithEndpoint("http", e => e.Port = 8080);
}

await builder.Build().RunAsync();

static async Task GetDiscordProvider()
{
    const string localFilePath = "./providers/keycloak-discord-0.6.1.jar";

    if (File.Exists(localFilePath))
    {
        return;
    }

    const string discordProviderJar = "https://github.com/iForged/keycloak-discord/releases/download/v1.3.1/keycloak-discord-1.3.1.jar";

    using var httpClient = new HttpClient();
    var response = await httpClient.GetAsync(discordProviderJar);

    using var localStream = File.OpenWrite(localFilePath);

    await response.Content.CopyToAsync(localStream);

    await localStream.FlushAsync();
}

static IResourceBuilder<KeycloakResource> AddKeycloak(IDistributedApplicationBuilder builder)
{
    var keycloak = builder.AddKeycloak("keycloak", 8080)
                    .WithBindMount("./providers", "/opt/keycloak/providers")
                    .WithRealmImport("./realms")
                    .WithDataVolume("keycloak-data")
                    .WithLifetime(ContainerLifetime.Persistent);

#pragma warning disable ASPIRECERTIFICATES001
    keycloak.WithoutHttpsCertificate();
#pragma warning restore ASPIRECERTIFICATES001

    if (builder.ExecutionContext.IsPublishMode)
    {
        // Keycloak will exist behind a proxy, so we can safely disable https
        keycloak.WithEnvironment("KC_HTTP_ENABLED", "true");
        keycloak.WithEnvironment("KC_PROXY_HEADERS", "xforwarded");
        keycloak.WithEnvironment("KC_HOSTNAME_STRICT", "false");

    }

    return keycloak;
}

static void AddMonitoringServices(IDistributedApplicationBuilder builder)
{
    if (builder.ExecutionContext.IsRunMode)
    {
        return;
    }

    var jaeger = builder.AddContainer("jaeger", "jaegertracing/all-in-one")
            .WithHttpEndpoint(16686, targetPort: 16686, name: "portal")
            .WithHttpEndpoint(4319, targetPort: 4317, "otel")
            .WithExternalHttpEndpoints();

    var seq = builder.AddSeq("seq");

    var otelCollector = builder.AddOpenTelemetryCollector("otel-collector")
        .WithAppForwarding()
        .WithConfig("./collector-config.yml")
        .WithEnvironment("JAEGER_ENDPOINT", jaeger.GetEndpoint("otel"))
        .WithEnvironment("SEQ_ENDPOINT", $"{seq.GetEndpoint("http")}/ingest/otlp");

    if (builder.ExecutionContext.IsPublishMode)
    {
        otelCollector.WithEnvironment(context =>
        {
            // Aspire Dashboard is disabled at publish, so we don't want these in the compose file
            context.EnvironmentVariables.Remove("ASPIRE_ENDPOINT");
            context.EnvironmentVariables.Remove("ASPIRE_API_KEY");
        });
    }
}
