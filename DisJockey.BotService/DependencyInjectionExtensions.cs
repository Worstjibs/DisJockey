using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DisJockey.BotService.Consumers;
using DisJockey.BotService.Interactions;
using DisJockey.BotService.Services;
using DisJockey.BotService.Services.Music;
using DisJockey.BotService.Services.WheelUp;
using DisJockey.Messaging;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Events;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players.Queued;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace DisJockey.BotService;

public static class ServiceCollectionExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddWolverine()
        {
            builder.AddWolverine(
                options =>
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
                },
                rabbitMqOptions =>
                {
                    rabbitMqOptions
                        .DeclareQueue("user-voice-state-changed-queue", q => q.QueueType = QueueType.stream)
                        .DeclareQueue("track-status-changed-queue", q => q.QueueType = QueueType.stream)
                        .DeclareQueue("track-progress-queue", q => q.QueueType = QueueType.stream);
                },
                typeof(PlayTrackEventConsumer).Assembly);

            builder.Services.AddScoped<IMessageSender, MessageSender>();

            return builder;
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddDiscordServices(IConfiguration configuration)
        {
            services.Configure<BotSettings>(configuration.GetSection("BotSettings"));

            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Debug,
                GatewayIntents = GatewayIntents.All,
                UseInteractionSnowflakeDate = true
            }));

            services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
            services.AddSingleton<InteractionHandler>();

            services.AddLavalink4NetServices(configuration);

            services.AddHostedService<HostedBotService>();

            services.AddScoped<IUserVoiceStateService, UserVoiceStateService>();

            return services;
        }

        private IServiceCollection AddLavalink4NetServices(IConfiguration configuration)
        {
            services.AddLavalink();
            services.ConfigureLavalink(config =>
            {
                config.BaseAddress = new Uri(configuration.GetConnectionString("lavalink")!);
                config.ReadyTimeout = TimeSpan.FromMinutes(1);
            });

            services.Configure<QueuedLavalinkPlayerOptions>(x => new QueuedLavalinkPlayerOptions());

            services.AddSingleton<IMusicService, MusicService>();
            services.AddSingleton<WheelUpService>();

            return services;
        }
    }
}
