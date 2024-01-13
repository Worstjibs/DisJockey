using Discord.Rest;
using DisJockey.Extensions;
using DisJockey.Middleware;
using DisJockey.Services;
using DisJockey.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddMassTransit(x =>
            {
                x.AddConsumers(Assembly.GetExecutingAssembly());

                x.UsingSqlServer((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddOptions<SqlTransportOptions>().Configure(options =>
            {
                options.Host = "mssql";
                options.Database = "disjockey";
                options.Schema = "transport";
                options.Role = "transport";
                options.Username = "masstransit";
                options.Password = "5itBXrOpu@f8OUcE7%0!";
                options.AdminUsername = "sa";
                options.AdminPassword = "Sn39QAi9h1htMYFI79Mf";
            });

            services.AddScoped<IDiscordTrackService, DiscordTrackService>();

            //services.AddDiscordServices(_config);
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
