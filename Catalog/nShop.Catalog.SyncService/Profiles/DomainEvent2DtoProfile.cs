namespace nShop.Catalog.SyncService.Profiles;

public class DomainEvent2DtoProfile: Profile
{
    public DomainEvent2DtoProfile() {
        CreateMap<ProductCreatedEvent, ProductDto>();
        CreateMap<ProductUpdatedEvent, ProductDto>();
    }
}
