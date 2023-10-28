using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DisJockey.BotService;

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

builder.Services.AddHostedService<HostedBotService>();

var host = builder.Build();

host.Run();
