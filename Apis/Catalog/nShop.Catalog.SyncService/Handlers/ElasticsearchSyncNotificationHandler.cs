namespace nShop.Catalog.SyncService.Handlers;

public class ElasticsearchSyncNotificationHandler : INotificationHandler<EventNotification>
{
    private readonly IMapper mapper;
    private readonly ElasticsearchClient client;
    private readonly ILogger<ElasticsearchSyncNotificationHandler> logger;

    public ElasticsearchSyncNotificationHandler(IMapper mapper, ElasticsearchClient client,
        ILogger<ElasticsearchSyncNotificationHandler> logger)
    {
        this.mapper = mapper;
        this.client = client;
        this.logger = logger;
    }

    public async Task Handle(EventNotification notification, CancellationToken cancellationToken)
    {
        // TODO: need to define cache timeout

        try
        {
            if (notification.Event is ProductCreatedEvent productCreatedEvent)
            {
                logger.LogInformation("Index product: {k}", productCreatedEvent.ProductId);
                var doc = mapper.Map<ProductIndexDocument>(productCreatedEvent);

                var response = await client.IndexAsync(doc, cancellationToken: cancellationToken);

                if (!response.IsValidResponse)
                {
                    logger.LogInformation("Error indexing product {id}, {err}", productCreatedEvent.ProductId,
                        response.ElasticsearchServerError);
                }
            }
            else if (notification.Event is ProductUpdatedEvent productUpdatedEvent)
            {
                logger.LogInformation("Update product index: {k}", productUpdatedEvent.ProductId);
                var doc = mapper.Map<ProductIndexDocument>(productUpdatedEvent);

                var response = await client.UpdateAsync<ProductIndexDocument, ProductIndexDocument>(
                    IndexName.From<ProductIndexDocument>(),
                    productUpdatedEvent.ProductId, 
                    d => d.Doc(doc), 
                    cancellationToken);

                if (!response.IsValidResponse)
                {
                    logger.LogInformation("Error updating product index {id}, {err}", productUpdatedEvent.ProductId,
                        response.ElasticsearchServerError);
                }
            }
            else if (notification.Event is ProductDeletedEvent productDeletedEvent)
            {
                logger.LogInformation("Delete product index: {k}", productDeletedEvent.ProductId);
                var response =
                    await client.DeleteAsync(productDeletedEvent.ProductId, cancellationToken: cancellationToken);

                if (!response.IsValidResponse)
                {
                    logger.LogInformation("Error deleting product index {id}, {err}", productDeletedEvent.ProductId,
                        response.ElasticsearchServerError);
                }
            }
            else
            {
                logger.LogWarning("Unsupported event type: {eventType}", notification.Event.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling event {@event}", notification.Event);
        }
    }
}