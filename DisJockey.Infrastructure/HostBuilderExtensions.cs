using DisJockey.Application.Consumers;
using DisJockey.Application.Contracts;
using DisJockey.Application.Interfaces;
using DisJockey.Infrastructure.Clients;
using DisJockey.Infrastructure.Hubs;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Infrastructure.Persistence.Repositories;
using DisJockey.Infrastructure.YouTube;
using DisJockey.Messaging;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Helpers;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Events;
using DisJockey.Shared.Protos;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace DisJockey.Infrastructure;

public static class HostBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddInfrastructureServices()
        {
            builder.AddSqlServerDbContext<DataContext>("disjockey-db");

            var youtubeSettings = builder.Configuration.GetSection("YoutubeSettings").Get<YoutubeSettings>()
                ?? throw new Exception("Youtube Configuration must be provided in appsettings");

            builder.Services.AddScoped<YouTubeService>(
                _ => new(new BaseClientService.Initializer()
                {
                    ApiKey = youtubeSettings.ApiKey
                }));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            builder.Services.AddScoped<ITrackRepository, TrackRepository>();

            builder.Services.AddScoped<IVideoDetailService, VideoDetailService>();

            builder.AddWolverine();

            builder.Services.AddScoped<IMessageSender, MessageSender>();

            builder.Services.AddSingleton<IUserIdProvider, DiscordUserIdProvider>();
            builder.Services.AddSingleton<IUserConnectionTracker, UserConnectionTracker>();

            builder.Services.AddScoped<INotifier, HubNotifier>();

            builder.Services.AddTransient<IBotServiceClient, BotServiceClient>();
            builder.Services.AddGrpcClient<DisJockeyGrpc.DisJockeyGrpcClient>(o =>
            {
                o.Address = new Uri("http://bot");
            });

            return builder;
        }

        private IHostApplicationBuilder AddWolverine()
        {
            builder.AddWolverine(
                options =>
                {
                    options.PublishMessage<PlayTrackEvent>().ToRabbitExchange("play-track-exchange", config =>
                    {
                        config.BindQueue("play-track-queue");
                    });

                    options.ListenToRabbitQueue("track-played-queue");
                    options.ListenToRabbitQueue(
                        "user-voice-state-changed-queue",
                        q =>
                        {
                            q.QueueType = QueueType.stream;
                        });

                    options.ListenToRabbitQueue(
                        "track-status-changed-queue",
                        q =>
                        {
                            q.QueueType = QueueType.stream;
                        });

                    options.ListenToRabbitQueue(
                        "track-progress-queue",
                        q =>
                        {
                            q.QueueType = QueueType.stream;
                        });
                }, 
                consumerAssembly: typeof(TrackPlayedEventConsumer).Assembly);

            return builder;
        }
    }
}
