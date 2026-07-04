using nShop.Core.IntegrationEvent;

namespace nShop.Catalog.IntegrationEvents;

public class VariantUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid VariantId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;

}
