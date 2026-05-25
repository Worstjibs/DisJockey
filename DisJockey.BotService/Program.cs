using DisJockey.BotService;
using DisJockey.BotService.Grpc;
using DisJockey.Messaging;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Events;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();

builder.Services.AddDiscordServices(builder.Configuration);

builder.UseWolverine(options =>
{
    options.ListenToRabbitQueue("play-track-queue");

    options.PublishMessage<TrackPlayedEvent>()
            .ToRabbitExchange("track-played-exchange", config =>
            {
                config.BindQueue("track-played-queue");
            });

    options.PublishMessage<UserVoiceStateChangedEvent>()
            .ToRabbitExchange("user-voice-state-changed-exchange", config =>
            {
                config.BindQueue("user-voice-state-changed-queue");
            });

    options.PublishMessage<TrackStatusChangedEvent>()
            .ToRabbitExchange("track-status-changed-exchange", config =>
            {
                config.BindQueue("track-status-changed-queue");
            });

    options.PublishMessage<TrackProgressEvent>()
            .ToRabbitExchange("track-progress-exchange", config =>
            {
                config.BindQueue("track-progress-queue");
            });

    options.UseRabbitMqUsingNamedConnection("rabbit-mq")
        .DeclareQueue("user-voice-state-changed-queue", q => q.QueueType = QueueType.stream)
        .DeclareQueue("track-status-changed-queue", q => q.QueueType = QueueType.stream)
        .DeclareQueue("track-progress-queue", q => q.QueueType = QueueType.stream)
        .AutoProvision();
});

builder.Services.AddScoped<IMessageSender, MessageSender>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<DisJockeyGrpcService>();

app.Run();
