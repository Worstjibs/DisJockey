using DisJockey.Application.Profiles;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DisJockey.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServicesV2()
        {
            services.AddAutoMapper(configuration =>
            {
                configuration.AddProfile<AutoMapperProfiles>();
            });

            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.Load("DisJockey.Application")));

            return services;
        }
    }
}
