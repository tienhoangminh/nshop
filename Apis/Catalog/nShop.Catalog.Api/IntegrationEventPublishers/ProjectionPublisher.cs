namespace nShop.Catalog.Api.IntegrationEventPublishers;

public class ProjectionPublisher : INotificationHandler<DomainEventWrapper>
{
    private readonly IProjectionBus projectionBus;
    private readonly ILogger<ProjectionPublisher> logger;

    public ProjectionPublisher(IProjectionBus projectionBus, ILogger<ProjectionPublisher> logger)
    {
        this.projectionBus = projectionBus ?? throw new ArgumentNullException(nameof(projectionBus));
        this.logger = logger;
    }

    public async Task Handle(DomainEventWrapper wrapper, CancellationToken cancellationToken)
    {
        if (wrapper.Event is ICatalogEvent catalogEvent)
        {
            await projectionBus.PublishAsync(catalogEvent, cancellationToken);
        }
    }
}