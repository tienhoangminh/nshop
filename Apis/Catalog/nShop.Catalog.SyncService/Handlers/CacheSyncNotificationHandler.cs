namespace nShop.Catalog.SyncService.Handlers;

public class CacheSyncNotificationHandler : INotificationHandler<EventNotification>
    {
        private readonly IMapper mapper;
        private readonly IDistributedCache cache;
        private readonly ILogger<CacheSyncNotificationHandler> logger;

        public CacheSyncNotificationHandler(IMapper mapper, IDistributedCache cache, ILogger<CacheSyncNotificationHandler> logger)
        {
            this.mapper = mapper;
            this.cache = cache;
            this.logger = logger;
        }
        public async Task Handle(EventNotification notification, CancellationToken cancellationToken)
        {
            // TODO: need to define cache timeout

            try
            {
                if (notification.Event is ProductCreatedEvent productCreatedEvent) {
                    var key = $"{Constant.CachePrefix}{typeof(ProductDto).FullName}:{productCreatedEvent.ProductId}";
                    logger.LogInformation("Update cache item: {k}", key);

                    var value = JsonSerializer.SerializeToUtf8Bytes(mapper.Map<ProductDto>(productCreatedEvent));
                    await cache.SetAsync(key, value, token: cancellationToken);
                }
                else if (notification.Event is ProductUpdatedEvent productUpdatedEvent)
                {
                    var key = $"{Constant.CachePrefix}{typeof(ProductDto).FullName}:{productUpdatedEvent.ProductId}";
                    logger.LogInformation("Update cache item: {k}", key);

                    var value = JsonSerializer.SerializeToUtf8Bytes(mapper.Map<ProductDto>(productUpdatedEvent));
                    await cache.SetAsync(key, value, token: cancellationToken);
                }
                else if (notification.Event is ProductDeletedEvent productDeletedEvent)
                {
                    var key = $"{Constant.CachePrefix}{typeof(ProductDto).FullName}:{productDeletedEvent.ProductId}";
                    logger.LogInformation("Update cache item: {k} to empty", key);

                    await cache.SetAsync(key, [], cancellationToken);
                }
                else if (notification.Event is ProductEvent productEvent)
                {
                    var key = $"{Constant.CachePrefix}{typeof(ProductDto).FullName}:{productEvent.ProductId}";
                    logger.LogInformation("Remove cache item: {k} to empty", key);

                    await cache.RemoveAsync(key, cancellationToken);
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