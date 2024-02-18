using DisJockey.MassTransit.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace DisJockey.MassTransit;

public static class ServicesExtensions
{
    public static IServiceCollection AddMassTransit(
        this IServiceCollection services,
        IConfiguration config,
        IHostEnvironment environment,
        Assembly[] assemblies
    )
    {
        var massTransitSettings = config.GetRequiredSection("MassTransitSettings").Get<MassTransitSettings>()!;

        services.AddMassTransit(x =>
        {
            x.AddConsumers(assemblies);

            if (environment.IsDevelopment())
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);

                    cfg.Host(massTransitSettings.RabbitMqHost);
                });
            }
            else
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);

                    cfg.Host(massTransitSettings.ServiceBusConnectionString);
                });
            }
        });

        return services;
    }
}
