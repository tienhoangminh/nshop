namespace nShop.Catalog.DomainEvents;

public abstract class VariantEvent : ProductEvent
{
    public Guid VariantId { get; set; }
}