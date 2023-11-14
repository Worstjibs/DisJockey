using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DisJockey.BotService;
using DisJockey.BotService.Services;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players.Vote;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<BotSettings>(builder.Configuration.GetSection("BotSettings"));

builder.Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
{
    AlwaysDownloadUsers = true,
    LogLevel = LogSeverity.Debug,
    GatewayIntents = GatewayIntents.All,
    UseInteractionSnowflakeDate = true
}));

builder.Services.AddSingleton<InteractionService>();
builder.Services.AddSingleton<InteractionHandler>();

builder.Services.AddLavalink();
builder.Services.ConfigureLavalink(config =>
{
    config.BaseAddress = new Uri("http://lavalink:2333");
});

builder.Services.Configure<QueuedLavalinkPlayerOptions>(x => new QueuedLavalinkPlayerOptions());

builder.Services.AddSingleton<IMusicService, MusicService>();

builder.Services.AddHostedService<HostedBotService>();

var host = builder.Build();

host.Run();
