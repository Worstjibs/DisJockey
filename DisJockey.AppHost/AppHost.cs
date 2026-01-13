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

var api = builder.AddProject<Projects.DisJockey>("api")
            .WithReference(database)
            .WaitFor(database)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq);

var bot = builder.AddProject<DisJockey_BotService>("bot")
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithReference(lavalink);

builder.Build().Run();
