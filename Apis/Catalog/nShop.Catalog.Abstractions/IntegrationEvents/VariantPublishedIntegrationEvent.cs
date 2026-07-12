namespace nShop.Catalog.IntegrationEvents;

public class VariantPublishedIntegrationEvent: IntegrationEvent
{
    public Guid VariantId { get; set; }
}
