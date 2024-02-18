using DisJockey.BotService;
using DisJockey.Shared.Settings;
using MassTransit;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordServices(builder.Configuration);

var massTransitSettings = builder.Configuration.GetRequiredSection("MassTransitSettings").Get<MassTransitSettings>()!;
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(Assembly.GetExecutingAssembly());

    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);

        cfg.Host(massTransitSettings.ServiceBusConnectionString);
    });
});

var host = builder.Build();

host.Run();
