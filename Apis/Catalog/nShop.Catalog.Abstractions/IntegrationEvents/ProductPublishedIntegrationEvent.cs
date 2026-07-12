namespace nShop.Catalog.IntegrationEvents;

public class ProductPublishedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
}