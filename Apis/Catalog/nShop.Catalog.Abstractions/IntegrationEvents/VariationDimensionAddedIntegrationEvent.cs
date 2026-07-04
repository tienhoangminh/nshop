using nShop.Catalog.DomainEvents;
using nShop.Core.IntegrationEvent;

namespace nShop.Catalog.IntegrationEvents;
public class VariationDimensionAddedIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public VariationDisplayStyles DisplayStyle { get; set; } = VariationDisplayStyles.Text;
    public string[] Values { get; set; } = [];
}