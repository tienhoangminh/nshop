namespace nShop.Catalog.IntegrationEvents;

public class ProductUnpublishedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
}