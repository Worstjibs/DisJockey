using DisJockey.BotService;
using DisJockey.MassTransit;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordServices(builder.Configuration);

builder.Services.AddMassTransit(
                        builder.Configuration,
                        builder.Environment,
                        [Assembly.GetExecutingAssembly()]);

var host = builder.Build();

host.Run();
