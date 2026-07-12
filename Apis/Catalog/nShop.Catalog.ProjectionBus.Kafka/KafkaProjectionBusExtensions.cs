using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace nShop.Catalog.ProjectionBus.Kafka;

public static class KafkaProjectionBusExtensions
{
    public static void AddKafkaProjectionBus(this IServiceCollection services, string topic)
    {
        services.AddTransient<IProjectionBus>(services => new KafkaProjectionBus(
            topic,
            services.GetRequiredService<IProducer<string, string>>(), 
            services.GetRequiredService<ILogger<KafkaProjectionBus>>()
        ));
    }
}