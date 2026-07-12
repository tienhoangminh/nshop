namespace nShop.Catalog.SyncService.Handlers;

public class ReadModelSyncNotificationHandler(
    IReadModelSyncFactory readModelSyncFactory,
    ILogger<ReadModelSyncNotificationHandler> logger) : INotificationHandler<EventNotification>
{
    public Task Handle(EventNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            if (notification.Event is ICatalogEvent catalogEvent)
            {
                var readModelSync = readModelSyncFactory.GetSyncEventHandler();
                return readModelSync.HandleAsync(catalogEvent);
            }

            logger.LogWarning("Unsupported event type: {eventType}", notification.Event.GetType().Name);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling event {@event}", notification.Event);
            throw;
        }
    }
}