using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using DisJockey.Application;
using DisJockey.Extensions;
using DisJockey.Infrastructure;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Middleware;
using DisJockey.Endpoints;
using Microsoft.Extensions.Configuration;
using DisJockey.Hubs;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Seed.json");
}

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddCors();

builder.AddInfrastructureServices();

builder.Services.AddApplicationServicesV2()
        .AddIdentityServices(builder.Configuration)
        .AddDiscordServices(builder.Configuration);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapDefaultEndpoints();

Configure(app, app.Environment);

await MigrateDatabase(app.Services, app.Configuration);

await app.RunAsync();

void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    if (!env.IsEnvironment("Testing"))
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapDisJockeyEndpoints();
        endpoints.MapFallbackToController("Index", "Fallback");

        endpoints.MapHub<TrackControlHub>("/hubs/track-control");
    });
}

async Task MigrateDatabase(IServiceProvider serviceProvider, IConfiguration configuration)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DataContext>();

        await context.Database.MigrateAsync();

        var env = services.GetRequiredService<IHostEnvironment>();
        if (env.IsDevelopment())
        {
            await Seed.SeedData(context, configuration);
        }
    }
    catch (Exception e)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(e, "An error occured during migraiton");
    }
}
