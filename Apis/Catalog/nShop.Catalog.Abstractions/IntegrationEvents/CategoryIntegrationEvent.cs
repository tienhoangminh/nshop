namespace nShop.Catalog.IntegrationEvents;

public abstract class CategoryIntegrationEvent : IntegrationEvent
{
    public Guid CategoryId { get; set; }
    public Guid ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

public class CategoryCreatedIntegrationEvent : CategoryIntegrationEvent
{
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CategoryUpdatedIntegrationEvent : CategoryIntegrationEvent
{
}