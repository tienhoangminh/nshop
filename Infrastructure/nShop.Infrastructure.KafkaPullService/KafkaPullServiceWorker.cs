using System.Text.Json;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace nShop.Infrastructure.KafkaPullService;

public class KafkaPullServiceWorker(
    IOptions<KafkaPullServiceWorkerOptions> options,
    IMediator mediator,
    ILoggerFactory loggerFactory) : BackgroundService
{
    private readonly ILogger _logger = string.IsNullOrEmpty(options.Value.ServiceName) ? loggerFactory.CreateLogger<KafkaPullServiceWorker>() : loggerFactory.CreateLogger(options.Value.ServiceName);
    private readonly KafkaPullServiceWorkerOptions _options = options.Value;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Connection to Kafka...");
            
                var config = new ConsumerConfig
                {
                    BootstrapServers = _options.BootstrapServers,
                    GroupId = _options.GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };
            
                _logger.LogInformation("Trying to connect to {bs}...", config.BootstrapServers);
            
                // Run until cancellation or unexpected exit
                await ConnectAndProcessMessagesAsync(config, [_options.Topic], stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consumer exited unexpectedly.");
            }

            // Safety net: restart the consumer if it ever exits
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ConnectAndProcessMessagesAsync(ConsumerConfig config, string[] topics,
        CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Subscribing to {t}...", string.Join(", ", topics));
                consumer.Subscribe(topics);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    
                    var message = consumeResult.Message;
                    if (message != null)
                    {
                        _logger.LogInformation("Consumed message '{m}'", message.Value);
                        
                        // Process the message
                        await ProcessMessageAsync(message, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation cancelled");
            }
            catch (ConsumeException e)
            {
                _logger.LogError(e, "Error consuming message");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing message");
            }
            
            if (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    private async Task ProcessMessageAsync(Message<string, string> message, CancellationToken cancellationToken)
    {
        var eventType = GetEventType(message.Key);
        
        if (eventType == null)
        {
            _logger.LogWarning("Event type {k} not found", message.Key);
            return;
        }
        
        var evt = JsonSerializer.Deserialize(message.Value, eventType);
        if (evt != null)
        {
            // Process the event
            _logger.LogInformation("Processing event {event}", evt);
            
            var eventNotification = new EventNotification(evt);
            await mediator.Publish(eventNotification, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Error deserializing event {k}", message.Key);
        }
    }

    private Type? GetEventType(string type)
    {
        var t = Type.GetType(type);
        
        if (t == null && _options.LoadEventType != null)
        {
            t = _options.LoadEventType?.Invoke(type);
        }
        
        return t ?? AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == type);
    }
}