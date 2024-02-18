using DisJockey.BotService;
using MassTransit;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordServices(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(Assembly.GetExecutingAssembly());

    x.UsingSqlServer((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<SqlTransportOptions>(builder.Configuration.GetSection("MassTransitSettings:SqlSettings"));

var migrationSettings = builder.Configuration.GetRequiredSection("MassTransitSettings:MigrationSettings").Get<MigrationSettings>()!;

builder.Services.AddSqlServerMigrationHostedService(create: migrationSettings.Create, delete: migrationSettings.Delete);

var host = builder.Build();

host.Run();
