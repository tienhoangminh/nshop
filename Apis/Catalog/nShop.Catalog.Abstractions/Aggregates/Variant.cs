using nShop.Catalog.DomainEvents;

namespace nShop.Catalog.Aggregates;

public class Variant
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public VariantStates State { get; set; }
    public List<VariantDimensionValue> VariantDimensionValues { get; set; } = [];
}