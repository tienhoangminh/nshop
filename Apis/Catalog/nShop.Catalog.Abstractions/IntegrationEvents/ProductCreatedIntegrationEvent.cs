namespace nShop.Catalog.IntegrationEvents;

public class ProductCreatedIntegrationEvent : ProductBaseIntegrationEvent, ITenancyEntity
{
    public Guid TenantId { get; set; }
}