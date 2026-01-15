using DisJockey.BotService;
using DisJockey.Messaging;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Events;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDiscordServices(builder.Configuration);

builder.UseWolverine(options =>
{
    options.ListenToRabbitQueue("play-track-queue");

    options.PublishMessage<TrackPlayedEvent>()
            .ToRabbitExchange("track-played-exchange", config =>
            {
                config.BindQueue("track-played-queue");
            });

    options.UseRabbitMqUsingNamedConnection("rabbit-mq").AutoProvision();
});

builder.Services.AddScoped<IMessageSender, MessageSender>();

var host = builder.Build();

host.Run();
