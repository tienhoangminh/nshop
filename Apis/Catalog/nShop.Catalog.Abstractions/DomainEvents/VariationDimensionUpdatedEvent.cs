namespace nShop.Catalog.DomainEvents;

public class VariationDimensionUpdatedEvent : ProductEvent
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}