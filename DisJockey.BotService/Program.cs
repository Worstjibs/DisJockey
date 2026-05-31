using DisJockey.BotService;
using DisJockey.BotService.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();

builder.Services.AddDiscordServices(builder.Configuration);

builder.AddWolverine();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<DisJockeyGrpcService>();

app.Run();
