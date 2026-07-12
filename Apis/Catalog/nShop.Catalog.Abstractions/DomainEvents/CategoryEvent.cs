namespace nShop.Catalog.DomainEvents;

public class CategoryEvent : ICatalogEvent
{
    public Guid CategoryId { get; set; }
    public DateTime Timestamp { get; set; }
}