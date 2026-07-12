using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using nShop.Catalog.DomainEvents;

namespace nShop.Catalog.ProjectionBus.Kafka;

public class KafkaProjectionBus : IProjectionBus
{
    private readonly string topic;
    private readonly IProducer<string, string> producer;
    private readonly ILogger<KafkaProjectionBus> logger;

    public KafkaProjectionBus(string topic, IProducer<string, string> producer, ILogger<KafkaProjectionBus> logger)
    {
        this.topic = topic ?? throw new ArgumentNullException(nameof(topic));
        this.producer = producer;
        this.logger = logger;
    }

    public Task PublishAsync(ICatalogEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            producer.Produce(topic,
                new Message<string, string>
                    { Key = @event.GetType().FullName!, Value = JsonSerializer.Serialize(@event, @event.GetType()) });

            logger.LogInformation("Published event {@event}", @event);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing event {@event}", @event);

            return Task.FromException(ex);
        }
    }

    public Task PublishAsync(IEnumerable<ICatalogEvent> events, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var @event in events)
            {
                producer.Produce(topic,
                    new Message<string, string>
                    {
                        Key = @event.GetType().FullName!, Value = JsonSerializer.Serialize(@event, @event.GetType())
                    });

                logger.LogInformation("Published event {@event}", @event);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing events {@events}", events);

            return Task.FromException(ex);
        }
    }
}