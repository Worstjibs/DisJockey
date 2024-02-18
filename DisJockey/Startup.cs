using DisJockey.Extensions;
using DisJockey.Middleware;
using DisJockey.Services;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Settings;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DisJockey
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
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

            var massTransitSettings = _config.GetRequiredSection("MassTransitSettings").Get<MassTransitSettings>()!;
            services.AddMassTransit(x =>
            {
                x.AddConsumers(Assembly.GetExecutingAssembly());

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);

                    cfg.Host(massTransitSettings.ServiceBusConnectionString);
                });
            });

            services.AddScoped<IDiscordTrackService, DiscordTrackService>();
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
