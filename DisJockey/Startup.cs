using DisJockey.Extensions;
using DisJockey.Middleware;
using DisJockey.Services;
using DisJockey.Services.Interfaces;
using DisJockey.MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord.Rest;
using Microsoft.AspNetCore.Authorization;

namespace DisJockey
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration config, IWebHostEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Main WebApp Services (DB, Unit of Work)
            services.AddApplicationServices(_config);

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddCors();

            services.AddIdentityServices(_config);

            services.AddMassTransit(
                            _config,
                            _environment,
                            [Assembly.GetExecutingAssembly()]);

            services.AddScoped<IDiscordTrackService, DiscordTrackService>();

            services.AddScoped<DiscordRestClient>();
            services.AddScoped<IAuthorizationMiddlewareResultHandler, DisJockeyAuthorizationMiddlewareResultHandler>();
            services.AddSingleton<BotGuildsService>();
            services.AddHostedService<BotGuildsScheduledService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
    }
}
