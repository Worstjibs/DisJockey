using DisJockey.Application.Interfaces;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Infrastructure.Persistence.Repositories;
using DisJockey.Infrastructure.YouTube;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Helpers;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DisJockey.Infrastructure;

public static class HostBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddInfrastructureServices()
        {
            builder.AddSqlServerDbContext<DataContext>("disjockey-db");

            var youtubeSettings = builder.Configuration.GetSection("YoutubeSettings").Get<YoutubeSettings>()
                ?? throw new Exception("Youtube Configuration must be provided in appsettings");

            builder.Services.AddScoped<YouTubeService>(
                _ => new(new BaseClientService.Initializer()
                {
                    ApiKey = youtubeSettings.ApiKey
                }));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            builder.Services.AddScoped<ITrackRepository, TrackRepository>();

            builder.Services.AddScoped<IVideoDetailService, VideoDetailService>();

            return builder;
        }
    }
}
