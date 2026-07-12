using nShop.Catalog.IntegrationEvents;
using nShop.Core.IntegrationEvent;

namespace nShop.Catalog.Api.IntegrationEventPublishers;

public class CategoryDomainEventNotificationHandler(IEventBus eventBus, IMapper mapper)
    : INotificationHandler<DomainEventWrapper>
{
    public async Task Handle(DomainEventWrapper wrapper, CancellationToken cancellationToken)
    {
        switch (wrapper.Event) {
            case CategoryCreatedEvent evt:
                await eventBus.PublishAsync(mapper.Map<CategoryCreatedEvent, CategoryCreatedIntegrationEvent>(evt), cancellationToken);
                break;
            case CategoryUpdatedEvent evt:
                await eventBus.PublishAsync(mapper.Map<CategoryUpdatedEvent, CategoryUpdatedIntegrationEvent>(evt), cancellationToken);
                break;
        }
    }
}