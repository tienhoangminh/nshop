namespace nShop.Catalog.DomainEvents;

public class VariationDimensionValuesChangedEvent : ProductEvent
{
    public string Name { get; set; } = string.Empty;
    public string[] Values { get; set; } = [];
}