using DisJockey.Data;
using DisJockey.Services;
using DisJockey.Services.YouTube;
using DisJockey.Shared.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DisJockey.Services.Interfaces;

namespace DisJockey.Extensions {
    public static class ApplicationServiceExtensions {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration config) {
            var autoMapperProfile = typeof(AutoMapperProfiles).Assembly;
            services.AddAutoMapper(autoMapperProfile);

            services.Configure<YoutubeSettings>(config.GetSection("YoutubeSettings"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVideoDetailService, VideoDetailService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddDbContext<DataContext>(options => {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });
        }
    }
}