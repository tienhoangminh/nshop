using AutoMapper;
using nShop.Catalog.IntegrationEvents;
using nShop.Core.IntegrationEvent;

namespace nShop.Catalog.Api.IntegrationEventPublishers;

public class ProductDomainEventNotificationHandler(IEventBus eventBus, IMapper mapper)
    : INotificationHandler<ProductEvent>
{
    
    public async Task Handle(ProductEvent @event, CancellationToken cancellationToken)
    {
        switch (@event)
        {
            case ProductUpdatedEvent evt:
                await eventBus.PublishAsync(mapper.Map<ProductUpdatedEvent, ProductUpdatedIntegrationEvent>(evt), cancellationToken);
                break;
            case ProductCreatedEvent evt:
                await eventBus.PublishAsync(mapper.Map<ProductCreatedEvent, ProductCreatedIntegrationEvent>(evt), cancellationToken);
                break;
            case ProductPublishedEvent evt:
                await eventBus.PublishAsync(mapper.Map<ProductPublishedEvent, ProductPublishedIntegrationEvent>(evt), cancellationToken);
                break;
            case ProductUnpublishedEvent evt:
                await eventBus.PublishAsync(mapper.Map<ProductUnpublishedEvent, ProductUnpublishedIntegrationEvent>(evt), cancellationToken);
                break;
            case VariantPriceChangedEvent evt:
                await eventBus.PublishAsync(mapper.Map<VariantPriceChangedIntegrationEvent>(evt), cancellationToken);
                break;
            case VariantUpdatedEvent evt:
                await eventBus.PublishAsync(mapper.Map<VariantUpdatedIntegrationEvent>(evt), cancellationToken);
                break;
        }
    }
}