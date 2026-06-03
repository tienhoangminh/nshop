namespace nShop.Catalog.DomainEvents;

public class VariantCreatedEvent : VariantEvent
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public List<VariantDimensionValue> VariantDimensionValues { get; set; } = [];
}