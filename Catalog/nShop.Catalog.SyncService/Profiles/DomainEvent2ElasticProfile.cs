namespace nShop.Catalog.SyncService.Profiles;

public class DomainEvent2ElasticProfile : Profile
{
    public DomainEvent2ElasticProfile() {
        CreateMap<ProductCreatedEvent, ProductIndexDocument>()
            .ForMember(d => d.Id, e => e.MapFrom(f => f.ProductId))
            .ForMember(d => d.TenantId, e => e.MapFrom(f => f.TenantId));
        CreateMap<ProductUpdatedEvent, ProductIndexDocument>()
            .ForMember(d => d.Id, e => e.MapFrom(f => f.ProductId))
            .ForMember(d => d.TenantId, e => e.MapFrom(f => f.TenantId));
    }
}
