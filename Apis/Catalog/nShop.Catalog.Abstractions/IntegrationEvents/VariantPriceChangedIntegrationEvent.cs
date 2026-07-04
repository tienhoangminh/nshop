using nShop.Core.IntegrationEvent;

namespace nShop.Catalog.IntegrationEvents;

public class VariantPriceChangedIntegrationEvent : IntegrationEvent
{
    public Guid VariantId { get; set; }
    public Guid ProductId { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }

}