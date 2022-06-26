using DisJockey.Data;
using DisJockey.Services;
using DisJockey.Services.YouTube;
using DisJockey.Shared.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DisJockey.Services.Interfaces;
using DisJockey.Profiles;

namespace DisJockey.Extensions {
    public static class ApplicationServiceExtensions {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration config) {
            services.AddAutoMapper(configuration => {
                configuration.AddProfile<AutoMapperProfiles>();
            });

            services.Configure<YoutubeSettings>(config.GetSection("YoutubeSettings"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVideoDetailService, VideoDetailService>();

            services.AddDbContext<DataContext>(options => {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });
        }
    }
}