namespace nShop.Catalog.Api.Helpers;

public class DomainEventWrapper(IDomainEvent @event) : INotification
{
    public IDomainEvent Event { get; } = @event;
}