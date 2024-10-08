using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using DisJockey.Extensions;
using DisJockey.Infrastructure.Persistence;
using DisJockey.MassTransit;
using DisJockey.Middleware;
using DisJockey.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureKeyVault();

ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();

Configure(app, app.Environment);

await MigrateDatabase(app.Services);

await app.RunAsync();

void ConfigureServices(IServiceCollection services, IConfiguration config, IHostEnvironment environment)
{
    services.AddControllers();

    services.AddCors();

    services.AddApplicationServices(config)
            .AddIdentityServices(config)
            .AddDiscordServices(config)
            .AddMassTransit(
                    config,
                    environment,
                    [Assembly.GetExecutingAssembly()]);
}

void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapFallbackToController("Index", "Fallback");
    });
}

async Task MigrateDatabase(IServiceProvider serviceProvider)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<DataContext>();

            await context.Database.MigrateAsync();

            var env = services.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                await Seed.SeedData(context);
            }
        }
        catch (Exception e)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(e, "An error occured during migraiton");
        }
    }
}
