using DisJockey.Application.Clients;
using DisJockey.Application.Contracts;
using DisJockey.Shared.Protos;
using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServicesV2()
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

            services.AddGrpcClient<DisJockeyGrpc.DisJockeyGrpcClient>(o =>
            {
                o.Address = new Uri("http://bot");
            });

            services.AddTransient<IBotServiceClient, BotServiceClient>();

            return services;
        }
    }
}
