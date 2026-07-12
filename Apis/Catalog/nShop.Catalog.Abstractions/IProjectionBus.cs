namespace nShop.Catalog;

public interface IProjectionBus
{
    Task PublishAsync(ICatalogEvent @event, CancellationToken cancellationToken = default);
    Task PublishAsync(IEnumerable<ICatalogEvent> events, CancellationToken cancellationToken = default);
}