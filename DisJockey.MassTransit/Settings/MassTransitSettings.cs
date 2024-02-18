#nullable disable
namespace DisJockey.MassTransit.Settings;

public class MassTransitSettings
{
    public string ServiceBusConnectionString { get; set; }
    public string RabbitMqHost { get; set; }
}
