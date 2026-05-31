using ImTools;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Model;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace DisJockey.Messaging;

public static class HostApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddWolverine(
            Action<WolverineOptions> configureOptions,
            Action<RabbitMqTransportExpression>? configureRabbitMq = null,
            Assembly? consumerAssembly = null)
        {
            builder.UseWolverine(options =>
            {
                var rabbitMq = options.UseRabbitMqUsingNamedConnection("rabbit-mq");
                if (configureRabbitMq is not null)
                {
                    configureRabbitMq(rabbitMq);
                }

                rabbitMq.AutoProvision();

                configureOptions(options);

                if (consumerAssembly is not null)
                {
                    options.ApplicationAssembly = consumerAssembly;
                }

                TypeLoadMode loadMode = builder.Environment.IsProduction()
                                            ? TypeLoadMode.Static
                                            : TypeLoadMode.Dynamic;

                options.CodeGeneration.TypeLoadMode = loadMode;

                options.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
            });

            return builder;
        }
    }
}
