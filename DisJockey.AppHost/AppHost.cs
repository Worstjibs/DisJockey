using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var jaeger = builder.AddContainer("jaeger", "jaegertracing/all-in-one")
            .WithHttpEndpoint(16686, targetPort: 16686, name: "portal")
            .WithHttpEndpoint(4319, targetPort: 4317, "otel");

var seq = builder.AddSeq("seq")
            .WithLifetime(ContainerLifetime.Persistent);

builder.AddOpenTelemetryCollector("otel-collector")
        .WithAppForwarding()
        .WithConfig("./collector-config.yml")
        .WithEnvironment("JAEGER_ENDPOINT", jaeger.GetEndpoint("otel"))
        .WithEnvironment("SEQ_ENDPOINT", $"{seq.GetEndpoint("http")}/ingest/otlp");

var sql = builder.AddSqlServer("sql")
            .WithDataVolume("sql-data")
            .WithLifetime(ContainerLifetime.Persistent);

var database = sql.AddDatabase("disjockey-db");

var rabbitMq = builder.AddRabbitMQ("rabbit-mq")
                .WithManagementPlugin(61532)
                .WithDataVolume("rabbit-mq-data")
                .WithLifetime(ContainerLifetime.Persistent)
                .WithOtlpExporter();

var lavalink = builder.AddLavalinkServer("lavalink")
            .WithLifetime(ContainerLifetime.Persistent);

await GetDiscordProvider();

var keycloak = builder.AddKeycloak("keycloak", 8080)
            .WithBindMount("./providers", "/opt/keycloak/providers")
            .WithRealmImport("./realms/realm-export.json")
            .WithDataVolume("keycloak-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithOtlpExporter();

#pragma warning disable ASPIRECERTIFICATES001
keycloak.WithoutHttpsCertificate();
#pragma warning restore ASPIRECERTIFICATES001

var api = builder.AddProject<DisJockey>("api")
            .WithReference(database)
            .WaitFor(database)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq);

var bot = builder.AddProject<DisJockey_BotService>("bot")
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithReference(lavalink)
            .WithReference(keycloak)
            .WaitFor(keycloak);

api.WithReference(bot);

builder.AddProject<DisJockey_Bff>("bff")
            .WithReference(api)
            .WaitFor(api)
            .WithReference(keycloak)
            .WaitFor(keycloak);

await builder.Build().RunAsync();

static async Task GetDiscordProvider()
{
    const string localFilePath = "./providers/keycloak-discord-0.6.1.jar";

    if (File.Exists(localFilePath))
    {
        return;
    }

    const string discordProviderJar = "https://github.com/wadahiro/keycloak-discord/releases/download/v0.6.1/keycloak-discord-0.6.1.jar";

    using var httpClient = new HttpClient();
    var response = await httpClient.GetAsync(discordProviderJar);

    using var localStream = File.OpenWrite(localFilePath);

    await response.Content.CopyToAsync(localStream);

    await localStream.FlushAsync();
}