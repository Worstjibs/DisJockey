using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MassTransit;

namespace DisJockey.MassTransit;

public static class ServicesExtensions
{
    public static IServiceCollection AddMassTransit(
        this IServiceCollection services,
        IConfiguration config,
        Assembly[] assemblies
    )
    {
        var rabbitMqConnectionString = config.GetConnectionString("rabbit-mq");

        services.AddMassTransit(x =>
        {
            x.AddConsumers(assemblies);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);

                cfg.Host(rabbitMqConnectionString);
            });
        });

        return services;
    }
}
