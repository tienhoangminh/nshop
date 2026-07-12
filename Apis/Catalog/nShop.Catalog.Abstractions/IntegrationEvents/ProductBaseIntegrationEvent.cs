namespace nShop.Catalog.IntegrationEvents;

public class ProductBaseIntegrationEvent : IntegrationEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string[] Tags { get; set; } = [];
    public string Slug { get; set; } = string.Empty;
    public string[] Images { get; set; } = [];
    public Guid[] Groups { get; set; } = [];
}

public class VariationDimension
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public VariationDisplayStyles DisplayStyle { get; set; } = VariationDisplayStyles.Text;
    public string[] Values { get; set; } = [];
}

public class VariantDimensionValue
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

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