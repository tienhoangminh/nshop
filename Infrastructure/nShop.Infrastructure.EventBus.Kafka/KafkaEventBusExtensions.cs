using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nShop.Core.IntegrationEvent;

namespace nShop.Infrastructure.EventBus.Kafka;

public static class KafkaEventBusExtensions
{
    public static void AddKafkaEventBus(this IServiceCollection services, string topic)
    {
        services.AddTransient<IEventBus>(serviceProvider => new KafkaEventBus(
            topic,
            serviceProvider.GetRequiredService<IProducer<string, string>>(), 
            serviceProvider.GetRequiredService<ILogger<KafkaEventBus>>()
        ));
    }
}