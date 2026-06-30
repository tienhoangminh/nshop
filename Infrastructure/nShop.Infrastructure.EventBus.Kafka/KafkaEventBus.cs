using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using nShop.Core.IntegrationEvent;

namespace nShop.Infrastructure.EventBus.Kafka;

public class KafkaEventBus(
    string topic,
    IProducer<string, string> producer,
    ILogger<KafkaEventBus> logger) : IEventBus
{
    private readonly string _topic = topic ?? throw new ArgumentNullException(nameof(topic));

    public Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new Message<string, string>
                { Key = @event.GetType().FullName!, Value = JsonSerializer.Serialize(@event, @event.GetType()) };
            return producer.ProduceAsync(_topic, message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing event {@event}", @event);

            return Task.FromException(ex);
        }
    }

    public async Task PublishAsync(IEnumerable<IIntegrationEvent> events, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var @event in events)
            {
                await producer.ProduceAsync(topic, new Message<string, string> { Key = @event.GetType().FullName!, Value = JsonSerializer.Serialize(@event, @event.GetType()) }, cancellationToken);

                logger.LogInformation("Published event {@event}", @event);
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing events {@events}", events);
        }
    }
}