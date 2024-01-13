using DisJockey.BotService;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordServices(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.UsingSqlServer((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<SqlTransportOptions>(builder.Configuration.GetSection("MassTransitSettings:SqlSettings"));

builder.Services.AddSqlServerMigrationHostedService(create: true, delete: false);

var host = builder.Build();

host.Run();
