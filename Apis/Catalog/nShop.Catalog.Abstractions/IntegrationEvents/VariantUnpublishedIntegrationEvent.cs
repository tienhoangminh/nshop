namespace nShop.Catalog.IntegrationEvents;

public class VariantUnpublishedIntegrationEvent: IntegrationEvent
{
    public Guid VariantId { get; set; }
}
