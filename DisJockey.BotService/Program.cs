using DisJockey.BotService;
using DisJockey.MassTransit;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDiscordServices(builder.Configuration);

builder.Services.AddMassTransit(
                        builder.Configuration,
                        [Assembly.GetExecutingAssembly()]);

var host = builder.Build();

host.Run();
