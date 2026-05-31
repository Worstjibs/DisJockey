using DisJockey.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DisJockey.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.RegisterHandlers();

            return services;
        }

        private IServiceCollection RegisterHandlers()
        {
            services.AddScoped<IMediator, Mediator>();

            typeof(IRequestHandler<,>).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .ToList()
                .ForEach(x =>
                {
                    services.AddScoped(x.Interface, x.Type);
                });

            return services;
        }
    }
}
