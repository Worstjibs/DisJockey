using DisJockey.AppHost.Lavalink;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
            .WithLifetime(ContainerLifetime.Persistent);

var database = sql.AddDatabase("disjockey-db");

var rabbitMq = builder.AddRabbitMQ("rabbit-mq")
            .WithLifetime(ContainerLifetime.Persistent);

var lavalink = builder.AddLavalinkServer("lavalink")
            .WithLifetime(ContainerLifetime.Persistent);

await GetDiscordProvider();

var keycloak = builder.AddKeycloak("keycloak", 8080)
            .WithBindMount("./providers", "/opt/keycloak/providers")
            .WithDataVolume("keycloak-data")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithOtlpExporter();

#pragma warning disable ASPIRECERTIFICATES001
keycloak.WithoutHttpsCertificate();
#pragma warning restore ASPIRECERTIFICATES001

var api = builder.AddProject<Projects.DisJockey>("api")
            .WithReference(database)
            .WaitFor(database)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq);

var bot = builder.AddProject<DisJockey_BotService>("bot")
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithReference(lavalink);

builder.AddProject<DisJockey_Bff>("bff")
            .WithReference(api)
            .WaitFor(api)
            .WithReference(keycloak)
            .WaitFor(keycloak);

await builder.Build().RunAsync();

async Task GetDiscordProvider()
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