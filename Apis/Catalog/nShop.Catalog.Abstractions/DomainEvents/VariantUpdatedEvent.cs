namespace nShop.Catalog.DomainEvents;

public class VariantUpdatedEvent : VariantEvent
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
}