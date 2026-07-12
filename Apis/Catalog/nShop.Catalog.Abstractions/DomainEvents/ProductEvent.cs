namespace nShop.Catalog.DomainEvents;

public class ProductEvent : ICatalogEvent
{
    public Guid ProductId { get; set; }
    public DateTime Timestamp { get; set; }
}